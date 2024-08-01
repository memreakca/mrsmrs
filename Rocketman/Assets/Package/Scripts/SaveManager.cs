using PlayFab;
using PlayFab.ServerModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    public string PlayfabId;

    public bool isGuest;

    public string leaderboardId;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(this);
    }


    public void SubmitScore(float score)
    {
        if (isGuest)
            return;

        PlayFabServerAPI.UpdatePlayerStatistics(new PlayFab.ServerModels.UpdatePlayerStatisticsRequest
        {
            PlayFabId = PlayfabId,

            Statistics = new List<StatisticUpdate> {
            new StatisticUpdate {
                StatisticName = leaderboardId,
                Value =(int) score
            }
        }
        }, result => OnStatisticsUpdated(result), FailureCallback);
    }

    private void OnStatisticsUpdated(PlayFab.ServerModels.UpdatePlayerStatisticsResult updateResult)
    {
        Debug.Log("Successfully submitted high score");
    }
    private void FailureCallback(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong with your API call. Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }

}
