using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PromoteHTManager : MonoBehaviour
{
    public static PromoteHTManager Instances;

    public Dictionary<int, PromoteHTManager.BackModelType> _backModelList = new Dictionary<int, PromoteHTManager.BackModelType>();

    public Dictionary<int, PromoteHTManager.BorderModelType> _borderModelList = new Dictionary<int, PromoteHTManager.BorderModelType>();

    public Dictionary<int, PromoteHTManager.IconModeType> _iconModeList = new Dictionary<int, PromoteHTManager.IconModeType>();

    public Dictionary<int, PromoteHTManager.ContentModelType> _contentModelList = new Dictionary<int, PromoteHTManager.ContentModelType>();

    public Animator BackModelsAnimation;

    public Animator BorderModelsAnimation;

    public Animator IconModelsAnimation;

    public Animator ContentModelsAnimation;

    public PromoteHTManager.BackModelType mBackModel;

    public PromoteHTManager.BorderModelType mBorderModel;

    public PromoteHTManager.IconModeType mIconMode;

    public PromoteHTManager.ContentModelType mContentModel;

    public bool BackModelSwitch;

    public bool BorderModelSwitch;

    public bool IconModeSwitch;

    public bool ContentModelSwitch;

    public string IconDownLoadUrl;

    public string loadingIconPathName = "icon.jpg";

    public Image RootImg;

    public Image BackImg;

    public Image BordertImg;

    public Image IconImg;

    public Text ContentText;

    private FileInfo File;

    private Texture2D texture2D;

    public float mRootX;

    public float mRootY;

    public float mBackModelX;

    public float mBackModelWidth;

    public float mBackModelHight;

    public float mBorderModelX;

    public float mBorderModelWidth;

    public float mBorderModelHight;

    public float mIconModeX;

    public float mIconModeWidth;

    public float mIconModeHight;

    public float mContentModelX;

    public float mContentModelWidth;

    public float mContentModelHight;

    public enum BackModelType
    {
        VibrationModel,
        StaticMobil
    }

    public enum BorderModelType
    {
        WavesModel,
        StaticMobil
    }

    public enum IconModeType
    {
        BeatUpMobile,
        MagnifyMobileMobile,
        StaticMobile
    }

    public enum ContentModelType
    {
        FadeFade_InModel,
        ScrollBroadcastModel,
        DisappearInModel,
        StaticMobil
    }

    public void Awake()
	{
		Instances = this;
	}

	public void OpenDistributionAnimation()
	{
		gameObject.transform.GetChild(0).gameObject.SetActive(true);
		if (BackModelSwitch)
		{
			using (Dictionary<int, BackModelType>.Enumerator enumerator = _backModelList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<int, BackModelType> keyValuePair = enumerator.Current;
					if (mBackModel == keyValuePair.Value)
					{
						string name = "PromoteHT-Pattern_" + keyValuePair.Key.ToString();
						BackModelsAnimation.SetBool(name, true);
					}
					else
					{
						string name2 = "PromoteHT-Pattern_" + keyValuePair.Key.ToString();
						BackModelsAnimation.SetBool(name2, false);
					}
				}
				goto IL_C3;
			}
		}
		BackModelsAnimation.enabled = false;
		IL_C3:
		if (BorderModelSwitch)
		{
			using (Dictionary<int, BorderModelType>.Enumerator enumerator2 = _borderModelList.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					KeyValuePair<int, BorderModelType> keyValuePair2 = enumerator2.Current;
					if (mBorderModel == BorderModelType.WavesModel)
					{
						string name3 = "Circle-Pattern_1";
						BorderModelsAnimation.SetBool(name3, true);
					}
					else if (mBorderModel == BorderModelType.StaticMobil)
					{
						string name4 = "Circle-Pattern_1";
						BorderModelsAnimation.SetBool(name4, false);
					}
				}
				goto IL_144;
			}
		}
		this.BorderModelsAnimation.enabled = false;
		IL_144:
		if (this.IconModeSwitch)
		{
			using (Dictionary<int, IconModeType>.Enumerator enumerator3 = this._iconModeList.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					KeyValuePair<int, IconModeType> keyValuePair3 = enumerator3.Current;
					if (mIconMode == keyValuePair3.Value)
					{
						string name5 = "Icon-Pattern_" + keyValuePair3.Key.ToString();
						IconModelsAnimation.SetBool(name5, true);
					}
					else
					{
						string name6 = "Icon-Pattern_" + keyValuePair3.Key.ToString();
						IconModelsAnimation.SetBool(name6, false);
					}
				}
				goto IL_1EF;
			}
		}
		IconModelsAnimation.enabled = false;
		IL_1EF:
		if (ContentModelSwitch)
		{
			using (Dictionary<int, ContentModelType>.Enumerator enumerator4 = _contentModelList.GetEnumerator())
			{
				while (enumerator4.MoveNext())
				{
					KeyValuePair<int, ContentModelType> keyValuePair4 = enumerator4.Current;
					if (mContentModel == keyValuePair4.Value)
					{
						string name7 = "Content-Pattern_" + keyValuePair4.Key.ToString();
						ContentModelsAnimation.SetBool(name7, true);
					}
					else
					{
						string name8 = "Content-Pattern_" + keyValuePair4.Key.ToString();
						ContentModelsAnimation.SetBool(name8, false);
					}
				}
				return;
			}
		}
		this.ContentModelsAnimation.enabled = false;
	}

	public void ParsingCallback(string _str)
	{
		string[] array = _str.Split(new char[]
		{
			'&'
		});
		InItGame(array[0], array[1]);
	}

	public void InItGame(string iconUrl, string context)
	{
		Instances = this;
		if (this.IsNetWork())
		{
			_backModelList.Add(1, BackModelType.VibrationModel);
			_backModelList.Add(2, BackModelType.StaticMobil);
			_borderModelList.Add(1, BorderModelType.WavesModel);
			_borderModelList.Add(2, BorderModelType.StaticMobil);
			_iconModeList.Add(1, IconModeType.BeatUpMobile);
			_iconModeList.Add(2, IconModeType.MagnifyMobileMobile);
			_iconModeList.Add(3, IconModeType.StaticMobile);
			_contentModelList.Add(1, ContentModelType.FadeFade_InModel);
			_contentModelList.Add(2, ContentModelType.ScrollBroadcastModel);
			_contentModelList.Add(3, ContentModelType.DisappearInModel);
			_contentModelList.Add(4, ContentModelType.StaticMobil);
			DownLoadImage(iconUrl, context);
			return;
		}
		Debug.Log("没有网络");
	}

	public void DownLoadImage(string url, string context)
	{
		File = new FileInfo(RootPath + loadingIconPathName);
		Debug.Log(RootPath + loadingIconPathName);
		IconDownLoadUrl = url;
		StartCoroutine(DownFile(IconDownLoadUrl, context));
	}

	private static string RootPath
	{
		get
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
			{
				return Application.persistentDataPath + "/";
			}
			if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
			{
				return Application.dataPath.Replace("Assets", "");
			}
			return Application.dataPath + "/";
		}
	}

	public IEnumerator DownFile(string url, string Context)
	{
		WWW www = new WWW(url);
		yield return www;
		if (www.isDone)
		{
			byte[] bytes = www.bytes;
			texture2D = www.texture;
			Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0f, 0f));
			IconImg.sprite = sprite;
			ContentText.text = Context;
			OpenDistributionAnimation();
			Debug.Log("加载完成");
		}
		yield break;
	}

	public void CreatFile(byte[] bytes)
	{
		FileStream fileStream = File.Create();
		fileStream.Write(bytes, 0, bytes.Length);
		fileStream.Close();
		fileStream.Dispose();
	}

	public bool IsNetWork()
	{
		if (Application.internetReachability == NetworkReachability.NotReachable || Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
		{
			Debug.Log("No Network");
			return false;
		}
		return true;
	}
}
