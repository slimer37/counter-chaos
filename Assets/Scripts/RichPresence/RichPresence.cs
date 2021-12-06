using UnityEngine;
using UnityEngine.SceneManagement;
using Discord;

public class RichPresence : MonoBehaviour
{
    const long ClientId = 917277682226593812;
    const int SteamId = 1835160;
    static Discord.Discord discordClient;
    static ActivityManager activityManager;
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Init()
    {
        discordClient = new Discord.Discord(ClientId, (ulong)CreateFlags.NoRequireDiscord);
        activityManager = discordClient.GetActivityManager();
        activityManager.RegisterSteam(SteamId);
        UpdateRichPresence(0, 0);
        SceneManager.sceneLoaded += (s, m) => UpdateRichPresence(s.buildIndex, m);
    }

    static void UpdateRichPresence(int sceneIndex, LoadSceneMode m)
    {
        if (m == LoadSceneMode.Additive) return;

        activityManager.ClearActivity(_ => { });

        var status = sceneIndex == 0 ? "In Main Menu" : "Playing";

        var activity = new Activity
        {
            State = status,
        };
        
        activityManager.UpdateActivity(activity, _ => { });
    }

    void Update()
    {
        discordClient.RunCallbacks();
    }

    void OnDestroy()
    {
        activityManager.ClearActivity(_ => { });
    }
}
