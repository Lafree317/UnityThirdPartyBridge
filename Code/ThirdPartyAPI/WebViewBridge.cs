using System;
using System.Collections.Generic;
using Szn.Framework.UtilPackage;
using ThirdParty.Debug;
using ThirdParty.Util;
using UnityEngine;

namespace Ada.ThirdParty.Plugins
{
    public static class WebViewBridge
    {
        private static UniWebView _webView;
        private static bool isWebViewShowing;
        private static readonly Dictionary<string, Action<UniWebViewMessage>> _webViewCallbackDict;

#if !UNITY_EDITOR && UNITY_IOS 
		[System.Runtime.InteropServices.DllImport("__Internal")]
		private static extern void RegisterKeyboardNotification();
#endif
        
        static WebViewBridge()
        {
            UniWebView.SetAllowAutoPlay(true);
            UniWebView.SetAllowInlinePlay(true);

            _webViewCallbackDict = new Dictionary<string, Action<UniWebViewMessage>>();
        }
        
        

        public static void RegisterWebViewCallback(string InCallbackName, Action<UniWebViewMessage> InCallbackAction)
        {
            if (_webViewCallbackDict.ContainsKey(InCallbackName))
            {
                _webViewCallbackDict[InCallbackName] += InCallbackAction;
            }
            else
            {
                _webViewCallbackDict.Add(InCallbackName, InCallbackAction);
            }
        }

        public static void UnregisterWebViewCallback(string InCallbackName, Action<UniWebViewMessage> InCallbackAction)
        {
            if (_webViewCallbackDict.ContainsKey(InCallbackName))
            {
                // ReSharper disable once DelegateSubtraction
                _webViewCallbackDict[InCallbackName] -= InCallbackAction;
            }
            else
            {
                Log.Warning($"[WebViewBridge] No callback named {InCallbackName} is registered!");
            }
        }

        public static void Init(Action<bool> InInitCallback)
        {
            if (_webView != null)
            {
                InInitCallback?.Invoke(true);
                return;
            }
            
            _webView = UnityEngine.Object.FindObjectOfType<UniWebView>();
            if (_webView != null)
            {
                InInitCallback?.Invoke(true);
                return;
            }
            
            GameObject gameObject = GameObject.Find("UniWebView");
            if (null == gameObject) gameObject = new GameObject("UniWebView");
            _webView = gameObject.AddComponent<UniWebView>();
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
                
            _webView.SetShowToolbar(false);
            _webView.SetBackButtonEnabled(false);
            _webView.SetBouncesEnabled(false);
            _webView.SetAllowFileAccessFromFileURLs(true);
            _webView.BackgroundColor = Color.white;

            UniWebView.PageFinishedDelegate onPageFinished = null;

            onPageFinished = (InWebView, InCode, InUrl) =>
            {
                isWebViewShowing = false;
#if !UNITY_EDITOR && UNITY_IOS
                RegisterKeyboardNotification();
#endif
                _webView.Hide();
                // ReSharper disable once AccessToModifiedClosure
                _webView.OnPageFinished -= onPageFinished;
                InInitCallback?.Invoke(true);
            };

            UniWebView.PageErrorReceivedDelegate onPageErrorReceived = null;
            onPageErrorReceived = (InView, InCode, InMessage) =>
            {
                Log.Error($"[WebViewBridge] UniWebView.OnPageErrorReceived : errorCode = {InCode}, msg = {InMessage}");
                _webView.Hide();
                _webView.OnPageErrorReceived -= onPageErrorReceived;
                UnityEngine.Object.DestroyImmediate(_webView);
                InInitCallback?.Invoke(false);
            };

            _webView.OnPageFinished += onPageFinished;

            _webView.OnPageErrorReceived += onPageErrorReceived;

            _webView.OnShouldClose += InView =>
            {
#if UNITY_ANDROID
                if (isWebViewShowing)
                {
                    CallJsFunction("goBack");
                }
                else
                {
                    ADA_Manager.PageManager.Instance.BackButtonAction();
                }
#endif
                return isWebViewShowing;
            };

        }

        private static void CallJsFunction(string InJsFunctionName)
        {
            string jsFunction = $"{InJsFunctionName}()";
            Log.Info($"[WebViewBridge] Call Js Function : {jsFunction}", ColorName.Olive);

            _webView.EvaluateJavaScript(jsFunction, InPayload =>
            {
                if (InPayload.resultCode == "0")
                {
                    Log.Info(
                        $"[WebViewBridge] {jsFunction} Callback : data = {(string.IsNullOrEmpty(InPayload.data) ? "null" : InPayload.data)}",
                        ColorName.Olive);
                }
                else
                {
                    Log.Error(
                        $"[WebViewBridge] {jsFunction} Callback error : resultCode = {InPayload.resultCode}, data = {(string.IsNullOrEmpty(InPayload.data) ? "null" : InPayload.data)}");
                }
            });
        }

        private static bool isWebViewLoading;
        public static void LoadWebView(string InPageCodeStr)
        {
            isWebViewLoading = true;
            
            
            
            
            
            string url = _webView.Url;
            if (string.IsNullOrEmpty(url) || !url.Contains(Define_China.Instance.webChannel))
            {
                
            }
            else
            {
                
            }
        }

        public static void ShowWebView(UniWebViewTransitionEdge InTransitionEdge = UniWebViewTransitionEdge.None)
        {
            Log.Info($"[WebViewBridge] show web view : {InTransitionEdge}");
            
            isWebViewShowing = true;
            
            if (InTransitionEdge == UniWebViewTransitionEdge.None)
            {
                _webView.Show();
                SetGameSceneCameraActive(false);
            }
            else
            {
                _webView.Show(false, InTransitionEdge, .4f, () => { SetGameSceneCameraActive(false);});
            }
            _webView.SetBackButtonEnabled(true);
        }

        public static void HideWebView(UniWebViewTransitionEdge InTransitionEdge = UniWebViewTransitionEdge.None)
        {
            Log.Info($"[WebViewBridge] hide web view : {InTransitionEdge}");
            
            SetGameSceneCameraActive(true);

            if (InTransitionEdge == UniWebViewTransitionEdge.None)
            {
                TimerTools.Instance.RegisterTrigger(.4f, () => { _webView.Hide(); },_webView);
            }
            else
            {
                _webView.Hide(false, InTransitionEdge, .4f, () => { Log.Info($"[WebViewBridge] hide web view finished.", ColorName.Olive);});
            }
            
            Szn.Framework.Audio.AudioManager.Instance.FadeTurnSound(false);
            isWebViewShowing = false;
        }

        public static void SetGameSceneCameraActive(bool InActive, bool InIsUiControl = true)
        {
            
        }
    }
}