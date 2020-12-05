using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;


public class FeedModule : MonoSingleton<FeedModule>
{
	public delegate void OnImageCompleted(string imagePath);
	OnImageCompleted _onFacePhotoCompleted;
	OnImageCompleted _onFilterImageCompleted;
	OnImageCompleted _onProfileGalleryCompleted;
	OnImageCompleted _onProfileCameraCompleted;

	string _feedMessage;
	string _discoveryImagePath;
	List<string> _imagePathList = new List<string>();

	public delegate void OnFeedCompleted(string message, string discovertyImage, string[] images);
	OnFeedCompleted _onFeedCompleted;

#if UNITY_ANDROID && !UNITY_EDITOR
	private AndroidJavaObject activityContext = null;
	private AndroidJavaClass javaClass = null;
	private AndroidJavaObject javaClassInstance = null;
#elif UNITY_IOS && !UNITY_EDITOR
    static RectTransform staticPanel;
#endif

	public RectTransform iOSPanel;


#if UNITY_IOS && !UNITY_EDITOR

    private delegate void callback(int result);

    [DllImport("__Internal")]
    private static extern void IOSshowView(callback callback);

    [DllImport("__Internal")]
    private static extern void IOShideView(callback callback);

    [DllImport("__Internal")]
    private static extern void IOSProfile(callback callback);

    [DllImport("__Internal")]
    private static extern void IOSFacePhoto(callback callback);

    [DllImport("__Internal")]
    private static extern void IOSSendPhoto(callback callback, string url);

    [DllImport("__Internal")]
    private static extern void IOSFeed(callback callback, string url);

    [DllImport("__Internal")]
    private static extern void IOSProfileCamera(callback callback);

    [DllImport("__Internal")]
    private static extern void IOSSetTagURL(callback callback, string data);

    [DllImport("__Internal")]
    private static extern void IOSSetPeopleURL(callback callback, string data);

    [DllImport("__Internal")]
    private static extern void IOSSetToken(callback callback, string data);


    [AOT.MonoPInvokeCallback(typeof(callback))]
    static void closeiOSHandler(int result)
    {
        if (staticPanel==null)
        {
            Debug.Log("staticPanel is null");
        }
        else
        {
            staticPanel.gameObject.SetActive(false);
        }
    }

    public void CloseiOSViewTapped()
    {
        CloseiOSView((int result) =>
        {
            iOSPanel.gameObject.SetActive(false);
        });
    }

    public void CloseiOSView(System.Action<int> closeComplete)
    {
        onCloseiOSView = closeComplete;
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            IOShideView(closeiOSViewHandler);
        }
        else
            closeiOSViewHandler(0);
    }

    [AOT.MonoPInvokeCallback(typeof(callback))]
    static void closeiOSViewHandler(int result)
    {
        if (onCloseiOSView != null)
            onCloseiOSView(result);
        onCloseiOSView = null;
    }
    static System.Action<int> onCloseiOSView;

#endif

    private void Start()
	{
        // 		Debug.Log("FeedModule Start");

        iOSPanel.gameObject.SetActive(false);

        // #if UNITY_ANDROID && !UNITY_EDITOR
        // 		using (AndroidJavaClass activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        // 		{
        // 			activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        // 			Debug.LogFormat(" activityContext {0}", activityContext);
        // 		}

        // 		using (javaClass = new AndroidJavaClass("com.feed.plugin.BridgeCls"))
        // 		{
        // 			Debug.LogFormat(" javaClass {0}", javaClass);
        // 			if (javaClass != null)
        // 			{
        // 				javaClassInstance = javaClass.CallStatic<AndroidJavaObject>("instance");
        // 				Debug.LogFormat(" javaClassInstance {0}", javaClass);
        // 				javaClassInstance.Call("setContext", activityContext);
        // 			}
        // 		}
        // #endif
    }

	public void CallGalleryFeed(OnFeedCompleted OnCompleted)
	{
		Debug.Log("CallGalleryFeed");

		_onFeedCompleted = OnCompleted;

#if UNITY_ANDROID && !UNITY_EDITOR
		using (javaClass = new AndroidJavaClass("com.feed.plugin.BridgeCls"))
		{
			Debug.LogFormat(" javaClass {0}", javaClass);
			if (javaClass != null)
			{
				javaClassInstance = javaClass.CallStatic<AndroidJavaObject>("instance");
				Debug.LogFormat(" javaClassInstance {0}", javaClass);
				javaClassInstance.Call("startGalleryActivity", activityContext);
			}
		}
#elif UNITY_IOS && !UNITY_EDITOR
        iOSPanel.gameObject.SetActive(true);
        staticPanel = iOSPanel;

        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            IOSshowView(closeiOSHandler);
        }
