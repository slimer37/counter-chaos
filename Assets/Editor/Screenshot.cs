using UnityEditor;
using UnityEngine;

namespace Project.Editor
{
    public static class ScreenshotGrabber
    {
        static string filePath;
        const string PathKey = "screenshotPath";
        
        [MenuItem("Screenshot/Grab")]
        public static void Grab()
        {
            var defaultPath = EditorPrefs.GetString(PathKey, "C:/Screenshot.png");
            var lastSlash = defaultPath.LastIndexOf('/');
            var name = defaultPath.Substring(lastSlash + 1);
            var dir = defaultPath.Substring(0, lastSlash);
            filePath = EditorUtility.SaveFilePanel(
                "Save Screenshot", 
                dir, 
                name,
                "png");

            if (filePath.Length == 0) return;
            
            ScreenCapture.CaptureScreenshot(filePath, 1);
            EditorPrefs.SetString(PathKey, filePath);
        }
    }
}
