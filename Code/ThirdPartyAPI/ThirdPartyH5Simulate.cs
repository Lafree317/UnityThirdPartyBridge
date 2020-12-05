#define UNIT_TEST
using System;
using System.Collections.Generic;
using System.IO;
using Ada.Main;
using ADA_Manager;
using SimpleJSON;
using ThirdParty.Debug;
using ThirdParty.Util;
using UnityEngine;
using Ada.Character;
using INetworkGamePacketResponse;

namespace Ada.ThirdParty.Plugins
{
    public class ThirdPartyH5Simulate : MonoBehaviour
    {
#if CHINA_VERSION && UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void Init()
        {
            GameObject go = new GameObject("H5 Simulate");
            DontDestroyOnLoad(go);
            go.AddComponent<ThirdPartyH5Simulate>();
        }
#endif
        private float width;
        private float height;
        private Rect logWinRect;
        private Rect screenRect;
        
        private struct ActionInfo
        {
            public string Name;
            public Action DoAction;

            public ActionInfo(string InName, Action InAction)
            {
                Name = InName;
                DoAction = InAction;
            }
        }
        private List<ActionInfo> testAction = new List<ActionInfo>(16);
        private int testActionCount;

        private void Awake()
        {
            width = Screen.width;
            height = Screen.height;
            logWinRect = new Rect(width * .375f, height * .4f, width * .25f, height * .2f);
            screenRect = new Rect(0, 0, width, height);
            
            testAction = new List<ActionInfo>
            {
                // new ActionInfo("Styling",Styling),
                // new ActionInfo("BackMainLooby",BackMainLooby),
                // new ActionInfo("TestTryOnLook",TestTryOnLook),
                new ActionInfo("TestActivityLook",TestActivityLook),
                
            };

            testActionCount = testAction.Count;
        }

        private void OnGUI()
        {
// #if UNITY_EDITOR
            logWinRect = GUI.Window(0, logWinRect, LogWindow, "H5 Simulate");
// #endif
        }

        private void LogWindow(int InWindowId)
        {
            for (int i = 0; i < testActionCount; i++)
            {
                
                if (GUILayout.Button(testAction[i].Name))
                {
                    testAction[i].DoAction.Invoke();
                }
            }


            GUI.DragWindow(screenRect);
        }

        static void Styling()
        {
            JSONObject param = new JSONObject()
            {
                ["url"] = "Styling",
                ["partId"] = 2000003,
                ["isEquip"] = false,
            };
            ThirdPartyH5Dispatch.Handle("gotoUnityPage", param, null, string.Empty);
        }
        private static void TestChallenge()
        {
            string jsonFilePath =
                Path.Combine(Application.dataPath, "Main/Script/ThirdPartyAPI/TestData/ChalllengeData.json");
            string json = File.ReadAllText(jsonFilePath);
            JSONObject param = new JSONObject()
            {
                ["url"] = "ChallengeRoom",
                ["challengeData"] = json,
            };
            Log.Info(param.Value, ColorName.Red);
            ThirdPartyH5Dispatch.Handle("gotoUnityPage", param, null, string.Empty);
            UIPageInterface.OnStudioShot(100001, 3);
        }

        private static void TestActivityLook()
        {
            string jsonFilePath =
                Path.Combine(Application.dataPath, "Main/Script/ThirdPartyAPI/TestData/ChalllengeData.json");
            string json = File.ReadAllText(jsonFilePath);
            JSONObject param = new JSONObject()
            {
                ["url"] = "ActivityLookRoom",
                ["challengeData"] = json,
                ["viewPage"] = "Look",
                ["webUrl"] = "http://www.baidu.com",
                ["webType"] = 1,
            };
            Log.Info(param.Value, ColorName.Red);
            ThirdPartyH5Dispatch.Handle("gotoUnityPage", param, null, string.Empty);
        }
        static void HideLoading()
        {
            PageManager.Instance.HideLoading();
        }


        private static void TestAddCharacter()
        {
            TestCharacter.Instance.AddCharacter();
        }
        private static void TestAttendanceMission()
        {
            var data = DataTableManager.Instance.dailyAttendanceData.GetDailyData();
            if (data != null)
            {
                AdaNetwork.GetProcess<MissionProcess>().missionManager.UpdateMission(MissionPlay_type.PARTICIPATE,
                    MissionCategory_type.DAILYATTENDANCE, data.mission_tag);
            }
        }

        #region  Styling
        static void TestChatRoomRunWay()
        {
            JSONObject param = new JSONObject()
            {
                ["url"] = "ChatRoomRunWay",
            };
            ThirdPartyH5Dispatch.Handle("gotoUnityPage", param, null, string.Empty);
        }

        static void TestChallengeResult()
        {

            ThirdPartyH5Dispatch.Handle("ChallengeResult", null, null, string.Empty);
        }



        static void TestActivityStylingChallenge()
        {
            JSONObject param = new JSONObject()
            {
                ["url"] = "ActivityLookRoom",
                ["challengeId"]=109999,
                ["viewPage"] = "Look",
            };
            ThirdPartyH5Dispatch.Handle("gotoUnityPage", param, null, string.Empty);
        }

