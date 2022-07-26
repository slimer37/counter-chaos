using Core;
using UnityEngine;
using Steamworks;

public static class SteamHelper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        Object.DontDestroyOnLoad(new GameObject("Steam Manager").AddComponent<SteamManager>());
        
        GameEvents.AchievementEarned += EarnAchievement;
    }
    
    static void EarnAchievement(string achievement)
    {
        SteamUserStats.SetAchievement(achievement);
    }
}