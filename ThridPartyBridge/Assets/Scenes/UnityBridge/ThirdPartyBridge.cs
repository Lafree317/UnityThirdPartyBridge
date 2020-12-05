using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static partial class ThirdPartyBridge
{
    private static bool _isInitialized;
    private static readonly Dictionary<int, string> _customCallbackDict;
    private static readonly ThirdPartyDispatch _dispatch;

    public delegate void OnCustomCallback(int InCommandType,string InJsonData,string InExtra);

    [DllImport("__Internal")]
    private static extern void InitCustomCommand(OnCustomCallback InCallback);

    static ThirdPartyBridge()
    {
        _customCallbackDict = new Dictionary<int, string>();
        GameObject gameObject = new GameObject("Third Party Unity dispatch");
        Object.DontDestroyOnLoad(gameObject);
        _dispatch = gameObject.AddComponent<ThirdPartyDispatch>();
    }
}
