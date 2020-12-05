using System;
using Ada.Main;
using SimpleJSON;
using ThirdParty.Debug;
using ThirdParty.Util;

namespace Ada.ThirdParty.Plugins
{
    public static partial class ThirdPartyBridge
    {
        public static void SetNativeTabBarActivity(bool InActivity)
        {
            string jsonObject = $"{{\"isShowNav\":{(InActivity ? "true" : "false")}}}";

            Log.Info($"[ThirdPartyBridge.ToNative] : Type = {CommandType.NativeNavbarHandle}, JsonData = {jsonObject}",
                ColorName.Tan);

            SendCustomCommand((int) CommandType.NativeNavbarHandle, jsonObject);
        }
        
        public static void SetNativeTabBarEnable(bool InEnable)
        {
            string jsonObject = $"{{\"isEnable\":{(InEnable ? "true" : "false")}}}";

            Log.Info($"[ThirdPartyBridge.ToNative] : Type = {CommandType.NavbarEnable}, JsonData = {jsonObject}",
                ColorName.Tan);

            SendCustomCommand((int) CommandType.NavbarEnable, jsonObject);
        }

        public static void OpenNativeWebView(string InUrl, bool InIsShowTitleBar = false, string InTitle = null,
            int InWebType = 1, bool InWebviewHidden = false)
        {
            if (string.IsNullOrEmpty(InUrl))
            {
                Log.Error("Can not open empty page.");
                return;
            }

            WebViewController.Instance.ActiveGameSceneCam(InWebviewHidden);
            Szn.Framework.Audio.AudioManager.Instance.FadeTurnSound(true);
            if (InWebType != 1)
            {
                InIsShowTitleBar = true;
            }

            JSONObject jsonObject = new JSONObject()
            {
                ["url"] = InUrl,
                ["showTitleBar"] = InIsShowTitleBar ? "true" : "false",
                ["InWebType"] = InWebType
            };
            if (!string.IsNullOrEmpty(InTitle)) jsonObject["title"] = InTitle;

            Log.Info($"[ThirdPartyBridge.ToNative] : Type = {CommandType.NativeWebviewHandle}, JsonData = {jsonObject}",
                ColorName.Tan);

            SendCustomCommand((int) CommandType.NativeWebviewHandle, jsonObject);
        }

        public static void ShareToNative(string InPicturePath, int InContentType = 0,
            Action<bool> InCallbackAction = null)
        {
            JSONObject jsonObject = new JSONObject
            {
                ["sinaTitle"] = "",
                ["friendTitle"] = "",
                ["zoneTitle"] = "",
                ["des"] = "",
                ["url"] = "",
                ["imgUrl"] = "",
                ["logoDes"] = "",
                ["logoName"] = "",
                ["appBrandPath"] = "",
                ["passThrougnParam"] = "",
                ["requestParam"] = "",
                ["contentType"] = InContentType,
                ["withShareTicket"] = false,
                ["miniProgramType"] = 0,
                ["imgLocalPath"] = InPicturePath
            };

            Log.Info($"[ThirdPartyBridge.ToNative] : Type = {CommandType.Share}, JsonData = {jsonObject}",
                ColorName.Tan);

            _customCallbackDict[(int) CommandType.Share] = (InCommandType, InJsonNode, InExtra) =>
            {
                bool result = InJsonNode["shareResult"].AsInt == (int) Result.Success;
                InCallbackAction?.Invoke(result);
                if (result)
                {
                    AdaNetwork.GetProcess<MissionProcess>().SocialTrackingInterface("ACTIVITY", "SHARE");
                }
            };

            SendCustomCommand((int) CommandType.Share, jsonObject);
        }

        public static void ShowNativeRewardPopup(JSONNode InJsonNode)
        {
            if (null == InJsonNode) return;

            JSONArray jsonArray = InJsonNode["rewardList"].AsArray;

            int count;
            if (jsonArray != null &&  (count = jsonArray.Count) > 0)
            {
                ToNative(CommandType.RewardPopup, InJsonNode, (InI, InData, InExtra)=>
                {
                    for (int i = 0; i < count; i++)
                    {
                        JSONNode subNode = jsonArray[i];
                        switch (subNode["type"].Value)
                        {
                            case "cash":
                                AccountManager.Instance.SetPropertyCash(AccountManager.Instance.GetPropertyCash() + subNode["count"].AsInt);
                                break;
                            
                            case "gold":
                                AccountManager.Instance.SetPropertyGold(AccountManager.Instance.GetPropertyGold() + subNode["count"].AsInt);
                                break;
                        }
                    }
                });
            }
        }
    }
}