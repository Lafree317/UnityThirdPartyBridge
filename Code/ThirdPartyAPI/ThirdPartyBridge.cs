//#define NEXT_FOR_WEB

#if UNITY_EDITOR
#define UNITY_EDITOR_RUN
#endif

using System;
using System.Collections.Generic;
using System.Threading;
#if !UNITY_EDITOR_RUN && UNITY_IOS
using System.Runtime.InteropServices;
#endif
using SimpleJSON;
using ThirdParty.Debug;
using ThirdParty.Util;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Ada.ThirdParty.Plugins
{
    //1001001
    public enum CommandType
    {
        // 设备信息
        SystemInfo = 1000000, // 获取设备信息
        RequestAccess = 1000001, // 请求设备权限
        ScreenOrientation = 1000002, // 屏幕方向

        // 登录
        Login = 1001000, // 登录
        Logout = 1001001, // 退出登录
        ModuleSwitch = 1001002, // 提审模块开关

        // H5
        H5AndUnity = 1002000, // H5和Unity互调
        BackFlow = 1002001, // 回流
        CustomChannel = 1002002, // H5和Unity互调
        NativeWebviewHandle = 1002003, // 控制Native的web view
//        StylebookImage = 1002004, // 合拍传递图片,已弃用

        // 分享
        Share = 1003000, // 分享
        PostingStyleBook = 1003001, // 合照分享

        // 支付
        Pay = 1004000, // 支付

        // 打点
        EventTrack = 1005000, // EventTrack
        Mission = 1005001, // 任务
        EventTrackBackEnd = 1005002, // 后端打点

        // 千寻 已弃用
        ScanHead = 1006000, // 是否支持纤寻
        CanScan = 1006001, // 调用纤寻扫脸

        // 菜单栏
        NativeNavbarHandle = 1007000, // Native TabBar控制
        NavbarEvent = 1007001, // Native TabBar事件
        NavbarEnable = 1007002, // Native TabBar是否可点击

        // 路由
        BackToNative = 1009000, // Unity页面回到Native页面  已弃用
        GoToUnity = 1009001, // 跳转Unity页面

        // Network
        BannedUser = 1010000, // 封禁用户 当游戏网络请求返回200007时，唤起弹窗
        ErrorLog = 1010001, // 错误日志

        //User Info
        ProfileImg = 1011001, // 用户更换头像
        Username = 1011002, // 用户更换昵称

        //Audio
        PlayEffect = 1012000, // 播放音效
        PlayMusic = 1012001, // 播放音乐
        EffectSwitch = 1012002, // 音效开关
        MusicSwitch = 1012003, // 音乐开关

        //Screen Capture
        RunWayScreenCapture = 1013000, // 走秀录制视频

        // 弹窗
        RewardPopup = 1014000, // 通用奖励弹窗
        LevelPopup = 1014001, //升级弹窗
        ClosedActivePopup = 1014002, //unity活动弹窗全部关闭之后通知Native

        // 运行环境
        Environment = 1999000, // 切换运行环境
    }

    public enum RequestAccess
    {
        Albume = 0,
    }

    public static partial class ThirdPartyBridge
    {
        private static bool _isInitialized;
        private static readonly Dictionary<int, Action<int, JSONNode, string>> _customCallbackDict;
        private static readonly ThirdPartyUnityDispatch _dispatch;

        static ThirdPartyBridge()
        {
            Log.Info($"ThirdPartyUnity static ctor thread id = {Thread.CurrentThread.ManagedThreadId}", ColorName.Tan);

            _customCallbackDict = new Dictionary<int, Action<int, JSONNode, string>>();

            GameObject gameObject = new GameObject("Third Party Unity Dispatch");
            Object.DontDestroyOnLoad(gameObject);
            _dispatch = gameObject.AddComponent<ThirdPartyUnityDispatch>();
        }

        public static void Init(Action<bool> InResultAction = null)
        {
//            Debug.LogError("Effect volume = "+LocalDataController.GetLocalDataValue_Int(LocalDataController.EffectVolume, 100));
//            Debug.LogError("Bgm volume = " + LocalDataController.GetLocalDataValue_Int(LocalDataController.BGMVolume, 100));

            if (_isInitialized)
            {
                InResultAction?.Invoke(true);
                return;
            }

            //Called when native init is complete.
            Action<bool> nativeInitResultAction = InNativeResult =>
            {
                //native init successfully, init web view.
                Log.Info($"ThirdPartyUnity Init thread id = {Thread.CurrentThread.ManagedThreadId}", ColorName.Tan);
                if (InNativeResult)
                {
                    _isInitialized = true;
                    InResultAction?.Invoke(true);
                }
                else
                {
                    _isInitialized = false;
                    InResultAction?.Invoke(false);
                }
            };

#if UNITY_EDITOR_RUN

#if CHINA_VERSION
            PlayerPrefs.DeleteAll();
#endif
            nativeInitResultAction.Invoke(true);

#elif UNITY_IOS
    #if CHINA_VERSION
            InitCustomCommand(ListenerCallback);
            InitCommandByte(ListenerByteCallback);
    #endif            
            
            nativeInitResultAction.Invoke(true);

#elif UNITY_ANDROID
                _unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                _activity = _unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                if (null == _activity)
                {
                    Log.Exception(new Exception("com.unity3d.player.UnityPlayer.currentActivity is null!"));
                    nativeInitResultAction.Invoke(false);
                }
                else
                {
                    _activity.Call("init", new MsgJavaListener("com.putu.account.inter.IMessageListener",
                        nativeInitResultAction,
                        ListenerCallback,
                        null));
                }
#endif
        }

#if UNITY_EDITOR

        private static void SendCustomCommand(int InCommandType, string InJsonData)
        {
            ThirdPartyNativeSimulate.SendCustomCommand(InCommandType, InJsonData, ListenerCallback);
        }


#elif UNITY_ANDROID
        private static AndroidJavaClass _unityPlayer;
        private static AndroidJavaObject _activity;

        private class MsgJavaListener : AndroidJavaProxy
        {
            private readonly Action<bool> resultCallbackAction;
            private readonly Action<int, string, string> customCallbackAction;
            private readonly Action<int, byte[]> byteCallbackAction;

            public MsgJavaListener(string InJavaInterface,
                Action<bool> InResultCallbackAction,
                Action<int, string, string> InCustomCallbackAction,
                Action<int, byte[]> InByteCallbackAction)
                : base(InJavaInterface)
            {
                resultCallbackAction = InResultCallbackAction;
                customCallbackAction = InCustomCallbackAction;
                byteCallbackAction = InByteCallbackAction;
            }

            public void OnInitResult(bool InResult)
            {
                resultCallbackAction?.Invoke(InResult);
            }

            public void OnCustomCallback(int InCommandType, string InJsonData, string InExtra)
            {
                customCallbackAction?.Invoke(InCommandType, InJsonData, InExtra);
            }

            public void OnByteCallback(int InCommandType, byte[] InBytes)
            {
                byteCallbackAction?.Invoke(InCommandType, InBytes);
            }
        }

        private static void SendCustomCommand(int InCommandType, string InJsonData)
        {
            _activity?.Call("sendCustomCommand", InCommandType, InJsonData);
        }
#elif UNITY_IOS
        public delegate void OnCustomCallback(int InCommandType, string InJsonData, string InExtra);
        
        public delegate void OnByteCallback(int InCommandType, byte[] InBytes);

    #if CHINA_VERSION
        [DllImport("__Internal")]
        private static extern void InitCustomCommand(OnCustomCallback InCallback);
        
        [DllImport("__Internal")]
        private static extern void InitCommandByte(OnByteCallback InCallback);
    #endif

        [DllImport("__Internal")] 
        private static extern void SendCustomCommand(int InCommandType, string InJsonData);
  
        [DllImport("__Internal")]
        private static extern void SendCommandByteCallback(int InCommandType, String InJsonData);
#endif
#if UNITY_IOS && !UNITY_EDITOR
        [AOT.MonoPInvokeCallback(typeof(OnByteCallback))]
#endif
        private static void ListenerByteCallback(int InCommandType, byte[] InBytes)
        {
            Log.Info($"调用了c# callback,type = {InCommandType} ,bytes = {InBytes}");
        }

#if UNITY_IOS && !UNITY_EDITOR
        [AOT.MonoPInvokeCallback(typeof(OnCustomCallback))]
#endif
        private static void ListenerCallback(int InCommandType, string InJsonData, string InExtra)
        {
            Log.Info(
                $"[ThirdPartyBridge.ListenerCallback] : type = {(CommandType) InCommandType}, JsonData = {InJsonData}, Extend = {InExtra}",
                ColorName.Tan);

            _dispatch.Register(_customCallbackDict.TryGetValue(InCommandType, out var callback)
                ? new ThirdPartyParam((CommandType) InCommandType, InJsonData, InExtra, callback)
                : new ThirdPartyParam((CommandType) InCommandType, InJsonData, InExtra));
        }

        public static void ToNative(CommandType InCommandType, JSONNode InJsonNode = null,
            Action<int, JSONNode, string> InCustomCallback = null, bool InNeedLogContent = false)
        {
            string jsonData = InJsonNode == null ? "{}" : InJsonNode.Value;

            Log.Info(
                InNeedLogContent
                    ? $"[ThirdPartyBridge.ToNative] : Type = {InCommandType}, JsonData = dont log"
                    : $"[ThirdPartyBridge.ToNative] : Type = {InCommandType}, JsonData = {InJsonNode}",
                ColorName.Tan);

            int commandType = (int) InCommandType;

            if (null != InCustomCallback) _customCallbackDict[commandType] = InCustomCallback;

            SendCustomCommand(commandType, jsonData);
        }
        
    }
}