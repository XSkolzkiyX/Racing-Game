using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IronSourceJSON;

public class AdManager : MonoBehaviour
{
    /*
    private string appKey = "";
    
    void Start()
    {
        IronSource.Agent.init(appKey);
        IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent;
        IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosedEvent;
    }

    void OnDestroy()
    {
        IronSourceEvents.onRewardedVideoAdRewardedEvent -= RewardedVideoAdRewardedEvent;
        IronSourceEvents.onRewardedVideoAdClosedEvent -= RewardedVideoAdClosedEvent;
    }
    
    public void ShowRewardedAd()
    {
        if (IronSource.Agent.isRewardedVideoAvailable())
        {
            IronSource.Agent.showRewardedVideo();
        }
        else
        {
            Debug.Log("Rewarded video not available");
        }
    }
    
    private void RewardedVideoAdRewardedEvent(IronSourcePlacement placement)
    {
        Debug.Log("Rewarded Video Ad Rewarded");
        int rewardAmount = placement.getRewardAmount();
        string rewardName = placement.getRewardName();
        Debug.Log("Reward: " + rewardAmount + " " + rewardName);
    }
    
    private void RewardedVideoAdClosedEvent()
    {
        Debug.Log("Rewarded Video Ad Closed");
    }*/
}