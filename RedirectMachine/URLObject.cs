using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace RedirectMachine
{
    abstract class UrlObject
    {
        //private string originalUrl;
        //private int count;
        private string originalUrl, sanitizedUrl;
        private int count;
        private bool test = true;

        public UrlObject()
        {
            // default constructor
            originalUrl = "";
            sanitizedUrl = "";
            count = 0;
        }

        public UrlObject(string url)
        {
            originalUrl = url;
            sanitizedUrl = CheckUrlTail(url);
            count = 1;
        }

        public bool ReturnTest()
        {
            return test;
        }

        public string GetOriginalUrl()
        {
            // Purpose: return private string originalUrl
            return originalUrl;
        }

        public string GetSanitizedUrl()
        {
            return sanitizedUrl;
        }

        public int GetCount()
        {
            // Purpose: return private int score
            return count;
        }

        public void AddCount()
        {
            count++;
        }

        public void SubtractCount()
        {
            count--;
        }

        public void SetCount(int i)
        {
            count = i;
        }

        public string TruncateUrl(string value)
        {
            // Purpose of method: retrieve usable/searchable end of url from variable value.
            // Get url text after last slash in url
            string temp = CheckVars(value);
            int index = value.Length;
            int pos = temp.LastIndexOf("/") + 1;
            temp = temp.Substring(pos, temp.Length - pos);
            return temp;
        }

        public string CheckVars(string value)
        {
            // Purpose: remove unnecessary contents on end of url if found
            if (value.Contains("?"))
                value = GetSubString(value, "?", false);
            if (value.Contains("."))
                value = GetSubString(value, ".", false);
            if (value.EndsWith("/"))
                value = GetSubString(value, "/", false);
            if (value.EndsWith("-"))
                value = GetSubString(value, "-", false);
            value = Regex.Replace(value, "--", "-");
            value = Regex.Replace(value, "---", "-");
            value = Regex.Replace(value, "dont", "don-t");
            value = Regex.Replace(value, "cant", "can-t");
            return value;
        }

        public static string GetSubString(string i, string j, bool x)
        {
            // Purpose of method: return the substring of the string that is passed into this function.
            // This method is overloaded with a bool. The bool indicates to the function that it must return a substring
            // 1) if true, includes the string j rather than excluding it, or
            // 2) if false, returns a substring that excludes string j.
            int index = GetLastIndex(i, j);
            string temp;
            if (x == true)
            {
                temp = i.Substring(0, index + j.Length);
            }
            else
                temp = i.Substring(0, index);
            return temp;
        }

        public static int GetFirstIndex(string i, string j)
        {
            // Purpose of method: return first position of j variable in string i.
            if (i.Contains(j))
                return i.IndexOf(j);
            else
                return i.Length;
        }

        public static int GetLastIndex(string i, string j)
        {
            // Purpose of method: return last position of j variable in string i.
            if (i.Contains(j))
                return i.LastIndexOf(j);
            else
                return i.Length;
        }

        public string CheckUrlTail(string url)
        {
            // Purpose: return a url that has a slash on the end for redirection purposes.
            // if url contains an ending extension already (eg .html), skip
            if (url.Contains("?"))
                url = GetSubString(url, "?", false);
            if (url.Contains("."))
                return url;
            return (!url.EndsWith("/") ? url + "/" : url);
        }

    }
}
