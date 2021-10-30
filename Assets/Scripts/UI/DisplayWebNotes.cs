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
        [SerializeField, RequireSubstring("{0}")] string titleFormat;
        [Header("Content Tags")]
        [SerializeField] string beginTag;
        [SerializeField] string endTag;
        [SerializeField] string splitter;
        [Header("TMP")]
        [SerializeField] TextMeshProUGUI text;

        void Awake()
        {
            splitter = Regex.Unescape(splitter);
            titleFormat = Regex.Unescape(titleFormat);
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

            var feed = UnityWebRequest.Get(rssFeedSource);
            yield return feed.SendWebRequest();
            ShowErrorIfNeeded(feed);
            
            using var strReader = new StringReader(feed.downloadHandler.text);
            {
                var settings = new XmlReaderSettings {Async = true};
                var reader = XmlReader.Create(strReader, settings);
                var foundItem = false;
                var onLink = false;

                var readTask = reader.ReadAsync();
                yield return new WaitUntil(() => readTask.IsCompleted);

                while (readTask.Result)
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (!foundItem && reader.Name == "item") foundItem = true;
                            if (foundItem && reader.Name == "link") onLink = true;
                            break;
                        case XmlNodeType.Text:
                            if (onLink)
                            {
                                links.Add(reader.Value);
                                onLink = false;
                            }
                            break;
                    }

                    readTask = reader.ReadAsync();
                    yield return new WaitUntil(() => readTask.IsCompleted);
                }
            }
            
            var content = "";
            
            foreach (var link in links)
            {
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
                        ? Regex.Replace(post, key.Substring(2), conversion)
                        : post.Replace(key, conversion);
                    
                    yield return null;
                }

                content += WebUtility.HtmlDecode(string.Format(titleFormat, title) + post).Trim('\n') + splitter;
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
            { "\n", "" },
            { "</p>", "\n" },
            { "<strong>", "<b>" },
            { "</strong>", "</b>" },
            { "<em>", "<i>" },
            { "</em>", "</i>" },
            { "<del>", "<s>" },
            { "</del>", "</s>" },
            { "<ul><li>", "- " },
            { "<ol><li>", "- " },
            { "<li>", "\n- " },
            { "</ul>", "\n" },
            { "</ol>", "\n" },
            { "</pre>", "\n" },
            { "</blockquote>", "\n" },
            { "r:</h.*?>", "\n" },
            { "r:<table>.*?</table>", "[Can't display tables here.]\n" },
            // Remove all leftover tags (including closing tags)
            { "r:<.*?>", "" }
        };
    }
}
