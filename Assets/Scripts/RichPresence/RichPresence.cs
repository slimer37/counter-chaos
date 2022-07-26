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

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init() => SceneManager.sceneLoaded += (s, _) => UpdateAllRPC(s.buildIndex);

    static void UpdateAllRPC(int s)
    {
        if (SceneManager.GetSceneByBuildIndex(s).name == "Base") return;
        UpdateSteamRPC(s);
        UpdateDiscordRPC(s);
    }

    static void UpdateSteamRPC(int sceneIndex)
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

    static void UpdateDiscordRPC(int sceneIndex)
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
                Start = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            },
            Assets =
            {
                LargeImage = "icon_large",
                LargeText = "Counter Chaos"
            },
            Instance = playing
        };

        activityManager.UpdateActivity(activity, r =>
        {
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
        activityManager?.ClearActivity(r =>
        {
            if (r != Result.Ok)
                Debug.LogError("(Discord RPC) Clear activity failed: " + r);
        });
    }
}