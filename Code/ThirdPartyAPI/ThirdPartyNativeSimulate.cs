#if UNITY_EDITOR
using System;
using ADA_Manager;
using SimpleJSON;
using Szn.Framework.UtilPackage;
using ThirdParty.Debug;
using ThirdParty.Util;
using UnityEngine;

namespace Ada.ThirdParty.Plugins
{
    public static class ThirdPartyNativeSimulate
    {
        public static readonly string CURRENT_UID_PREF_KEY = "CurrentUidPrefKey";
        public static readonly string CURRENT_TOKEN_PREF_KEY = "CurrentTokenPrefKey";
        public static readonly string CURRENT_RUN_MODE_PREF_KEY = "CurrentRunModePrefKey";


        public static void SendCustomCommand(int InCommandType, string InJsonData,
            Action<int, string, string> InListenerCallback)
        {
            if (null == InListenerCallback) return;

            JSONObject jsonObject = new JSONObject();
            string extend = string.Empty;

            switch (InCommandType)
            {
                case (int) CommandType.Login:
                    string uid = UnityEditor.EditorPrefs.GetString(CURRENT_UID_PREF_KEY);
                    string token = UnityEditor.EditorPrefs.GetString(CURRENT_TOKEN_PREF_KEY);

                    RunMode runMode =
                        (RunMode) UnityEditor.EditorPrefs.GetInt(CURRENT_RUN_MODE_PREF_KEY, (int) RunMode.Test);
                    UnityEditor.EditorPrefs.SetInt(CURRENT_RUN_MODE_PREF_KEY, (int) runMode);

                    if (string.IsNullOrEmpty(uid) || string.IsNullOrEmpty(token))
                    {
                        SimulateLogin.GetToke(runMode, "15201346562", (InResult, InUid, InToken) =>
                        {
                            if (InResult)
                            {
                                Log.Info(
                                    $"Login from server config in {runMode.ToString()} mode, uid = {uid}, token = {token}",
                                    ColorName.Red);

                                UnityEditor.EditorPrefs.SetString(CURRENT_UID_PREF_KEY, InUid != "" ? InUid : "123");
                                UnityEditor.EditorPrefs.SetString(CURRENT_TOKEN_PREF_KEY, InToken);
                                InListenerCallback(InCommandType, jsonObject.ToString(), extend);
                            }
                            else
                            {
                                Log.Error($"Get token error. msg = {InUid}");
                                Application.Quit();
                            }
                        });
                    }
                    else
                    {
                        Log.Info($"Login from local config in {runMode.ToString()} mode, uid = {uid}, token = {token}",
                            ColorName.Red);

                        jsonObject["uid"] = uid;
                        jsonObject["token"] = token;
                        InListenerCallback(InCommandType, jsonObject.ToString(), extend);
                    }

                    //                    jsonObject["uid"] = UnityEditor.EditorPrefs.GetString(CURRENT_UID_PREF_KEY);
                    //                    jsonObject["token"] = "e7f450f0c7e8e971206f96f64e3227f093257bd3";
                    //                    ListenerCallback(InCommandType, jsonObject.ToString(), extend);
                    break;


                case (int) CommandType.Share:
                    jsonObject["shareType"] = (int) ShareType.QQ;
                    jsonObject["shareResult"] = 0;
                    InListenerCallback(InCommandType, jsonObject.ToString(), extend);
                    break;

                case (int) CommandType.Pay:
                    jsonObject["goodsID"] = "goodsId." + UnityEngine.Random.Range(1000, int.MaxValue);
                    jsonObject["goodsOrderID"] = "goodsOrderID." + UnityEngine.Random.Range(1000, int.MaxValue);
                    jsonObject["payOrderID"] = "payOrderID." + UnityEngine.Random.Range(1000, int.MaxValue);
                    jsonObject["payResult"] = 0;
                    jsonObject["payWay"] = (int) PayWay.AliPay;
                    jsonObject["paymentNum"] = "paymentNum." + UnityEngine.Random.Range(1000, int.MaxValue);
                    InListenerCallback(InCommandType, jsonObject.ToString(), extend);
                    break;

                case (int) CommandType.Environment:
                    switch (UnityEditor.EditorPrefs.GetInt(CURRENT_RUN_MODE_PREF_KEY))
                    {
                        case 0:
                            jsonObject["environment_type"] = Define_China.CHINA_CONFIG_DEV;
                            break;

                        case 1:
                            jsonObject["environment_type"] = Define_China.CHINA_CONFIG_TEST;
                            break;

                        case 2:
                            jsonObject["environment_type"] = Define_China.CHINA_CONFIG_ONLINE;
                            break;
                    }

                    InListenerCallback(InCommandType, jsonObject.ToString(), extend);
                    break;

                case (int) CommandType.PostingStyleBook:
                    jsonObject["inTimeResult"] = 0;
                    InListenerCallback(InCommandType, jsonObject.ToString(), extend);
                    break;

                case (int) CommandType.CustomChannel:
                    JSONObject subJson = new JSONObject();
                    subJson["type"] = "1";
                    subJson["resultCode"] = "0";
                    jsonObject["result"] = subJson;
                    jsonObject["passThrougnParam"] = new JSONObject();
                    InListenerCallback(InCommandType, jsonObject.ToString(), extend);
                    break;

                case (int) CommandType.ScanHead:
                    jsonObject["faceScanResult"] = "0";
                    InListenerCallback(InCommandType, jsonObject.ToString(), extend);
                    break;

                case (int) CommandType.CanScan:
                    jsonObject["faceScanConfResult"] = "0";
                    InListenerCallback(InCommandType, jsonObject.ToString(), extend);
                    break;

                case (int) CommandType.NativeNavbarHandle:
#if CHINA_VERSION
//                    JSONNode node = JSONNode.Parse(InJsonData);
//                    MainPage mainPage = PageManager.Instance.CurrOpenPageInfo() as MainPage;
//                    if (mainPage != null)
//                    {
//                        mainPage.SetUnityTabBarActivity(node["isShowNav"].Value == "true");
//                    }
#endif
                    break;
                case (int) CommandType.RequestAccess:
                    jsonObject["result"] = 0;
                    InListenerCallback(InCommandType, jsonObject.ToString(), extend);
                    break;

                case (int) CommandType.ProfileImg:
                    jsonObject["ProfileImage"] =
                        "https://imgsa.baidu.com/exp/w=480/sign=fc089cc8a7014c08193b29ad3a7b025b/f31fbe096b63f6247e4c28628944ebf81a4ca3b9.jpg";
                    InListenerCallback(InCommandType, jsonObject.ToString(), extend);
                    break;
                case (int) CommandType.NativeWebviewHandle:
                    break;
                case (int) CommandType.RunWayScreenCapture:
                    jsonObject["result"] = 0;
                    jsonObject["data"] = new JSONObject
                    {
                        ["post_id"] = -1
                    };
                    InListenerCallback.Invoke(InCommandType, jsonObject.Value, null);
                    break;
            }
        }
    }
}
#endif