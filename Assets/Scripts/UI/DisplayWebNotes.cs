using System;
using System.Collections;
using System.Collections.Generic;
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
            
            if (beginTag == "" || endTag == "")
                throw new ArgumentException("Beginning or ending tags are unspecified.");
        }

        public void Refresh() => Start();
        
        void Start()
        {
            StopAllCoroutines();
            StartCoroutine(ProcessPosts());
        }

        IEnumerator ProcessPosts()
        {
            text.text = loadingMessage;

            var posts = new List<Post>();

            var request = UnityWebRequest.Get(rssFeedSource);
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                text.text = $"<color=red>{request.error}</color>";
                throw new Exception(request.error);
            }

            var doc = new XmlDocument();
            doc.LoadXml(request.downloadHandler.text);
            var postNodes = doc.SelectNodes("//channel/item");
            
            // Start processing RSS feed
            
            foreach (XmlNode node in postNodes)
            {
                var post = new Post();
                
                // format: title, desc, link, pubDate
                foreach (XmlNode prop in node.ChildNodes)
                {
                    var value = prop.InnerText;
                    switch (prop.Name)
                    {
                        case "title":
                            post.title = value;
                            break;
                        case "description":
                            post.description = value;
                            break;
                        case "pubDate":
                            post.pubDate = DateTime.Parse(value);
                            break;
                    }
                }
                posts.Add(post);
            }
            
            // Reformat description text
            
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
            // Links
            { "r:<a.*?url=(.*?)\".*?>(.*?)</a>", "$2 ($1)" },
            { "r:<a.*?href=\"(.*?)\".*?>(.*?)</a>", "$2 ($1)" },
            { "r:<span class=\"bb_link_host\">.*?</span>", "" },
            // Remove all leftover tags (including closing tags)
            // Excludes removal of b, i, and s tags
            { "r:<(?!\\/*b\\b|\\/*i\\b|\\/*s\\b).*?>", "" }
        };
    }
}
