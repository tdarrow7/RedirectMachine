using System;
using System.Collections.Generic;
using System.Text;

namespace RedirectMachine
{
    class DictionaryUrl : UrlObject
    {
        private string dictionaryUrl;

        public DictionaryUrl()
        {
            // Default contstructor
            dictionaryUrl = "";
        }

        public DictionaryUrl(string url) : base (url)
        {
            dictionaryUrl = GetSanitizedUrl();
        }

        public bool CheckDictionary(string url)
        {
            if (dictionaryUrl.Contains(url))
                return false;
            else
            {
                AddCount();
                return true;
            }
        }

        public string GetProbableUrl()
        {
            return dictionaryUrl;
        }
    }

}