#endif
    }

    public void CallFacePhoto(OnImageCompleted OnCompleted)
	{
		Debug.Log("CallFacePhoto");

		_onFacePhotoCompleted = OnCompleted;

#if UNITY_ANDROID && !UNITY_EDITOR
		using (javaClass = new AndroidJavaClass("com.feed.plugin.BridgeCls"))
		{
			if (javaClass != null)
			{
				javaClassInstance = javaClass.CallStatic<AndroidJavaObject>("instance");
				javaClassInstance.Call("startFacePhotoActivity", activityContext);
			}
		}
#elif UNITY_IOS && !UNITY_EDITOR
        iOSPanel.gameObject.SetActive(true);
        staticPanel = iOSPanel;

        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            IOSFacePhoto(closeiOSHandler);
        }
#endif
    }

    public void CallEditActivity(string filePath, OnImageCompleted OnCompleted)
	{
		Debug.LogFormat("CallEditActivity. path : {0}", filePath);

		_onFilterImageCompleted = OnCompleted;

#if UNITY_ANDROID && !UNITY_EDITOR
		using (javaClass = new AndroidJavaClass("com.feed.plugin.BridgeCls"))
		{
			if (javaClass != null)
			{
				javaClassInstance = javaClass.CallStatic<AndroidJavaObject>("instance");
				javaClassInstance.Call("startEditActivity", activityContext, filePath);	//"/storage/emulated/0/temp_photo.jpg");
			}
		}
#elif UNITY_IOS && !UNITY_EDITOR
        iOSPanel.gameObject.SetActive(true);
        staticPanel = iOSPanel;

        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            IOSSendPhoto(closeiOSHandler, filePath); //"sample.jpg");
        }
#endif
	}

	public void CallFeedActivity(string filePath, OnFeedCompleted OnCompleted)
	{
		Debug.LogFormat("CallFeedActivity. path : {0}", filePath);

		_onFeedCompleted = OnCompleted;

#if UNITY_ANDROID && !UNITY_EDITOR
		using (javaClass = new AndroidJavaClass("com.feed.plugin.BridgeCls"))
		{
			if (javaClass != null)
			{
				javaClassInstance = javaClass.CallStatic<AndroidJavaObject>("instance");
				javaClassInstance.Call("startFeedActivity", activityContext, filePath); //"/storage/emulated/0/temp_photo.jpg");
			}
		}
#elif UNITY_IOS && !UNITY_EDITOR
        iOSPanel.gameObject.SetActive(true);
        staticPanel = iOSPanel;

        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            IOSFeed(closeiOSHandler, filePath); //"sample.jpg");
        }
