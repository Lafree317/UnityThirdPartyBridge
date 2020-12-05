using System.Collections.Generic;
using Ada.Main;
using ADA_Manager;
using SimpleJSON;
using Szn.Framework.Audio;
using Szn.Framework.UtilPackage.Event;
using ThirdParty.Debug;

namespace Ada.ThirdParty.Plugins
{
    public static class ThirdPartyDispatch
    {
        private static int nativeBarState = -1;
        public static void Handle(CommandType InCommandType, JSONNode InJsonNode, string InExtra)
        {
            switch (InCommandType)
            {
                case CommandType.Logout:
                    AccountManager.Instance.ClearPlayerPrefs();
                    GameStarter.Instance.GameLogout();
                    break;

                case CommandType.Mission:
                    if (null != InJsonNode)
                    {
                        string playType = InJsonNode["play_type"].Value;
                        string categoryType = InJsonNode["category_type"].Value;
                        JSONArray missionTag = InJsonNode["mission_tag"].AsArray;
                        List<string> list = new List<string>();
                        for (int i = 0; i < missionTag.Count; i++)
                        {
                            list.Add(missionTag[i]);
                        }
                        MissionProcess.NativeSendMission(playType, categoryType, list.ToArray());
                    }

                    break;
               
                case CommandType.H5AndUnity:
                    if (null != InJsonNode)
                    {
                        JSONNode param = InJsonNode["requestParam"];
                        ThirdPartyH5Dispatch.Handle(param["type"].Value, param["data"], InJsonNode["passThroughParam"],
                            InExtra);
                    }

                    break;
                
                case CommandType.NavbarEvent:
                    if (null != InJsonNode)
                    {
                        int type = InJsonNode["type"].AsInt;
                        if (type != nativeBarState)
                        {
                            nativeBarState = type;

                            //SetUnityActivity(type == 1);
                            AudioManager.Instance.FadeTurnSound(type != 1);
                            EventManager.Instance.Trigger(EventKey.OnTabBarClicked, type);
                        }
                    }
                    break;

                case CommandType.NativeWebviewHandle:
                {
                    if (null != InJsonNode)
                    {
                        if (InJsonNode["type"].Value == "close")
                        {
                            if (PageManager.Instance.CurrOpenPageInfo() is MainPage)
                            {
                                ThirdPartyBridge.SetNativeTabBarActivity(true);
                            }
                            else
                            {
                                ThirdPartyBridge.SetNativeTabBarActivity(false);
                            }

                            WebViewController.Instance.ActiveGameSceneCam(true);
                        }
                    }
                }
                    break;

                case CommandType.ProfileImg:
                {
                    AccountManager.Instance.SetUserThumbnailURL(InJsonNode["ProfileImage"].Value);
                }
                    break;

                case CommandType.Username:
                {
                    AccountManager.Instance.SetUserName(InJsonNode["Username"].Value);
                }
                    break;
                
                case CommandType.PlayEffect:
                    AudioManager.Instance.PlayEffect((AudioKey)InJsonNode["index"].AsInt);
                    break;
                
                case CommandType.PlayMusic:
                    AudioManager.Instance.PlayMusic((AudioKey)InJsonNode["index"].AsInt);
                    break;
                
                case CommandType.EffectSwitch:
                    AudioManager.Instance.EffectSwitch = InJsonNode["Switch"].Value == "On";
                    break;
                
                case CommandType.MusicSwitch:
                    AudioManager.Instance.MusicSwitch = InJsonNode["Switch"].Value == "On";
                    break;
                
                case CommandType.GoToUnity:
                    ThirdPartyGotoUnity.GotoUnity(InJsonNode);
                    break;

                default:
                    Log.Error($"Not found third party dispatch handle named {InCommandType}.");
                    break;
            }

            Log.Info($"[ThirdPartyH5Dispatch] -- Type = {InCommandType}\nParam = {InJsonNode}");
        }

//        private static void SetUnityActivity(bool InActivity)
//        {
//            AudioManager.Instance.FadeTurnSound(!InActivity);
//            WebViewController.Instance.ActiveGameSceneCam(InActivity);
//        }
    }
}