        static void TestParts()
        {
            JSONObject param = new JSONObject()
            {
                ["url"] = "Styling",
                ["partId"] = 100074,
                ["isEquip"] = true,
            };
            ThirdPartyH5Dispatch.Handle("gotoUnityPage", param, null, string.Empty);
        }


        static void Logout()
        {
            GameStarter.Instance.GameLogout();
        }

        static void TestMission()
        {
            JSONArray tagArr = new JSONArray();
            tagArr.Add("PEOPLE_2");
            JSONObject param = new JSONObject()
            {
                ["play_type"] = "SHOOTING",
                ["category_type"] = "STYLEBOOK",
                ["mission_tag"] = tagArr,
            };
            ThirdPartyH5Dispatch.Handle("mission", param, null, string.Empty);
        }

        static void TestActivityRunway()
        {

        }
        #endregion

        private static void Ranking()
        {
            JSONObject param = new JSONObject()
            {
                ["url"] = "RankingInfo",
                ["type"] = 1,
                ["account_id"] = "100000002",
            };
            ThirdPartyH5Dispatch.Handle("gotoUnityPage", param, null, string.Empty);
        }

        private static void FollowLobby()
        {
            JSONObject param = new JSONObject()
            {
                ["url"] = "FollowLobby",
                ["account_id"] = "100007908",
            };
            ThirdPartyH5Dispatch.Handle("gotoUnityPage", param, null, string.Empty);
        }


        private static void TestMailRecive()
        {
            string jsonFilePath =
                Path.Combine(Application.dataPath, "Main/Script/ThirdPartyAPI/TestData/testRecive.json");
            Log.Info(jsonFilePath, ColorName.Red);
            string json = File.ReadAllText(jsonFilePath);

            JSONObject param = JSONNode.Parse(json).AsObject;
            Log.Info(param.Value, ColorName.Red);
            ThirdPartyH5Dispatch.Handle("mailReceive", param, null, string.Empty);
        }

        private static void TestMailReciveAll()
        {
            string jsonFilePath = Path.Combine(Application.dataPath,
                "Main/Script/ThirdPartyAPI/TestData/testReciveAll.json");
            Log.Info(jsonFilePath, ColorName.Red);
            string json = File.ReadAllText(jsonFilePath);

            JSONObject param = JSONNode.Parse(json).AsObject;
            Log.Info(param.Value, ColorName.Red);
            ThirdPartyH5Dispatch.Handle("mialReceiveAll", param, null, string.Empty);
        }

        private static void TestStylebookMission()
        {
            JSONArray stampList = new JSONArray();
            stampList.Add(1);
            stampList.Add(2);
            JSONObject param = new JSONObject
            {
                ["stylebookId"] = "1", ["customBg"] = true, ["stampList"] = stampList
            };
            ThirdPartyH5Dispatch.Handle("missionCheck", param, null, string.Empty);
        }


        private static void TestGotoStyleBook()
        {
            JSONArray account_id_list = new JSONArray();
            account_id_list.Add(100005072);
            JSONObject param = new JSONObject
            {
                ["url"] = "Stylebook",
                ["account_id_list"] = account_id_list,
                ["stylebook_id"] = 1
            };
            ThirdPartyH5Dispatch.Handle("gotoUnityPage", param, null, string.Empty);
        }


        private static void TestGotoCaptureMultiPeople()
        {
            JSONArray account_id_list = new JSONArray();
            account_id_list.Add(100002576);
            account_id_list.Add(100000002);
            account_id_list.Add(100005029);
            JSONObject param = new JSONObject
            {
                ["url"] = "Stylebook", ["account_id_list"] = account_id_list, ["stylebook_id"] = 72
            };
            ThirdPartyH5Dispatch.Handle("gotoUnityPage", param, null, string.Empty);
        }

        private static void TestGotoCapture()
        {
            JSONArray account_id_list = new JSONArray();
            account_id_list.Add(-1);

            JSONObject param = new JSONObject
            {
                ["url"] = "Stylebook", ["account_id_list"] = account_id_list, ["stylebook_id"] = 111
            };
            ThirdPartyH5Dispatch.Handle("gotoUnityPage", param, null, string.Empty);
        }

        private static void TestRoomChange()
        {
            JSONObject param = new JSONObject
            {
                ["url"] = "RoomChange",
            };
            ThirdPartyH5Dispatch.Handle("gotoUnityPage", param, null, string.Empty);
        }

        private static void ChangeLook()
        {
            string jsonFilePath = Path.Combine(Application.dataPath, "Main/Script/ThirdPartyAPI/TestData/LookTest.json");
            string json = File.ReadAllText(jsonFilePath);
            ResponseLookInfoList lookInfo = JsonUtility.FromJson<ResponseLookInfoList>(json);
            AdaCharacter character =  AdaAvatar.Instance.GetCurrCharacter();
            character.EquipLook(lookInfo.command.lookList[0].equip);
        }

        private static void RunWayChallenge()
        {
            UIPageInterface.OnRunwayChallenge(1);
        }

        private static void OpenRunWay()
        {
//            CnRunWay.Instance.Init();
        }
        
    }
}
//#endif