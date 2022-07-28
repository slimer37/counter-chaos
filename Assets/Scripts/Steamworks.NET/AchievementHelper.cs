using System;
using Achievements;
using UnityEngine;
using Steamworks;
using Object = UnityEngine.Object;

public static class AchievementHelper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        Object.DontDestroyOnLoad(new GameObject("Steam Manager").AddComponent<SteamManager>());
        
        AchievementManager.EarnAchievement += OnEarnAchievement;
        AchievementManager.ProgressStat += OnSetStat;
    }
    
    static void OnEarnAchievement(APIName pchAchievement)
    {
        SteamUserStats.SetAchievement(pchAchievement.ToString());
    }

    static void OnSetStat(APIName pchStat, int data)
    {
        if (!SteamUserStats.RequestCurrentStats())
            throw new Exception("Failed to get current stats. [Not logged in.]");

        SteamUserStats.SetStat(pchStat.ToString(), data);
    }
}