using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdManager : MonoBehaviour
{
    //private string appKey = "your_app_key_here";
    //
    //void Start()
    //{
    //    IronSource.Agent.init(appKey);
    //    IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent;
    //    IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosedEvent;
    //}
    //
    //void OnDestroy()
    //{
    //    IronSourceEvents.onRewardedVideoAdRewardedEvent -= RewardedVideoAdRewardedEvent;
    //    IronSourceEvents.onRewardedVideoAdClosedEvent -= RewardedVideoAdClosedEvent;
    //}
    //
    //public void ShowRewardedAd()
    //{
    //    if (IronSource.Agent.isRewardedVideoAvailable())
    //    {
    //        IronSource.Agent.showRewardedVideo();
    //    }
    //    else
    //    {
    //        Debug.Log("Rewarded video not available");
    //    }
    //}
    //
    //private void RewardedVideoAdRewardedEvent(IronSourcePlacement placement)
    //{
    //    Debug.Log("Rewarded Video Ad Rewarded");
    //    // Добавьте код для вознаграждения пользователя здесь
    //    int rewardAmount = placement.getRewardAmount();
    //    string rewardName = placement.getRewardName();
    //    Debug.Log("Reward: " + rewardAmount + " " + rewardName);
    //    // Например, добавление валюты
    //    // GameManager.instance.AddCurrency(rewardAmount);
    //}
    //
    //private void RewardedVideoAdClosedEvent()
    //{
    //    Debug.Log("Rewarded Video Ad Closed");
    //    // Добавьте код для обработки закрытия рекламы здесь, если необходимо
    //}
}