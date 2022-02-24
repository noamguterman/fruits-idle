using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using UnityEngine.Monetization;
using UnityEngine.Advertisements;
using ShowResult = UnityEngine.Monetization.ShowResult;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance;
    public static string targetStr = "";

    public static event System.Action<bool> RewardedClosed;
    //public static event System.Action<bool> InterstitialClosed;

    #region UnityAds
    string placementId_video = "video";
    string placementId_rewardedvideo = "rewardedVideo";
    private void Awake()
    {
        Instance = this;

        //Initialize Unity
#if UNITY_ANDROID
        Monetization.Initialize("3267697", false);
#elif UNITY_IOS
        Monetization.Initialize("3267696", false);
#endif
    }

    private void Start()
    {
        //StartCoroutine(ShowBannerWhenReady());
    }

    //IEnumerator ShowBannerWhenReady()
    //{
    //    while (!Advertisement.IsReady("bannerPlacement"))
    //    {
    //        Debug.Log("Banner not ready");
    //        yield return new WaitForSeconds(0.5f);
    //    }
    //    Debug.Log("Showing Banner");
    //    Advertisement.Banner.Show("bannerPlacement");
    //    Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
    //}

    public void ShowInterstitial()
    {
        Debug.Log("Showing interstitial ad");
#if UNITY_ANDROID
        StartCoroutine(WaitForAd());
#elif UNITY_IOS
        StartCoroutine(WaitForAd());
#endif

    }

    public void ShowRewardedVideo()
    {
        Debug.Log("Show Rewarded ad");
        StartCoroutine(WaitForAd(true));
    }

    IEnumerator WaitForAd(bool rewarded = false)
    {
        string placementId = rewarded ? placementId_rewardedvideo : placementId_video;
        while (!Monetization.IsReady(placementId))
        {
            yield return null;
        }

        ShowAdPlacementContent ad = null;
        ad = Monetization.GetPlacementContent(placementId) as ShowAdPlacementContent;

        if (ad != null)
        {
            if (rewarded == true)
                ad.Show(OnResultRewarded);
            else
                ad.Show(OnResultInterstitial);
        }
    }

    private void OnResultInterstitial(ShowResult result)
    {
        Debug.Log("InterstitialResult ::::::" + result);
        //InterstitialClosed(true);
    }

    private void OnResultRewarded(ShowResult result)
    {
        Debug.Log("RewardedResult ::::::" + result);
        if (result == ShowResult.Finished)
            RewardedClosed(true);
        else
        {
            RewardedClosed(false);
        }
    }
#endregion
}
