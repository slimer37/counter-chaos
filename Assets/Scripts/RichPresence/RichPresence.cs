using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Discord;
using Serialization;

public class RichPresence : MonoBehaviour
{
    const long ClientId = 917277682226593812;
    const int SteamId = 1835160;
    static Discord.Discord discordClient;
    static ActivityManager activityManager;
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Init()
    {
        SceneManager.sceneLoaded += (s, m) => UpdateDiscordRPC(s.buildIndex, m);
        
        InitDiscordRPCIfNeeded();
        UpdateDiscordRPC(0, 0);
    }

    static void InitDiscordRPCIfNeeded()
    {
        if (discordClient != null) return;
        discordClient = new Discord.Discord(ClientId, (ulong)CreateFlags.NoRequireDiscord);
        activityManager = discordClient.GetActivityManager();
        activityManager.RegisterSteam(SteamId);
    }

    static void UpdateDiscordRPC(int sceneIndex, LoadSceneMode m)
    {
        InitDiscordRPCIfNeeded();
        
        if (discordClient == null || m == LoadSceneMode.Additive) return;

        activityManager.ClearActivity(_ => { });

        var playing = sceneIndex != 0;
        var status = playing ? "Playing" : "In Main Menu";

        var save = playing ? SaveSystem.LoadedSave : null;

        var activity = new Activity
        {
            State = status,
            Details = playing ? $"{save.playerName}'s store: {save.money:C}" : "",
            Timestamps =
            {
                Start = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds
            },
            Assets =
            {
                LargeImage = "icon_large",
                LargeText = "Counter Chaos"
            },
            Instance = playing
        };
        
        activityManager.UpdateActivity(activity, r => print("Discord RPC: " + r));
    }

    void Update()
    {
        discordClient?.RunCallbacks();
    }

    void OnApplicationQuit()
    {
        activityManager?.ClearActivity(_ => { });
    }
}
