﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace RedirectMachine
{
    abstract class UrlObject
    {
        public string GetOriginalUrl()
        {
            // Purpose: return private string originalUrl
            return originalUrl;
        }

        public string GetSanitizedUrl()
        {
            return CheckUrlTail(originalUrl);
        }

        public string GetNewUrl()
        {
            // Purpose: return private string newUrl
            return newUrl;
        }

        public string GetUrlSub()
        {
            // Purpose: return private stirng urlSub
            return tail;
        }

        public int GetScore()
        {
            // Purpose: return private int score
            return score;
        }

        public int GetCount()
        {
            // Purpose: return private int score
            return count;
        }

        public void AddScore()
        {
            // Purpose of method: add score
            score++;
        }

        public void SubtractScore()
        {
            // Purpose of method: subtract score
            score--;
        }

        public void AddUrlHeaderMap(string a, string b)
        {
            urlHeaderMap[0] = a;
            urlHeaderMap[1] = b;
            Console.WriteLine($"{urlHeaderMap[0]}, {urlHeaderMap[1]}");
        }

        public string TruncateString(string value)
        {
            // Purpose of method: retrieve usable/searchable end of url from variable value.
            // Get url text after last slash in url
            string temp = CheckVars(value);
            int index = value.Length;
            int pos = temp.LastIndexOf("/") + 1;
            temp = temp.Substring(pos, temp.Length - pos);
            return temp;
        }

        public string TruncateString(string value, int maxLength)
        {
            // Purpose of method: retrieve usable/searchable end of url from variable value.

            // Get url text after last slash in url,
            // truncate temporary value to maxLength
            string temp = CheckVars(value);
            int index = temp.Length;
            int pos = temp.LastIndexOf("/") + 1;
            temp = temp.Substring(pos, temp.Length - pos);
            return temp.Length <= maxLength ? temp : temp.Substring(0, maxLength);
        }

        public string TruncateStringHead(string value)
        {
            // Purpose: return first chunk of url. 
            // Check if url starts with http or https. If it does, grab entire domain of url
            // if that doesn't exist, return the first chunk of the url in between the first two '/'
            string temp = value;

            if (temp.StartsWith("/"))
                temp = temp.Substring(1);
            int index = temp.IndexOf("/");
            if (index <= -1)
                index = temp.Length;
            temp = temp.Substring(0, index).ToLower();
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

        public static string GetSubString(string i, string j, int x)
        {
            // Purpose of method: return the substring of the string that is passed into this function.
            // This method is overloaded with an int. The int indicates to the function that it must rerun that many times.
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

        public string TrimFullUrl(string value)
        {
            int index = value.IndexOf("//");
            Console.WriteLine($"Length of value: {value.Length}");
            Console.WriteLine($"index of slashes: {index}");
            string temp = value.Substring(value.IndexOf("//"), value.Length - value.IndexOf("//"));
            temp = temp.Substring(0, GetFirstIndex(temp, "/"));
            return temp.Substring(GetFirstIndex(temp, "."), GetLastIndex(temp, "."));
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
