using System.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using Ada.Character;
using Ada.Main;
using ADA_Manager;
using INetworkGamePacketResponse;
using SimpleJSON;
using ThirdParty.Debug;
using UnityEngine;
using static StyleBookProcess;
using Debug = UnityEngine.Debug;

namespace Ada.ThirdParty.Plugins
{
    public static class ThirdPartyGotoUnity
    {
        public static void GotoUnity(JSONNode InParam)
        {
            Debug.Log("ThirdPartyGotoUnity GotoUnity " + InParam.ToString());
            string url = InParam["url"];

            if (url == "Stylebook")
            {   // is not page only funciton
                ThirdPartyBridge.SetNativeTabBarActivity(false);
                OnStylebookEdit(InParam);
                return;
            }

            WebViewController.Instance.ActiveGameSceneCam(true);
            Szn.Framework.Audio.AudioManager.Instance.FadeTurnSound(false);
            if (url == "Home")
            {
                ThirdPartyBridge.SetNativeTabBarActivity(true);
                UIPageInterface.OnMainLobby();
                return;
            }
            ThirdPartyBridge.SetNativeTabBarActivity(false);



            switch (url)
            {
                case "RankingInfo":
                    {
                        StyleBookCaptureImage.Instance.SetAllPartsDefault();
                        int type = InParam["type"];
                        int accountId = InParam["account_id"];
                        if (accountId == AccountManager.Instance.GetUserAccountID())
                        {
                            ThirdPartyBridge.SetNativeTabBarActivity(true);
                            UIPageInterface.OnMainLobby();
                        }
                        else
                        {
                            UIPageInterface.OnRankingRoom(accountId, type);
                            EventTrackingBackendProcess.FollowLobbyEvent(EventTrackingBackendProcess.InFolloLobbyType.GotoUnityRank, accountId);
                        }
                        break;
                    }
                case "FollowLobby":
                    {
                        StyleBookCaptureImage.Instance.SetAllPartsDefault();
                        int accountId = InParam["account_id"];
                        if (accountId == AccountManager.Instance.GetUserAccountID())
                        {
                            ThirdPartyBridge.SetNativeTabBarActivity(true);
                            UIPageInterface.OnMainLobby();
                        }
                        else
                        {
                            UIPageInterface.OnFollowLobby(accountId);
                            EventTrackingBackendProcess.FollowLobbyEvent(EventTrackingBackendProcess.InFolloLobbyType.GotoUnityFollow, accountId);
                        }
                        break;
                    }
                case "SeasonBook":
                    {
                        UIPageInterface.OnWebViewSeasonBook();
                        break;
                    }
                case "Mission":
                    {
                        UIPageInterface.OnWebViewMission();
                        break;
                    }
                case "Styling":
                    {
                        var pageinfo = PageManager.Instance.CurrOpenPageInfo();
                        if (pageinfo.CurrPage == PageEnum.ActivityStyling)
                        {
                            return;
                        }
                        int partID = InParam["partId"];
                        bool isEquip = InParam["isEquip"];
                        if(isEquip == true)
                        {
                            AdaAvatar.Instance.EquipPartsItem(AdaAvatar.Instance.selectedCharacterID, partID);
                            UIPageInterface.OnStylingParts(true, partID);
                        }
                        else
                        {
                            UIPageInterface.OnStylingParts(false, partID);
                        }

                        break;
                    }
                case "ActivityStyling":
                    {
                        var pageinfo = PageManager.Instance.CurrOpenPageInfo();
                        if (pageinfo.CurrPage == PageEnum.ActivityStyling)
                        {
                            return;
                        }
                        int challengeId = InParam["challengeId"];
                        int partID = InParam["partId"];
                        UIPageInterface.OnStylingActivityParts(challengeId, partID);
                        break;
                    }
                case "Challenge":
                    {
                        UIPageInterface.OnWebViewStyleChallenge();
                        break;
                    }
                case "CelebList":
                    {
                        UIPageInterface.OnCelebritySelect();
                        break;
                    }
                case "RoomChange":
                    {
                        UIPageInterface.OnRoomChange();
                        break;
                    }
                case "ActivityLookRoom":
                    {
                        ChallengeH5Data challengeH5Data = ChallengeH5Data_Parser.Parsing(InParam["challengeData"]);
                        if (challengeH5Data == null)
                        {
                            return;
                        }
                        string viewPage = InParam["viewPage"];
                        string webUrl = InParam["webUrl"];
                        int webType = InParam["webType"];
                        ICharacterEquipParts equip = JsonUtility.FromJson<ICharacterEquipParts>(InParam["equip"]);
                        UIPageInterface.OnActivityStylingChallenge(challengeH5Data, viewPage, equip, webUrl: webUrl, webType: webType);
                        break;
                    }
                case "ActivityLook":
                    {
                        ICharacterEquipParts equip = JsonUtility.FromJson<ICharacterEquipParts>(InParam["equip"]);
                        var pageinfo = PageManager.Instance.CurrOpenPageInfo();
                        if (pageinfo.CurrPage == PageEnum.ActivityStyling)
                        {
                            var adaCharacter = AdaAvatar.Instance.GetCurrCharacter();
                            adaCharacter.EquipLook(equip);
                        }
                        else
                        {
                            UIPageInterface.OnActivityStylingLook(equip);
                        }
                        break;
                    }
                case "ChatRoomRunWay":
                    {
                        UIPageInterface.OnChatRoomRunWay();
                        break;
                    }
                case "ChallengeRoom":
                    {
                        ChallengeH5Data challengeH5Data = ChallengeH5Data_Parser.Parsing(InParam["challengeData"]);
                        if(challengeH5Data == null)
                        {
                            return;
                        }
                        AdaNetwork.GetProcess<ChallengeProcess>().backH5DataJson = InParam["challengeData"];
                        UIPageInterface.OnStylingChallenge(challengeH5Data);
                        break;
                    }
            }

        }

        static void OnStylebookEdit(JSONNode InParam)
        {
            JSONArray account_id_list = InParam["account_id_list"].AsArray;
            int stylebook_id = InParam["stylebook_id"];
            PageManager.Instance.PrevNativeData = InParam;
            // 初始化合拍数据
            StyleBookProcess.SelectedPeopleInfoClear();
            // 单人合拍
            if (account_id_list.Count <= 1)
            {
                // UIPageInterface.OnStyleBookEdit(stylebook_id);
                StyleBookCaptureImage.Instance.StartCapture(stylebook_id);
                return;
            }
            AdaNetwork.GetProcess<StyleBookProcess>().InitSelectPeopleInfoList(account_id_list);
            StyleBookCaptureImage.Instance.MultiplayerCapture(stylebook_id);
        }

    }
}
