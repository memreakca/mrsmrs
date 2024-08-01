using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class SignInCanvasManager : MonoBehaviour
{
    Regex walletRegex = new Regex("^[a-zA-Z0-9]+$");

    [SerializeField] private TMP_InputField walletText;
    public void SignIn()
    {
        if (walletText.text != null && walletRegex.Match(walletText.text).Success)
        {
            LoginRequest();
        }
    }
    public void PlayGuest()
    {
        SaveManager.Instance.isGuest = true;
        gameObject.SetActive(false);
    }

    public void LoginRequest()
    {
        Debug.Log("loginId:" + SystemInfo.deviceUniqueIdentifier);
        var request = new LoginWithCustomIDRequest
        {
            CreateAccount = true,
            CustomId = walletText.text,
            InfoRequestParameters = new PlayFab.ClientModels.GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true,
                GetPlayerStatistics = true
            }
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnError);
    }
    public void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login succesfull " + result.PlayFabId);
        SaveManager.Instance.isGuest = false;
        SaveManager.Instance.PlayfabId = result.PlayFabId;
        gameObject.SetActive(false);
    }
    public void OnError(PlayFabError error)
    {
        Debug.Log(error.ErrorMessage);
    }
}
