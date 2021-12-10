using Core;
using UnityEngine;
using Steamworks;

public static class SteamHelper
{
    [RuntimeInitializeOnLoadMethod]
    static void Init()
    {
        GameEvents.AchievementEarned += EarnAchievement;
    }
    
    static void EarnAchievement(string achievement)
    {
        SteamUserStats.SetAchievement(achievement);
    }
}