using System;
using Achievements;
using Serialization;
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
        if (SaveSystem.LoadedSave.IsCompromised)
        {
            Debug.LogWarning("This save file has been modified. Achievements are disabled.");
            return;
        }
        
        SteamUserStats.SetAchievement(pchAchievement.ToString());
    }

    static void OnSetStat(APIName pchStat, int data)
    {
        if (SaveSystem.LoadedSave.IsCompromised)
        {
            Debug.LogWarning("This save file has been modified. Stats are disabled.");
            return;
        }
        
        if (!SteamUserStats.RequestCurrentStats())
            throw new Exception("Failed to get current stats. [Not logged in.]");

        SteamUserStats.SetStat(pchStat.ToString(), data);
    }
}