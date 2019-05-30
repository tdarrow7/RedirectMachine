using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RedirectMachine
{
    public class UrlUtils
    {
        private string urlParentDir, urlResourceDir, sanitizedUrl, originalUrl;
        private bool startsWithSlash = false;
        public string[] urlHeaderMap = new string[2];
        internal string[] urlResourceChunks;
        internal string[] urlAllChunks;

        //public string OriginalUrl { get; set; }
        public string OriginalUrl
        {
            get { return originalUrl; }
            set
            {
                originalUrl = CheckIfEndsWithSlash(value);
            }
        }

        public string UrlParentDir
        {
            get { return urlParentDir; }
            set { urlParentDir = TruncateStringHead(OriginalUrl); }
        }
        public string UrlResourceDir
        {
            get { return urlResourceDir; }
            set { urlResourceDir = TruncateString(value, 48); }
        }
        public string SanitizedUrl
        {
            get { return sanitizedUrl; }
            set { sanitizedUrl = CheckVars(value); }
        }
        public string NewUrl { get; set; }
        public bool HasHeaderMap { get; set; } = false;
        public bool IsParentDir { get; set; } = false;

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
            //OriginalUrl = originalUrl.Trim('"');
            OriginalUrl = originalUrl;
            UrlResourceDir = OriginalUrl;
            UrlParentDir = OriginalUrl;
            SanitizedUrl = OriginalUrl;
            urlResourceChunks = SplitUrlChunks(UrlResourceDir);
            //urlAllChunks = SplitUrlChunks(SanitizedUrl);
            urlAllChunks = SplitUrlChunks(OriginalUrl);
        }

        /// <summary>
        /// split url into a temporary list
        /// eliminate blank entries from that list
        /// return the list as an array
        /// if the array is empty (such as the root directory), add an empty string
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        internal string[] SplitUrlChunks(string url)
        {
            List<string> tempList = url.Split(new Char[] { '-', '/' }).ToList();
            tempList.RemoveAll(i => i == "");
            if (!tempList.Any())
                tempList.Add("");
            return tempList.ToArray();
        }

        /// <summary>
        /// Return the urlResourceChunks[] array
        /// </summary>
        /// <returns></returns>
        internal string[] ReturnUrlResourceChunks()
        {
            return urlResourceChunks;
        }

        /// <summary>
        /// Return the length of the urlResourceChunks[] array
        /// </summary>
        /// <returns></returns>
        internal int ReturnUrlResourceChunkLength() {
            return urlResourceChunks.Length;
        }

        /// <summary>
        /// Return the urlAllChunks[] array
        /// </summary>
        /// <returns></returns>
        internal string[] ReturnAllUrlChunks()
        {
            return urlAllChunks;
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
            IsParentDir = (urlResourceDir.Contains(value));
            if (value.StartsWith("/"))
            {
                startsWithSlash = true;
                value = value.Substring(1);
            }
            else
            {
                value = "/" + new Uri(value).Segments[1];
                value = value.Substring(0, value.Length - 1);
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

            //return (!value.EndsWith("/")) ? value.Substring(0, index) + "/" : value.Substring(0, index);
            return value.Substring(0, index);
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
            value = Regex.Replace(value, "--", "-");
            value = Regex.Replace(value, "---", "-");
            value = Regex.Replace(value, "dont", "don-t");
            value = Regex.Replace(value, "cant", "can-t");
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

        ///// <summary>
        ///// return a url that has a slash on the end for redirection purposes.
        ///// if url contains an ending extension already (eg .html), skip
        ///// </summary>
        ///// <param name="url"></param>
        ///// <returns></returns>
        //internal string CheckUrlTail(string url)
        //{
        //    if (url.EndsWith("-/"))
        //        url = GetSubString(url, "-/", false);
        //    if (url.Contains("."))
        //        return url;
        //    return url;
        //}

        /// <summary>
        /// return a string build from a series of chunks from the working url
        /// </summary>
        /// <param name="index"></param>
        public string BuildChunk(int index)
        {
            string temp = urlResourceChunks[0];
            for (int i = 1; i < index; i++)
            {
                temp = temp + "-" + urlResourceChunks[i];
            }
            return temp;
        }

        /// <summary>
        /// return a string build from a series of chunks from the working url
        /// </summary>
        /// <param name="index"></param>
        public string GetResourceChunk(int index)
        {
            return urlResourceChunks[index];
        }

        /// <summary>
        /// return a string build from a series of chunks from the working url
        /// </summary>
        /// <param name="index"></param>
        public int GetChunkLength()
        {
            return urlResourceChunks.Length;
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
            UrlParentDir = urlHeaderMap[1];
        }

        /// <summary>
        /// return the number of times each entry in the chunks[] array is seen in string url
        /// </summary>
        /// <param name="url"></param>
        /// <param name="chunks"></param>
        /// <returns></returns>
        internal int ReturnUrlMatches(string url, string[] chunks)
        {
            int j = 0;

            foreach (var chunk in chunks)
            {
                if (url.Contains(chunk))
                    j = j + (chunk.Length > 0 ? chunk.Length - 1 : 0);
            }
            return j;
        }

        /// <summary>
        /// do a very quick check if the url ends with a slash
        /// this also takes into account the root directory url.
        /// </summary>
        /// <param name="value"></param>
        private string CheckIfEndsWithSlash(string value)
        {
            if (value.Length == 1)
                return value;
            return value.EndsWith("/") ? value.Substring(0, value.Length - 1) : value;
        }
    }
}