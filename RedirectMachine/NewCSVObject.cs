using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RedirectMachine
{
    internal class NewCSVObject
    {
        List<string> urlList;

        public NewCSVObject()
        {
            urlList = new List<string>();
        }

        internal void ReadNewUrlsIntoList(string nsUrlFile)
        {
            // Purpose: add CSV file contents to list
            using (var reader = new StreamReader(@"" + nsUrlFile))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    line = line.ToLower();
                    urlList.Add(line);
                }
                urlList.Sort();
            }
        }
    }
}
