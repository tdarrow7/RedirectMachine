using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RedirectMachine
{
    public class UrlUtils
    {
        private string _originalUrl, urlHead, urlTail, newUrl, sanitizedUrl;
        public string[] urlHeaderMap = new string[2];
        private bool isParentDir = false;
        private bool hasUrlHeaderMap = false;

        public string OriginalUrl { get; set; }
        public string UrlHead
        {
            get {
                return urlHead;
            }
            set
            {
                urlHead = TruncateStringHead(OriginalUrl);
            }
        }

        public string UrlTail
        {
            get
            {
                return urlTail;
            }
            set
            {
                urlTail = TruncateString(value, 48);
            }
        }

        public string SanitizedUrl {
            get
            {
                return sanitizedUrl;
            }
            set
            {
                sanitizedUrl = CheckUrlTail(value);
            }
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
            OriginalUrl = originalUrl.ToLower().Trim('"');
            UrlTail = OriginalUrl;
            UrlHead = OriginalUrl;
            SanitizedUrl = OriginalUrl;
        }

        /// <summary>
        /// Check to see if the original url contains query strings
        /// </summary>
        public bool CheckForQueryStrings()
        {
            return _originalUrl.Contains("?") ? true : false;
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
            temp = temp.Substring(pos, temp.Length - pos);
            return temp;
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
            int index = temp.Length;
            int pos = temp.LastIndexOf("/") + 1;
            temp = temp.Substring(pos, temp.Length - pos);
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
            string temp = value;
            if (temp.StartsWith("/"))
                temp = temp.Substring(1);
            int index = temp.IndexOf("/");
            if (index <= -1)
                index = temp.Length;
            if (urlTail.Contains(temp))
            {
                isParentDir = true;
            }
            temp = temp.Substring(0, index).ToLower();
            return temp;
        }

        /// <summary>
        /// remove unnecessary contents on end of url if found
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string CheckVars(string value)
        {
            if (value.Contains("?"))
                value = GetSubString(value, "?", false);
            if (value.Contains("."))
                value = GetSubString(value, ".", false);
            if (value.EndsWith("/"))
                value = GetSubString(value, "/", false);
            if (value.EndsWith("/*"))
                value = GetSubString(value, "/*", false);
            if (value.EndsWith("-"))
                value = GetSubString(value, "-", false);
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
        /// return last position of j variable in string i.
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
        /// return something
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string TrimFullUrl(string value)
        {
            int index = value.IndexOf("//");
            Console.WriteLine($"Length of value: {value.Length}");
            Console.WriteLine($"index of slashes: {index}");
            string temp = value.Substring(value.IndexOf("//"), value.Length - value.IndexOf("//"));
            temp = temp.Substring(0, GetFirstIndex(temp, "/"));
            return temp.Substring(GetFirstIndex(temp, "."), GetLastIndex(temp, "."));
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
    }
}