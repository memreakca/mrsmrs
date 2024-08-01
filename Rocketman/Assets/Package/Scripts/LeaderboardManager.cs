using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{

    [SerializeField] private string leaderboardName;

    [SerializeField] private Transform rowParent;

    [SerializeField] private List<GameObject> firstThreeRowPrefab;

    [SerializeField] private GameObject rowPrefab;

    [SerializeField] private LeaderboardRow playerLeaderboardRow;


    private void OnEnable()
    {
        GetAndDisplayDailyLeaderboard();
        if (!SaveManager.Instance.isGuest)
        {

            playerLeaderboardRow.gameObject.SetActive(true);

            GetAndDisplayDailyLeaderboardAroundPlayer();
        }
        else
        {
            playerLeaderboardRow.gameObject.SetActive(false);
        }
    }

    public void GetAndDisplayDailyLeaderboard()
    {
        PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest
        {
            StatisticName = leaderboardName,
            StartPosition = 0,
            MaxResultsCount = 13,
        }, result => DisplayLeaderboard(result), FailureCallback);

    }


    public void GetAndDisplayDailyLeaderboardAroundPlayer()
    {
        PlayFabClientAPI.GetLeaderboardAroundPlayer(new GetLeaderboardAroundPlayerRequest
        {
            StatisticName = leaderboardName,
            PlayFabId = SaveManager.Instance.PlayfabId,
            MaxResultsCount = 1,
        }, result => DisplayCoinLeaderboardAroundPlayer(result), FailureCallback);

    }
    public void DisplayCoinLeaderboardAroundPlayer(GetLeaderboardAroundPlayerResult result)
    {
        for (int i = 0; i < result.Leaderboard.Count; i++)
        {
            playerLeaderboardRow.rankText.text = (result.Leaderboard[i].Position + 1).ToString();
            playerLeaderboardRow.nameText.text = result.Leaderboard[i].DisplayName;
            playerLeaderboardRow.scoreText.text = result.Leaderboard[i].StatValue.ToString();
        }
    }
    public void DisplayLeaderboard(GetLeaderboardResult result)
    {
        foreach (Transform item in rowParent)
        {
            Destroy(item.gameObject);
        }

        for (int i = 0; i < result.Leaderboard.Count; i++)
        {
            if (i < 3)
            {
                GameObject firstThreeGo = Instantiate(firstThreeRowPrefab[i], rowParent);
                LeaderboardRow firstThreeObj = firstThreeGo.GetComponent<LeaderboardRow>();

                firstThreeObj.rankText.text = (result.Leaderboard[i].Position + 1).ToString();
                firstThreeObj.nameText.text = result.Leaderboard[i].DisplayName;
                firstThreeObj.scoreText.text = result.Leaderboard[i].StatValue.ToString();
            }
            else
            {
                GameObject row = Instantiate(rowPrefab, rowParent);
                LeaderboardRow rowObj = row.GetComponent<LeaderboardRow>();


                rowObj.rankText.text = (result.Leaderboard[i].Position + 1).ToString();
                rowObj.nameText.text = result.Leaderboard[i].DisplayName;
                rowObj.scoreText.text = result.Leaderboard[i].StatValue.ToString();

            }
        }
    }


    private void FailureCallback(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong with your API call. Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }



}
