using System.Collections.Generic;
using System.IO;

namespace RedirectMachine
{
    internal class OldCSVObject
    {
        public List<CatchAllObject> catchAllList;
        //public List<UrlUtils> urlUtils;

        private string[,] osParams =  {
            { "/events/details/", "/classes-events/" },
            { "/events/event-results/", "/classes-events/" },
            { "/events/search-results/", "/classes-events/" },
            { "/events/smart-panel-overflow/", "/classes-events/" },
            { "/lifestyle-health-classes-and-events/lifestyle-health-calendar-of-events/", "/classes-events/" },
            { "/for-the-health-of-it/full-blog-listing/?searchId", "/blog/" },
            { "/locations/location-clinics/clinic-profile/", "/locations/" },
            { "/locations/results/", "/locations/" },
            { "/locations/profile/?id=", "/locations/" },
            { "/locations/monticello/enewsletter/", "/locations/centracare-monticello/" },
            { "/location-tabs-test/", "/locations" },
            { "/patients-visitors/cheer-cards/", "/ecards/" },
            { "/about-us/news-publications/news/?searchId", "/blog/" },
            { "/for-the-health-of-it/search-results/?searchId", "/blog/" },
            { "/about-us/news-publications/news/?year", "/blog/" },
            { "/providers/results/?searchId=", "/our-doctors/" },
            { "/providers/results/?termId=", "/our-doctors/" },
            { "/search-for-pages/results/?searchId", "/site-search/" },
            { "/services/?c=", "/our-services/" },
            { "/app/files/", "/" }
        };

        private string[,] urlHeaderMaps = {
            { "https://www.google.com", "/googleness/" }
        };

        public OldCSVObject()
        {

        }

        internal void ReadOldUrlsIntoList(string osUrlFile)
        {
            // Purpose: add CSV file contents to list
            using (var reader = new StreamReader(@"" + osUrlFile))
            {
                while (!reader.EndOfStream)
                {
                    bool catchAll = false;
                    var urlObj = new URLObject(reader.ReadLine());


                    var line = reader.ReadLine();
                    // When new line is read, reset catchAll property. Trim qoutes from var line temporarily


                    var temp = line.ToLower().Trim('"');
                    for (int i = 0; i < keyVals.GetLength(0); i++)
                    {
                        // Check if temp variable starts with any of the keyVal parameters. If found, do not add line to list
                        if (temp.StartsWith(keyVals[i, 0].ToString().ToLower()))
                        {
                            catchAll = true;
                            counter++;
                            break;
                        }
                    }
                    if (catchAll == false)
                        list.Add(new URLObject(line));
                }
            }


        }
        
        /// <summary>
        /// checks to see if the url belongs to one of the existing osParam catchAlls. If it does, don't do anything further with it. Essentially ignore that object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal bool CheckCatchAll(URLObject obj)
        {
            /// 
            var temp = obj.GetOriginalUrl();

            for (int i = 0; i < osParams.GetLength(0); i++)
            {
                // Check if temp variable starts with any of the keyVal parameters. If found, do not add line to list
                if (temp.StartsWith(osParams[i, 0].ToString().ToLower()))
                {
                    //catchAll = true;
                    //counter++;
                    break;
                }
            }

            if (true)
            {
                return true;
            }
            else return false;
            
        }
    }
}