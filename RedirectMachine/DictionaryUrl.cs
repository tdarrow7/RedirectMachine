using System;
using System.Collections.Generic;
using System.Text;

namespace RedirectMachine
{
    class DictionaryUrl : UrlObject
    {
        private string oldUrl;

        public DictionaryUrl()
        {
            // Default contstructor
            oldUrl = "";
        }

        public DictionaryUrl(string url)
        {
            oldUrl = GetSanitizedUrl();
        }

        public bool CheckDictionary(string url)
        {
            if (oldUrl.Contains(url))
                return false;
            else
            {
                AddCount();
                return true;
            }
        }
    }

}
