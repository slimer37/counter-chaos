using System.Text.RegularExpressions;
using Core;
using UnityEngine;

namespace UI
{
    public class Link : MonoBehaviour
    {
        [SerializeField] string destinationURL;
        [SerializeField] string promptTitle = "Visit Link";
        [SerializeField, RequireSubstring("{0}")] string promptMessage = "Open {0} in the default browser?";
        
        public void VisitLink()
        {
            Dialog.Instance.YesNo(promptTitle, string.Format(Regex.Unescape(promptMessage), destinationURL), yes => {
                if (yes) Application.OpenURL(destinationURL);
            });
        }
    }
}
