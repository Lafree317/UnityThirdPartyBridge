using System.Collections.Generic;
using Ada.Main;
using ADA_Manager;
using INetworkGamePacketResponse;
using SimpleJSON;
using Szn.Framework.Audio;
using ThirdParty.Debug;
using ThirdParty.Util;
using UnityEngine;

namespace Ada.ThirdParty.Plugins
{
    public static class ThirdPartyH5Dispatch
    {
        public static void Handle(string InType, JSONNode InParam, JSONNode InThroughParam, string InExtra)
        {
            JSONNode backDataJson = null;
            switch (InType)
            {
                case "getRoomConfig":
                {
                    var graphicOption = SceneManager.Instance.GetComponent<graphicOptionCTRL>();
                    int defaultPerformance = 2;
                    if (null != graphicOption)
                    {
                        switch (graphicOption.graphicOption)
                        {
                            case graphicOptionCTRL.TierSetting.High:
                                defaultPerformance = 2;
                                break;
                            case graphicOptionCTRL.TierSetting.Mid:
                                defaultPerformance = 1;
                                break;
                        }
                    }

                    backDataJson = new JSONObject
                    {
                        ["result"] = "succeed",
                        ["bgm"] = AudioManager.Instance.MusicVolume,
                        ["effect"] = AudioManager.Instance.EffectVolume,
                        ["performance"] =
                            LocalDataController.GetLocalDataValue_Int(LocalDataController.GraphicOption,
                                defaultPerformance)
                    };
                }
                    break;

                case "setSoundBgm":
                    AudioManager.Instance.MusicVolume = InParam["value"].AsInt;
                    backDataJson = new JSONObject
                    {
                        ["result"] = "succeed"
                    };
                    break;
                case "setSoundEffect":
                    AudioManager.Instance.EffectVolume = InParam["value"].AsInt;
                    backDataJson = new JSONObject
                    {
                        ["result"] = "succeed"
                    };
                    break;
                case "setSoundBGMOn":
                    AudioManager.Instance.MusicSwitch = true;
                    backDataJson = new JSONObject
                    {
                        ["result"] = "succeed"
                    };
                    break;
                case "setSoundBGMOff":
                    AudioManager.Instance.MusicSwitch = false;
                    backDataJson = new JSONObject
                    {
                        ["result"] = "succeed"
                    };
                    break;
                case "setSoundEffectOn":
                    AudioManager.Instance.EffectSwitch = true;
                    backDataJson = new JSONObject
                    {
                        ["result"] = "succeed"
                    };
                    break;
                case "setSoundEffectOff":
                    AudioManager.Instance.EffectSwitch = false;
                    backDataJson = new JSONObject
                    {
                        ["result"] = "succeed"
                    };
                    break;
                case "setGoldValue":
                    int gold = AccountManager.Instance.SetPropertyGold(InParam["value"].AsInt);
                    AdaNetwork.GetProcess<MissionProcess>().missionManager.UpdateMission_GOLD(gold);
                    backDataJson = new JSONObject
                    {
                        ["result"] = "succeed"
                    };
                    break;
                    
                case "setCashValue":
                    int cash = AccountManager.Instance.SetPropertyCash(InParam["value"].AsInt);
                    AdaNetwork.GetProcess<MissionProcess>().missionManager.UpdateMission_CASH(cash);
                    backDataJson = new JSONObject
                    {
                        ["result"] = "succeed"
                    };
                    break;

                case "setGraphicPerf":
                {
                    int quality = InParam["value"].AsInt;

                    var graphicOption = SceneManager.Instance.GetComponent<graphicOptionCTRL>();

                    if (graphicOption != null)
                    {
                        switch (quality)
                        {
                            case 1:
                                graphicOption.SetGraphicTierMid();
                                LocalDataController.SetLocalDataValue_Int(LocalDataController.GraphicOption, quality);
                                break;
                            case 2:
                                graphicOption.SetGraphicTierHigh();
                                LocalDataController.SetLocalDataValue_Int(LocalDataController.GraphicOption, quality);
                                break;
                            default:
                                backDataJson = new JSONObject
                                {
                                    ["result"] = "failed",
                                    ["value"] = 2,
                                    ["msg"] = "Out of range, Set graphic level = 2"
                                };
                                graphicOption.SetGraphicTierHigh();
                                LocalDataController.SetLocalDataValue_Int(LocalDataController.GraphicOption, 2);
                                break;
                        }
                    }                   
                    else
                    {
                        backDataJson = new JSONObject
                        {
                            ["result"] = "failed",
                            ["value"] = 2,
                            ["msg"] = "Not found graphic setting."
                        };
                    }


                    if (null == backDataJson)
                    {
                        backDataJson = new JSONObject
                        {
                            ["result"] = "succeed"
                        };
                    }
                }
                    break;
                
                case "gotoUnityPage":
                    ThirdPartyGotoUnity.GotoUnity(InParam);
                    break;
                
                case "mission":
                    if (InParam != null)
                    {
                        string playType = InParam["play_type"].Value;
                        string categoryType = InParam["category_type"].Value;
                        JSONArray missionTag =  InParam["mission_tag"].AsArray;
                        List<string> list = new List<string>(); 
                        for(int i = 0;i < missionTag.Count ; i++ )
                        {
                            list.Add(missionTag[i]);
                        }
                        MissionProcess.NativeSendMission(playType, categoryType,list.ToArray());
                    }
                    break;
                
                case "missionCheck":
                    var aCharacter = AdaAvatar.Instance.GetCurrCharacter();
                    var partsDatas = aCharacter.GetCurrEquipAllSlotID();
                    int stylebookId = InParam["stylebookId"].AsInt;
                    bool customBg = InParam["customBg"].AsBool;
                    JSONArray stampListArray = InParam["stampList"].AsArray;
                    List<int> stampList = new List<int>();
                    for (int i = 0; i < stampListArray.Count; i++)
                    {
                        stampList.Add(stampListArray[i]);
                    }
                    
                    AdaNetwork.GetProcess<MissionProcess>().StyleBookMissionCheck(stylebookId, partsDatas, stampList.ToArray(), customBg);
                    break;
                
                case "attendMissionCheck":
                    var data = DataTableManager.Instance.dailyAttendanceData.GetDailyData();
                    if (data != null)
                    {
                        AdaNetwork.GetProcess<MissionProcess>().missionManager.UpdateMission(MissionPlay_type.PARTICIPATE, MissionCategory_type.DAILYATTENDANCE, data.mission_tag);
                    }
                    break;
                
                case "playEffect":
                    AudioManager.Instance.PlayEffect((AudioKey)InParam["index"].AsInt);
                    break;
                
                case "playMusic":
                    AudioManager.Instance.PlayMusic((AudioKey)InParam["index"].AsInt);
                    break;
                
                case "mailReceive":
                {
                    Log.Info("开始处理邮件", ColorName.Red);
                    ResponseMailRecive value = JsonUtility.FromJson<ResponseMailRecive>(InParam.Value);
                    MailH5MissionProcess.OnReceiveMailReceive(value);
                    break;
                } 
                case "mialReceiveAll":
                {
                    Log.Info("开始处理全部邮件", ColorName.Red);
                    ResponseMailReciveAll value = JsonUtility.FromJson<ResponseMailReciveAll>(InParam.Value);
                    MailH5MissionProcess.OnReceiveMailReceiveAll(value);
                    break;
                }
                case "ChallengeResult":
                {
                    backDataJson = AdaNetwork.GetProcess<ChallengeProcess>().challengeResult;
                    break;
                }
                default:
                    Log.Error($"Not found third party H5 dispatch handle named {InType}.");
                    backDataJson = new JSONObject()
                    {
                        ["result"] = "failed",
                        ["msg"] = $"Not found third party H5 dispatch handle named {InType}."
                    };
                    break;
            }
        
            Log.Info(
                $"[ThirdPartyH5Dispatch] -- Type = {InType}\nParam = {InParam}\nThroughParam = {InThroughParam}\nBackParam = {backDataJson}");

            ThirdPartyBridge.ToNative(CommandType.H5AndUnity,
                MakeBackJsonNode(InType, backDataJson, InThroughParam));

            ToNativeDone(InType);
        }

        public static JSONNode MakeBackJsonNode(string InType, JSONNode InData, JSONNode InThroughParam)
        {
            return new JSONObject()
            {
                ["result"] = new JSONObject
                {
                    ["type"] = InType,
                    ["data"] = InData,
                },
                ["passThroughParam"] = InThroughParam
            };
        }

        public static void ToNativeDone(string InType)
        {
            switch (InType)
            {
                case "ChallengeResult":
                {
                    AdaNetwork.GetProcess<ChallengeProcess>().challengeResult = null;
                    break;
                }
            }
        }
        
    }
}