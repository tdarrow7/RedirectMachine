# RedirectMachine

SUMMARY
The redirect machine is a tool that automates the searching and matching of 301 redirects.
The idea behind the tool originated because the process of finding a matching 301 is a completely
	repetitive and simple process. The goal of the tool is to finish most of the heavy lifting
	by searching and matching urls from the old site to the urls on the new site.
	

Order of Operations:
1. Pull in a list of urls found by ScreamingFrog, a list of urls and page names from the site’s Page Manager, and both existing and preset catchalls.
2. Check if any urls pulled in by SreamingFrog can be skipped over because it already exists as a catchall or can be turned into a catchall.
3. Turn each url into a DTO for easy export.
4. Scan the old urls and checks to see if it finds any matches in the new urls using four different scanning methods. If only one match is found, it sets up the found url with a redirect and a “quality of match” flag. If it either found too many matches and couldn’t determine best match, or didn’t find any matches, it adds it to a lost url list.
5. Return three CSVs that need to be sifted through by a human operator:
	a. FoundUrls.csv: A list of the old urls, the new urls that they map to, and a flag that says the match was either Great, Good, Decent, or Needs To Be Checked
	b. LostUrls.csv: A list of urls it either couldn’t find a match to, or there were more than one potential match.
	c. Probabilities.csv: A list of urls that the tool thinks ought to be turned into a catchall, and the number of times it saw that url appear during the scan.

What needs to be manually on local machines:
* file path to OldSiteUrls.CSV
* file path to NewSiteUrls.csv
* file path to OldSiteCatchAlls.csv
* file path to ExistingRedirects.csv