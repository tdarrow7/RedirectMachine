using System;

namespace RedirectMachine
{
    internal class CatchAllObject
    {
        int count = 1;
        string catchAllUrl;

        public CatchAllObject(URLObject obj)
        {
            catchAllUrl = obj.GetSanitizedUrl();
        }

        internal void IncreaseCount()
        {
            count++;
        }
    }

    
}