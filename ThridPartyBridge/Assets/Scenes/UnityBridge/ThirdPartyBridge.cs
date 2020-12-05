using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static partial class ThirdPartyBridge
{
    private static bool _isInitialized;
    private static readonly Dictionary<int, Action<int,string>> _customCallbackDict;
    private static readonly ThirdPartyDispatch _dispatch;

    public delegate void OnCustomCallback(int InCommandType,string InJsonData,string InExtra);

    [DllImport("__Internal")]
    private static extern void InitCustomCommand(OnCustomCallback InCallback);

    static ThirdPartyBridge()
    {
        _customCallbackDict = new Dictionary<int, Action<int,string>>();
        GameObject gameObject = new GameObject("Third Party Unity dispatch");
        GameObject.DontDestroyOnLoad(gameObject);
        _dispatch = gameObject.AddComponent<ThirdPartyDispatch>();
    }

    public static void Init(Action<bool> InResultAction = null)
    {

#if UNITY_IOS
        InitCustomCommand(ListenerCallback);
        InitCommandByte(ListenerByteCallback);
#endif  

#if UNITY_ANDROID
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

#if UNITY_ANDROID
    private static AndroidJavaClass _unityPlayer;
    private static AndroidJavaObject _activity;

    private static void SendCustomCommand(int InCommandType, string InJsonData)
    {
        _activity?.Call("sendCustomCommand", InCommandType, InJsonData);
    }
#endif

#if UNITY_EDITOR
    private static void SendCustomCommand(int InCommandType, string InJsonData)
    {

    }
#endif



#if UNITY_IOS
    [DllImport("__Internal")] 
    private static extern void SendCustomCommand(int InCommandType, string InJsonData);
#endif

    [AOT.MonoPInvokeCallback(typeof(OnCustomCallback))]
    private static void ListenerCallback(int InCommandType, string InJsonData, string InExtra)
    {

        _dispatch.Register(_customCallbackDict.TryGetValue(InCommandType, out var callback)
            ? new ThirdPartyParam((CommandType) InCommandType, InJsonData, InExtra, callback)
            : new ThirdPartyParam((CommandType) InCommandType, InJsonData, InExtra));
    }


    public static void ToNative(CommandType InCommandType,string InJsonData, Action<int,string> InCustomCallback = null)
    {
        int commandType = (int) InCommandType;
        if (null != InCustomCallback) _customCallbackDict[commandType] = InCustomCallback;
        SendCustomCommand(commandType, InJsonData);
    }
}


public class MsgJavaListener : AndroidJavaProxy
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