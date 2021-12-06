using UnityEngine;
using UnityEngine.SceneManagement;
using Steamworks;

public static class SteamHelper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Init()
    {
        UpdateRichPresence(0, 0);
        SceneManager.sceneLoaded += (s, m) => UpdateRichPresence(s.buildIndex, m);
    }

    static void UpdateRichPresence(int sceneIndex, LoadSceneMode m)
    {
        if (!SteamManager.Initialized || m == LoadSceneMode.Additive) return;
        SteamFriends.ClearRichPresence();
        var status = sceneIndex == 0 ? "MainMenu" : "Playing";
        SteamFriends.SetRichPresence("steam_display", $"#Status_{status}");
    }
}