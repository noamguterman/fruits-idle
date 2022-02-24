using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity.Example;
using Facebook.Unity;

public class FBControl : MonoBehaviour
{
    private void Start()
    {
        FB.Init(this.OnInitComplete);
    }

    //private void Update()
    //{
    //    Cursor.visible = false;
    //}
    private void CallFBLogin()
    {
        FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email", "user_friends" });
    }

    private void CallFBLoginForPublish()
    {
        // It is generally good behavior to split asking for read and publish
        // permissions rather than ask for them all at once.
        //
        // In your own game, consider postponing this call until the moment
        // you actually need it.
        FB.LogInWithPublishPermissions(new List<string>() { "publish_actions" });
    }

    private void CallFBLogout()
    {
        FB.LogOut();
    }

    private void OnInitComplete()
    {
        string logMessage = string.Format(
            "OnInitCompleteCalled IsLoggedIn='{0}' IsInitialized='{1}'",
            FB.IsLoggedIn,
            FB.IsInitialized);
        LogView.AddLog(logMessage);
        if (AccessToken.CurrentAccessToken != null)
        {
            LogView.AddLog(AccessToken.CurrentAccessToken.ToString());
        }

        FB.ActivateApp();

        FB.LogAppEvent("GameStart");
    }

    //private void OnHideUnity(bool isGameShown)
    //{
    //    this.Status = "Success - Check log for details";
    //    this.LastResponse = string.Format("Success Response: OnHideUnity Called {0}\n", isGameShown);
    //    LogView.AddLog("Is game shown: " + isGameShown);
    //}
}
