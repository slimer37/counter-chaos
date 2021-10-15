using System;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace UI
{
    public class DisplayWebNotes : MonoBehaviour
    {
        [SerializeField, TextArea] string sourceLink;
        [SerializeField, TextArea] string beginTag;
        [SerializeField, TextArea] string endTag;
        [SerializeField] TextMeshProUGUI text;

        UnityWebRequest webRequest;

        public void Refresh()
        {
            webRequest?.Dispose();
            Start();
        }

        void Start()
        {
            webRequest = UnityWebRequest.Get(sourceLink);
            text.text = "<i>Loading content...</i>";
            
            webRequest.SendWebRequest().completed += _ => {
                if (webRequest.result == UnityWebRequest.Result.Success)
                    ProcessWebpageContents(webRequest.downloadHandler.text);
                else
                    text.text = $"<color=red>{webRequest.error}</color>";
                
                webRequest.Dispose();
                webRequest = null;
            };
        }

        void ProcessWebpageContents(string contents)
        {
            if (beginTag == "" || endTag == "")
                throw new ArgumentException("Beginning or ending tags are unspecified.");
            
            var invCulture = StringComparison.InvariantCulture;
            var start = contents.IndexOf(beginTag, invCulture) + beginTag.Length;
            var end = contents.IndexOf(endTag, start, invCulture);
            text.text = contents.Substring(start, end - start).Trim('\n');
        }
    }
}
