using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RedirectMachine
{
    public class UrlUtils
    {
        private string urlParentDir, urlResourceDir, sanitizedUrl;
        private bool endsWithSlash = false;
        private bool startsWithSlash = false;
        public string[] urlHeaderMap = new string[2];
        private string[] urlResourceChunks;
        internal string[] urlAllChunks;

        internal Tuple<string, int> urlResourceChunksV2;
        internal Tuple<string, int> urlAllChunksV2;

        public string OriginalUrl { get; set; }
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
            UrlResourceDir = OriginalUrl;
            UrlParentDir = OriginalUrl;
            SanitizedUrl = OriginalUrl;
            urlResourceChunks = SplitUrlChunks(UrlResourceDir);
            urlAllChunks = SplitUrlChunks(SanitizedUrl);
        }

        /// <summary>
        /// split url into a temporary list
        /// eliminate blank entries from that list
        /// return the list as an array
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
            IsParentDir = (urlResourceDir.Contains(value));
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
            if (value.EndsWith("#"))
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
            if (url.EndsWith("-/"))
                url = GetSubString(url, "-/", false);
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
        public string GetResourceDirChunk(int index)
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

        internal bool IsChunkFoundInResource(string chunk)
        {
            return urlAllChunks.Contains(chunk);
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

        ///// <summary>
        ///// split url into a temporary list
        ///// eliminate blank entries from that list
        ///// return the list as an array
        ///// </summary>
        ///// <param name="url"></param>
        ///// <returns></returns>
        //internal string[] SplitUrlChunksV2(string url)
        //{
        //    int j = 0;
        //    string temp = BasicTruncateString(url);

        //    string[] urlArray = url.Split(new Char[] { '-', '/' }).ToArray();
        //    urlArray = new HashSet<string>(urlArray).ToArray();

        //    string[] tempArray = temp.Split(new Char[] { '-', '/' }).ToArray();
        //    tempArray = new HashSet<string>(tempArray).ToArray();



            //List<string> urlList = url.Split(new Char[] { '-', '/' }).ToList();
            //urlList.RemoveAll(i => i == "");

            //if (!urlList.Any())
            //    urlList.Add("");
            //return urlList.ToArray();
        //}


    }
}