using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using Core;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace UI
{
    public class DisplayWebNotes : MonoBehaviour
    {
        [SerializeField] string rssFeedSource;
        [SerializeField] string loadingMessage;
        [Header("Title")]
        [SerializeField] string beginTitleTag;
        [SerializeField] string endTitleTag;
        [SerializeField, RequireSubstring(true, "{0}", "{1}")] string headerFormat;
        [SerializeField] string dateFormatSpecifier;
        [Header("Content Tags")]
        [SerializeField] string beginTag;
        [SerializeField] string endTag;
        [SerializeField] string splitter;
        [Header("TMP")]
        [SerializeField] TextMeshProUGUI text;

        void Awake()
        {
            splitter = Regex.Unescape(splitter);
            headerFormat = Regex.Unescape(headerFormat);
        }

        public void Refresh() => Start();
        
        void Start()
        {
            StopAllCoroutines();
            StartCoroutine(ProcessPosts());
        }

        IEnumerator ProcessPosts()
        {
            if (beginTag == "" || endTag == "")
                throw new ArgumentException("Beginning or ending tags are unspecified.");
            
            text.text = loadingMessage;
            
            var links = new List<string>();
            var dates = new List<DateTime>();

            var feed = UnityWebRequest.Get(rssFeedSource);
            yield return feed.SendWebRequest();
            ShowErrorIfNeeded(feed);
            
            using var strReader = new StringReader(feed.downloadHandler.text);
            {
                var settings = new XmlReaderSettings {Async = true};
                var reader = XmlReader.Create(strReader, settings);
                var foundItem = false;
                
                var onLink = false;
                var onDate = false;

                var readTask = reader.ReadAsync();
                yield return new WaitUntil(() => readTask.IsCompleted);

                while (readTask.Result)
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (!foundItem && reader.Name == "item") foundItem = true;
                            if (foundItem && reader.Name == "link") onLink = true;
                            if (foundItem && reader.Name == "pubDate") onDate = true;
                            break;
                        case XmlNodeType.Text:
                            if (onLink)
                            {
                                links.Add(reader.Value);
                                onLink = false;
                            }
                            else if (onDate)
                            {
                                dates.Add(DateTime.Parse(reader.Value));
                                onDate = false;
                            }
                            break;
                    }

                    readTask = reader.ReadAsync();
                    yield return new WaitUntil(() => readTask.IsCompleted);
                }
            }
            
            var content = "";

            for (var i = 0; i < links.Count; i++)
            {
                var link = links[i];
                var request = UnityWebRequest.Get(link);

                yield return request.SendWebRequest();
                ShowErrorIfNeeded(request);

                var pageContent = request.downloadHandler.text;
                var title = RetrieveBetween(pageContent, beginTitleTag, endTitleTag);
                var post = RetrieveBetween(pageContent, beginTag, endTag);

                foreach (var key in TagConversions.Keys)
                {
                    var conversion = TagConversions[key];
                    post = key.StartsWith("r:")
                        ? Regex.Replace(post, key[2..], conversion)
                        : post.Replace(key, conversion);

                    yield return null;
                }

                content +=
                    WebUtility.HtmlDecode(
                        string.Format(headerFormat, title, dates[i].ToString(dateFormatSpecifier)) + post).Trim('\n') 
                    + splitter;
            }

            text.text = content;

            static string RetrieveBetween(string text, string beginTag, string endTag)
            {
                var invCulture = StringComparison.InvariantCulture;
                var start = text.IndexOf(beginTag, invCulture) + beginTag.Length;
                var length = text.Substring(start).IndexOf(endTag, invCulture);
                return text.Substring(start, length);
            }

            void ShowErrorIfNeeded(UnityWebRequest request)
            {
                if (request.result != UnityWebRequest.Result.Success)
                    text.text = $"<color=red>{request.error}</color>";
            }
        }
        
        static readonly Dictionary<string, string> TagConversions = new()
        {
            { "</p>", "\n" },
            // Replace <strong>, <h#>, <em>, and <del>
            { "<strong>", "<b>" },
            { "</strong>", "</b>" },
            { "r:<h.*?>", "<b>" },
            { "r:</h.*?>", "</b>" },
            { "<em>", "<i>" },
            { "</em>", "</i>" },
            { "<del>", "<s>" },
            { "</del>", "</s>" },
            // Reformat list tags
            { "<ul><li>", "• " },
            { "<ol><li>", "• " },
            { "<li>", "\n• " },
            { "</ul>", "\n" },
            { "</ol>", "\n" },
            // Other
            { "</pre>", "\n" },
            { "</blockquote>", "\n" },
            { "r:<table>.*?</table>", "[Can't display tables here.]\n" },
            // Remove all leftover tags (including closing tags)
            // Excludes removal of b, i, and s tags
            { "r:<(?!\\/*b\\b|\\/*i\\b|\\/*s\\b).*?>", "" }
        };
    }
}
