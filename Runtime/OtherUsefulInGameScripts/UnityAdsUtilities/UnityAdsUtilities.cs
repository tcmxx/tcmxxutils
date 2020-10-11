using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Advertisements;
#if UNITY_ADS
public static class UnityAdsUtilities
{
    public static string AndroidAdsGameID = "3324414";  //change this
    public static string IOSAdsGameID = "3324415";      //change this
    public static async UniTask<ShowResult> ShowRewardAdsAsync(string placementID = null)
    {
        ShowOptions option = new ShowOptions();

        ShowResult result = ShowResult.Failed;
        bool showDone = false;
        option.resultCallback += (x) => { result = x; showDone = true; };

        if (string.IsNullOrEmpty(placementID))
            Advertisement.Show(option);
        else
            Advertisement.Show(placementID, option);

        await UniTask.WaitUntil(() => { return showDone; });
        return result;
    }

    public static UniTask InitializeAndGetAdsReadyAsync()
    {
#if UNITY_ANDROID
        Advertisement.Initialize(AndroidAdsGameID);
        Debug.Log("Initialize Android Ads");
#elif UNITY_IOS
				Advertisement.Initialize (IOSAdsGameID);
				Debug.Log ("IOS Ads");
#else
				Advertisement.Initialize (AndroidAdsGameID,true);
				Debug.Log ("PC Ads Test");
#endif
        return UniTask.WaitUntil(() => { return Advertisement.isInitialized && Advertisement.IsReady(); });
    }
}
#endif