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

            var posts = new List<Post>();

            var feed = UnityWebRequest.Get(rssFeedSource);
            yield return feed.SendWebRequest();
            ShowErrorIfNeeded(feed);
            
            // Start processing RSS feed
            
            using var strReader = new StringReader(feed.downloadHandler.text);
            {
                var settings = new XmlReaderSettings {Async = true};
                var reader = XmlReader.Create(strReader, settings);

                var readTask = reader.ReadAsync();
                yield return new WaitUntil(() => readTask.IsCompleted);

                while (readTask.Result)
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "item")
                    {
                        yield return Read();
                        
                        // Posts must begin with titles
                        if (reader.Name != "title") continue;
                        
                        // format: title, desc, link, pubDate
                        yield return Read();
                        var title = reader.Value;
                        for (var i = 0; i < 2; i++) yield return Read();
                        var description = reader.Value;
                        for (var i = 0; i < 4; i++) yield return Read();
                        var pubDate = DateTime.Parse(reader.Value);

                        posts.Add(new Post { title = title, description = description, pubDate = pubDate });
                    }

                    yield return Read();

                    IEnumerator Read()
                    {
                        readTask = reader.ReadAsync();
                        yield return new WaitUntil(() => readTask.IsCompleted);
                        
                        if (reader.NodeType is XmlNodeType.Whitespace or XmlNodeType.EndElement)
                            yield return Read();
                    }
                }
            }
            
            // Grab and format text from each item's link
            
            var content = "";

            foreach (var post in posts)
            {
                var postText = post.description;
                
                foreach (var key in TagConversions.Keys)
                {
                    var conversion = TagConversions[key];
                    postText = key.StartsWith("r:")
                        ? Regex.Replace(postText, key[2..], conversion)
                        : postText.Replace(key, conversion);

                    yield return null;
                }
                
                content +=
                    WebUtility.HtmlDecode(
                        string.Format(headerFormat, post.title, post.pubDate.ToString(dateFormatSpecifier)) + postText).Trim('\n') 
                    + splitter;
            }

            text.text = content;

            void ShowErrorIfNeeded(UnityWebRequest request)
            {
                if (request.result != UnityWebRequest.Result.Success)
                    text.text = $"<color=red>{request.error}</color>";
            }
        }

        struct Post
        {
            public string title;
            public DateTime pubDate;
            public string description;
        }
        
        static readonly Dictionary<string, string> TagConversions = new()
        {
            { "<br>", "\n" },
            { "<del>", "<s>" },
            { "</del>", "</s>" },
            // Reformat list tags
            { "<ul class=\"bb_ul\"><li>", "• " },
            { "<ol class=\"bb_ul\"><li>", "• " },
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
