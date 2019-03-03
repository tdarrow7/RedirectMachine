using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RedirectMachine
{
    class RedirectUrl : UrlObject
    {
        private string head, tail, newUrl;
        private int score;
        public List<string> matchedUrls;
        public string[] urlChunks;
        public string[] urlHeaderMap = new string[2];

        
        public RedirectUrl()
        {
            // default constructor
        }

        public RedirectUrl(string originalUrl)
        {
            // create working constructor
            head = TruncateStringHead(GetOriginalUrl());
            tail = TruncateString(originalUrl, 48);
            score = 0;
            matchedUrls = new List<string>();
            urlChunks = tail.Split("-").ToArray();
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

        public void AddMatchedUrl(string link)
        {
            // Purpose: add potential url match
            matchedUrls.Add(link);
            AddCount();
        }

        public void RemoveMatchedUrl(string link)
        {
            matchedUrls.Remove(link);
            SubtractCount();
        }

        public void ClearMatches()
        {
            matchedUrls.Clear();
        }

        public void CheckUrl(string url)
        {
            string temp = TruncateString(url, 48);
            if (temp.Contains(tail))
            {
                AddMatchedUrl(url);
            }
        }

        public void AdvCheckUrl(string url)
        {
            string temp = TruncateString(url, 48);
            if (temp.Contains(urlChunks[0]))
            {
                AddMatchedUrl(url);
            }
        }

        public bool ScanMatchedUrls()
        {
            int i = GetCount();
            //Console.WriteLine($"count for {tail} is: {count}");
            if (i == 0)
                return false;
                
            else if (i == 1)
            {
                newUrl = matchedUrls.First();
                AddScore();
                return true;
            }
            else
            {
                SetCount(0);
                List<string> list1 = new List<string>();
                foreach (var url in matchedUrls)
                {
                    list1.Add(url);
                }
                foreach (var url in list1)
                {
                    string temp = TruncateStringHead(url);
                    if (!temp.Contains(head))
                    {
                        matchedUrls.Remove(temp);
                        SubtractCount();
                    }
                }
                if (GetCount() == 1)
                {
                    newUrl = matchedUrls.First();
                    AddScore();
                    return true;
                }
            }
            return false;
        }

        public bool AdvScanUrls()
        {
            int tempCount = matchedUrls.Count;
            List<string> activeList = new List<string>();
            List<string> passiveList = new List<string>();

            foreach (var item in matchedUrls)
            {
                passiveList.Add(item);
            }
            for (int i = 0; i < urlChunks.Length; i++)
            {
                activeList.Clear();
                foreach (var item in passiveList)
                {
                    activeList.Add(item);
                }
                string temp = BuildChunk(i);
                foreach (var url in activeList)
                {
                    if (!url.Contains(temp))
                    {
                        passiveList.Remove(url);
                        // if index is greater than two, keep it in the matchedUrls list in case we want to spit out potential redirects to user
                        //if (i < 2)
                        matchedUrls.Remove(url);
                        // subtract count. used to determine if a match has not been found.
                        tempCount--;
                    }
                }
                if (tempCount == 0)
                {
                    return false;
                }
                    
                if (tempCount == 1)
                {
                    // found a single url that matches paramaters. 
                    // run one final check: 
                    
                    if (i < urlChunks.Length)
                    {
                        temp = BuildChunk(i + 1);
                        if (!passiveList.First().Contains(temp))
                            return false;
                    }
                    //Return this url as a redirect
                    newUrl = passiveList.First();
                    AddScore();
                    return true;
                }
            }
            return false;
        }

        public string BuildChunk(int index)
        {
            string temp = urlChunks[0];
            for (int i = 1; i < index; i++)
            {
                temp = temp + "-" + urlChunks[i];
            }
            return temp;
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

        public string TrimFullUrl(string value)
        {
            int index = value.IndexOf("//");
            Console.WriteLine($"Length of value: {value.Length}");
            Console.WriteLine($"index of slashes: {index}");
            string temp = value.Substring(value.IndexOf("//"), value.Length - value.IndexOf("//"));
            temp = temp.Substring(0, GetFirstIndex(temp, "/"));
            return temp.Substring(GetFirstIndex(temp, "."), GetLastIndex(temp, "."));
        }
    }

}