using System;

namespace Achievements
{
    // Enum for achievements set in Steamworks.
    // ReSharper disable InconsistentNaming
    public enum APIName
    {
        ACH_GET_LOAN
    }
    
    public static class AchievementManager
    {
        public static event Action<APIName> EarnAchievement;
        public static event Action<APIName, int> ProgressStat;

        public static void OnEarnAchievement(APIName achievement) => EarnAchievement?.Invoke(achievement);
        public static void OnProgressStat(APIName stat, int data) => ProgressStat?.Invoke(stat, data);
    }
}
