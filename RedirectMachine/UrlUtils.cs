using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RedirectMachine
{
    public class UrlUtils
    {
        private string urlHead, urlTail, sanitizedUrl;
        private bool endsWithSlash = false;
        private bool startsWithSlash = false;
        public string[] urlHeaderMap = new string[2];
        private string[] urlChunks;
        internal HashSet<string> urlChunksV2 = new HashSet<string>();

        public string OriginalUrl { get; set; }
        public string UrlHead
        {
            get { return urlHead; }
            set { urlHead = TruncateStringHead(OriginalUrl); }
        }
        public string UrlTail
        {
            get { return urlTail; }
            set { urlTail = TruncateString(value, 48); }
        }
        public string SanitizedUrl {
            get { return sanitizedUrl; }
            set { sanitizedUrl = CheckUrlTail(value); }
        }
        public string NewUrl { get; set; }
        public bool HasHeaderMap { get; set; } = false;
        public bool IsParentDir { get; set; } = false;
        public bool IsResourceFile { get; set; } = false;

        /// <summary>
        /// default constructor
        /// </summary>
        public UrlUtils()
        {
        }

        /// <summary>
        /// actual working constructor
        /// </summary>
        /// <param name="originalUrl"></param>
        public UrlUtils(string originalUrl)
        {
            OriginalUrl = originalUrl.Trim('"');
            UrlTail = OriginalUrl;
            UrlHead = OriginalUrl;
            SanitizedUrl = OriginalUrl;
            urlChunks = UrlTail.Split('-');
            CreateUrlChunksV2();
        }

        

        /// <summary>
        /// Check to see if the original url contains query strings
        /// </summary>
        public bool CheckForQueryStrings()
        {
            return OriginalUrl.Contains("?") ? true : false;
        }

        /// <summary>
        /// Purpose of method: retrieve usable/searchable end of url from variable value.
        /// Get url text after last slash in url
        /// </summary>
        /// <param name="value"></param>
        public string TruncateString(string value)
        {
            string temp = CheckVars(value);
            int index = value.Length;
            int pos = temp.LastIndexOf("/") + 1;
            return temp.Substring(pos, temp.Length - pos);
        }

        /// <summary>
        /// Purpose of method: retrieve usable/searchable end of url from variable value.
        /// Get url text after last slash in url
        /// </summary>
        /// <param name="value"></param>
        public string BasicTruncateString(string value)
        {
            string temp = CheckVars(value.Substring(0, value.Length - 1));
            int pos = temp.LastIndexOf("/") + 1;
            int i = temp.LastIndexOf("/");
            return "/" + value.Substring(pos, value.Length - pos);
        }

        /// <summary>
        /// Purpose of method: retrieve usable/searchable end of url from variable value.
        /// Get url text after last slash in url,
        /// truncate temporary value to maxLength
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public string TruncateString(string value, int maxLength)
        {
            string temp = CheckVars(value);
            int pos = temp.LastIndexOf("/") + 1;
            temp = temp.Substring(pos, temp.Length - pos);
            if (endsWithSlash)
                temp = temp + "/";
            if (!temp.StartsWith("/"))
                temp = "/" + temp;
            return temp.Length <= maxLength ? temp : temp.Substring(0, maxLength);
        }

        /// <summary>
        /// Purpose: return first chunk of url. 
        /// Check if url starts with http or https. If it does, grab entire domain of url
        /// if that doesn't exist, return the first chunk of the url in between the first two '/'
        /// </summary>
        /// <param name="value"></param>
        public string TruncateStringHead(string value)
        {
            IsParentDir = (urlTail.Contains(value));
            if (value.StartsWith("/"))
            {
                startsWithSlash = true;
                value = value.Substring(1);
            }
            int index = value.IndexOf("/");
            if (index <= -1)
                index = value.Length;
            
            value = value.Substring(0, index);
            if (startsWithSlash)
            {
                value = "/" + value;
                index++;
            }
                
            return (!value.EndsWith("/")) ? value.Substring(0, index) + "/" : value.Substring(0, index);
        }

        /// <summary>
        /// Purpose: return first chunk of url. 
        /// Check if url starts with http or https. If it does, grab entire domain of url
        /// if that doesn't exist, return the first chunk of the url in between the first two '/'
        /// </summary>
        /// <param name="value"></param>
        public string BasicTruncateStringHead(string value)
        {
            if (value.StartsWith("/"))
                value = value.Substring(1);
            int index = value.IndexOf("/");
            if (index <= -1)
                index = value.Length;
            return (!value.EndsWith("/")) ? "/" + value.Substring(0, index) + "/" : "/" + value.Substring(0, index);
        }

        /// <summary>
        /// remove unnecessary contents on end of url if found
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string CheckVars(string value)
        {
            if (value.EndsWith("/"))
            {
                endsWithSlash = true;
                value = GetSubString(value, "/", false);
            }
            if (value.EndsWith("-/"))
                value = GetSubString(value, "-/", false);
            if (value.EndsWith("-"))
                value = GetSubString(value, "-", false);
            value = Regex.Replace(value, "--", "-");
            value = Regex.Replace(value, "---", "-");
            value = Regex.Replace(value, "dont", "don-t");
            value = Regex.Replace(value, "cant", "can-t");
            if (value.Contains("."))
                IsResourceFile = true;
            return value;
        }

        /// <summary>
        /// return first position of j variable in string i.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public static int GetFirstIndex(string i, string j)
        {
            return (i.Contains(j)) ? i.IndexOf(j) : i.Length;
        }

        /// <summary>
        /// return last position of j variable in string i. If not found, return all of string i
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public static int GetLastIndex(string i, string j)
        {
            return (i.Contains(j)) ? i.LastIndexOf(j) : i.Length;
        }

        /// <summary>
        /// return the substring of the string that is passed into this function.
        /// This method is overloaded with a bool. The bool indicates to the function that it must return a substring
        /// 1) if true, includes the string j rather than excluding it, or
        /// 2) if false, returns a substring that excludes string j.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static string GetSubString(string i, string j, bool x)
        {
            int index = GetLastIndex(i, j);
            return (x == true) ? i.Substring(0, index + j.Length) : i.Substring(0, index);
        }

        /// <summary>
        /// return the substring of the string that is passed into this function.
        /// This method is overloaded with an int. The int indicates to the function that it must rerun that many times.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static string GetSubString(string i, string j, int x)
        {
            var pos = 0;
            string temp = i;
            while (pos <= x)
            {
                int index = GetLastIndex(i, j);
                temp = temp.Substring(0, index);
                pos++;
            }
            return temp;
        }

        /// <summary>
        /// return a url that has a slash on the end for redirection purposes.
        /// if url contains an ending extension already (eg .html), skip
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string CheckUrlTail(string url)
        {
            if (url.Contains("?"))
                url = GetSubString(url, "?", false);
            if (url.Contains("."))
                return url;
            return (!url.EndsWith("/") ? url + "/" : url);
        }

        /// <summary>
        /// return a string build from a series of chunks from the working url
        /// </summary>
        /// <param name="index"></param>
        public string BuildChunk(int index)
        {
            string temp = urlChunks[0];
            for (int i = 1; i < index; i++)
            {
                temp = temp + "-" + urlChunks[i];
            }
            return temp;
        }

        /// <summary>
        /// return a string build from a series of chunks from the working url
        /// </summary>
        /// <param name="index"></param>
        public string GetChunk(int index)
        {
            return urlChunks[index];
        }

        /// <summary>
        /// return a string build from a series of chunks from the working url
        /// </summary>
        /// <param name="index"></param>
        public int GetChunkLength()
        {
            return urlChunks.Length;
        }

        internal bool IsChunkFoundInResource(string chunk)
        {
            return urlChunksV2.Contains(chunk);
        }

        /// <summary>
        /// set urHeaderMap array to string a/b values
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public void SetUrlHeaderMap(string a, string b)
        {
            urlHeaderMap[0] = a;
            urlHeaderMap[1] = b;
            UrlHead = urlHeaderMap[1];
        }

        private void CreateUrlChunksV2()
        {
            urlChunksV2 = SanitizedUrl.Split(new Char[] { '-', '/' }).ToHashSet();
        }

        internal string[] ReturnResourceDirChunks(string url)
        {
            string temp = BasicTruncateString(url);
            string[] chunks = temp.Split(new Char[] { '-', '/' }).ToArray();
            return chunks;
        }

        internal string[] ReturnAllChunksInUrl(string url)
        {
            string[] chunks = url.Split(new Char[] { '-', '/' }).ToArray();
            return chunks;
        }

        internal int ReturnResourceDirMatches(string url)
        {
            int j = 0;
            List<string> chunks = ReturnResourceDirChunks(url).ToList();
            foreach (var chunk in urlChunks)
            {
                if (chunks.Contains(chunk))
                    j++;
            }
            return j * 2;
        }

        internal int ReturnFullUrlMatches(string url)
        {
            int j = 0;
            List<string> chunks = ReturnAllChunksInUrl(url).ToList();
            foreach (var chunk in urlChunksV2)
            {
                if (chunks.Contains(chunk))
                    j++;
            }
            return j;
        }
    }
}