using UnityEngine;

public class AdManager : MonoBehaviour
{
    [SerializeField] private LevelController level;

    private string appKey = "1f4681d85";

    private void Start()
    {
        IronSource.Agent.init(appKey);
        IronSourceRewardedVideoEvents.onAdOpenedEvent += RewardedVideoOnAdOpenedEvent;
        IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoOnAdClosedEvent;
        IronSourceRewardedVideoEvents.onAdAvailableEvent += RewardedVideoOnAdAvailable;
        IronSourceRewardedVideoEvents.onAdUnavailableEvent += RewardedVideoOnAdUnavailable;
        IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoOnAdShowFailedEvent;
        IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
        IronSourceRewardedVideoEvents.onAdClickedEvent += RewardedVideoOnAdClickedEvent;
    }
    #region Callbacks
    private void RewardedVideoOnAdAvailable(IronSourceAdInfo adInfo)
    {
        Debug.Log("Rewarded Video Ad Available: " + adInfo);
    }

    private void RewardedVideoOnAdUnavailable()
    {
        Debug.Log("Rewarded Video Ad Unavailable");
    }

    private void RewardedVideoOnAdOpenedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("Rewarded Video Ad Opened: " + adInfo);
    }

    private void RewardedVideoOnAdClosedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("Rewarded Video Ad Closed: " + adInfo);
    }

    private void RewardedVideoOnAdRewardedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
    {
        Debug.Log("Rewarded Video Ad Rewarded: " + placement + ", " + adInfo);
        level.DoubleReward();
    }

    private void RewardedVideoOnAdShowFailedEvent(IronSourceError error, IronSourceAdInfo adInfo)
    {
        Debug.LogError("Rewarded Video Ad Show Failed: " + error + ", " + adInfo);
    }

    private void RewardedVideoOnAdClickedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
    {
        Debug.Log("Rewarded Video Ad Clicked: " + placement + ", " + adInfo);
    }
    #endregion

    private void ShowRewardedAd()
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

    public void OnShowAdButtonClick()
    {
        ShowRewardedAd();
    }
}