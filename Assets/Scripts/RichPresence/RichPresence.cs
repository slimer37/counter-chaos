using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Serialization;
using Discord;
using Steamworks;

public class RichPresence : MonoBehaviour
{
    const long ClientId = 917277682226593812;
    const int SteamId = 1835160;
    static Discord.Discord discordClient;
    static ActivityManager activityManager;
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Init()
    {
        SceneManager.sceneLoaded += (s, m) => UpdateAllRPC(s.buildIndex, m);
        
        UpdateAllRPC(0, 0);
    }

    static void UpdateAllRPC(int s, LoadSceneMode m)
    {
        if (m == LoadSceneMode.Additive) return;
        UpdateSteamRPC(s, m);
        UpdateDiscordRPC(s, m);
    }
    
    static void UpdateSteamRPC(int sceneIndex, LoadSceneMode m)
    {
        if (!SteamManager.Initialized) return;
        SteamFriends.ClearRichPresence();
        var status = sceneIndex == 0 ? "MainMenu" : "Playing";
        SteamFriends.SetRichPresence("steam_display", $"#Status_{status}");
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
        
        if (discordClient == null) return;

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
                Start = (int)DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds
            },
            Assets =
            {
                LargeImage = "icon_large",
                LargeText = "Counter Chaos"
            },
            Instance = playing
        };
        
        activityManager.UpdateActivity(activity, r => {
            if (r != Result.Ok)
                Debug.LogError("(Discord RPC) Update activity failed: " + r);
        });
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