#endif
	}

	public void CallProfileGallery(OnImageCompleted OnCompleted)
	{
		// �Լ��� check startProfileGallleryActivity

		Debug.Log("CallProfileGallery");

		_onProfileGalleryCompleted = OnCompleted;

#if UNITY_ANDROID && !UNITY_EDITOR
		using (javaClass = new AndroidJavaClass("com.feed.plugin.BridgeCls"))
		{
			if (javaClass != null)
			{
				javaClassInstance = javaClass.CallStatic<AndroidJavaObject>("instance");
				javaClassInstance.Call("startProfileGallleryActivity", activityContext);
			}
		}
#elif UNITY_IOS && !UNITY_EDITOR
        iOSPanel.gameObject.SetActive(true);
        staticPanel = iOSPanel;

        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            IOSProfile(closeiOSHandler);
        }
#endif
	}

	public void CallProfileCamera(OnImageCompleted OnCompleted)
    {
        Debug.Log("CallProfileCamera");

		_onProfileCameraCompleted = OnCompleted;

#if UNITY_ANDROID && !UNITY_EDITOR
        using (javaClass = new AndroidJavaClass("com.feed.plugin.BridgeCls"))
        {
            if (javaClass != null)
            {
                javaClassInstance = javaClass.CallStatic<AndroidJavaObject>("instance");
                javaClassInstance.Call("startProfilePhotoActivity", activityContext);
            }
        }
#elif UNITY_IOS && !UNITY_EDITOR
        iOSPanel.gameObject.SetActive(true);
        staticPanel = iOSPanel;

        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            IOSProfileCamera(closeiOSHandler);
        }
#endif
	}


	void SetFilterImagePath(string strPath)
	{
		Debug.LogFormat("FeedModule.SetFilterImagePath : {0}", strPath);

		if (_onFilterImageCompleted != null)
		{
			_onFilterImageCompleted(strPath);
			_onFilterImageCompleted = null;
		}
	}

	void SetFacePhotoPath(string strPath)
	{
		Debug.LogFormat("FeedModule.SetFacePhotoPath : {0}", strPath);

		if (_onFacePhotoCompleted != null)
		{
			_onFacePhotoCompleted(strPath);
			_onFacePhotoCompleted = null;
		}
	}

	void SetProfilePath(string strPath)
	{
		Debug.LogFormat("FeedModule.SetProfilePath : {0}", strPath);

		if (_onProfileGalleryCompleted != null)
		{
			_onProfileGalleryCompleted(strPath);
			_onProfileGalleryCompleted = null;
		}

		if (_onProfileCameraCompleted != null)
		{
			_onProfileCameraCompleted(strPath);
			_onProfileCameraCompleted = null;
		}
	}

	// just for test
	void CallByFacePhoto(string message)
	{
		Debug.LogFormat("FeedModule.CallByFacePhoto : {0}", message);
	}

	void SetMessage(string message)
	{
		Debug.LogFormat("FeedModule.SetMessage : {0}", message);

		_feedMessage = message;
	}

	void AddImagePath(string strPath)
	{
		Debug.LogFormat("FeedModule.AddImagePath : {0}", strPath);

		_imagePathList.Clear();

		var images = strPath.Split('|');
		Debug.LogFormat("AddImagePath image count : [{0}]", images?.Length);
		for (int i = 0; i != images.Length; ++i)
		{
			Debug.LogFormat("AddImagePath image : [{0} : {1}]", i, images[i]);
			_imagePathList.Add(images[i]);
		}

		if (_imagePathList.Count == 1 && _imagePathList[0].Equals("BACK"))
		{
			if (_onFeedCompleted != null)
			{
				_onFeedCompleted(_feedMessage, _discoveryImagePath, _imagePathList.ToArray());
				_onFeedCompleted = null;
			}
		}
	}

	void AddDiscoveryImagePath(string strPath)
	{
		Debug.LogFormat("FeedModule.AddDiscoveryImagePath : {0}", strPath);

		_discoveryImagePath = strPath;
	}

	void Feed(string strPath)
	{
		Debug.LogFormat("FeedModule.Feed : {0}", strPath);

		if (_onFeedCompleted != null)
		{
			Debug.LogFormat("_onFeedCompleted. message : {0}, discovery : {1}, imageCount :{2}", _feedMessage, _discoveryImagePath, _imagePathList.Count);
			for (int i = 0; i != _imagePathList.Count; ++i)
			{
				Debug.LogFormat("image {0} [{1}]", i, _imagePathList[i]);
			}

			_onFeedCompleted(_feedMessage, _discoveryImagePath, _imagePathList.ToArray());
			_onFeedCompleted = null;
		}

		_feedMessage = "";
		_discoveryImagePath = "";
		_imagePathList.Clear();
	}


	public void Init(string token)
	{
        // SetToken(token);
        // SetPeopleURL(Define_China.Instance.searchPeople);
        // SetTagURL(Define_China.Instance.searchTag);
    }

    void SetTagURL(string url)
    {
        Debug.LogFormat("SetTagURL : {0}", url);

#if UNITY_ANDROID && !UNITY_EDITOR
        using (javaClass = new AndroidJavaClass("com.feed.plugin.BridgeCls"))
        {
            if (javaClass != null)
            {
                javaClassInstance = javaClass.CallStatic<AndroidJavaObject>("instance");
                javaClassInstance.Call("setRequestTagURL", url);
            }
        }
#elif UNITY_IOS && !UNITY_EDITOR
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            IOSSetTagURL(closeiOSHandler, url);
        }
