using System;

namespace Core
{
    public static class GameEvents
    {
        public static event Action<string> AchievementEarned;

        public static void EarnAchievement(string achievement) => AchievementEarned?.Invoke(achievement);
    }
}