#endif
    }

    void SetPeopleURL(string url)
    {
        Debug.LogFormat("SetPeopleURL : {0}", url);

#if UNITY_ANDROID && !UNITY_EDITOR
        using (javaClass = new AndroidJavaClass("com.feed.plugin.BridgeCls"))
        {
            if (javaClass != null)
            {
                javaClassInstance = javaClass.CallStatic<AndroidJavaObject>("instance");
                javaClassInstance.Call("setRequestPeopleURL", url);
            }
        }
#elif UNITY_IOS && !UNITY_EDITOR
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            IOSSetPeopleURL(closeiOSHandler, url);
        }
#endif
    }

    void SetToken(string token)
    {
        Debug.LogFormat("SetToken : {0}", token);

#if UNITY_ANDROID && !UNITY_EDITOR
        using (javaClass = new AndroidJavaClass("com.feed.plugin.BridgeCls"))
        {
            if (javaClass != null)
            {
                javaClassInstance = javaClass.CallStatic<AndroidJavaObject>("instance");
                javaClassInstance.Call("setRequestToken", token);
            }
        }
#elif UNITY_IOS && !UNITY_EDITOR
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            IOSSetToken(closeiOSHandler, token);
        }
#endif
    }


    /// test function
    /// 

    public void Test(int index)
	{
		switch (index)
		{
			case 1:
				{
					CallGalleryFeed((msg, discovery, images) => { Debug.LogFormat("CallGalleryFeed callback : msg {0}, discovery {1}, images {2}", msg, discovery, images); });
				}
				break;
			case 2:
				{
					CallFacePhoto((test) => { Debug.LogFormat("test CallFacePhoto callback : {0}", test); });
				}
				break;
			case 3:
				{
					// com.fit.ada/files/a.jpg;
					string filePath = Application.persistentDataPath + "/a.jpg";
					CallEditActivity(filePath, (test) => { Debug.LogFormat("test CallEditActivity callback : {0}", test); });
				}
				break;
			case 4:
				{
					string filePath = Application.persistentDataPath + "/a.jpg";
					CallFeedActivity(filePath, (msg, discovery, images) => { Debug.LogFormat("CallFeedActivity callback : msg {0}, discovery {1}, images {2}", msg, discovery, images); });
				}
				break;
			case 5:
				{
					CallProfileGallery((test) => { Debug.LogFormat("test CallProfileGallery callback : {0}", test); });
				}
				break;
			case 6:
				{
					CallProfileCamera((test) => { Debug.LogFormat("test CallProfileCamera callback : {0}", test); });
				}
				break;
		}
	}
}
