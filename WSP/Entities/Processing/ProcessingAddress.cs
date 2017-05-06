using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WSP.Entities.Context;

namespace WSP.Entities.Processing
{
    public partial class ProcessingAddress
    {
        /// <summary>
        /// The Context for connection to the data base and processing a data
        /// </summary>
        private ContextSite _db;

        private string _httpwebAddress = @"https?://(www\.)?([\w\d\-]+(\.[\w\d]+)+)";
        private string _webAddress = @"(www\.)?([\w\d\-]+(\.[\w\d]+)+)";
        private string _httpwebXml = @"https?[\w\d/\:\.\-_]+\.xml";
        private string _httpwebPage = @"<loc>([\w\W]+?)</loc>";

        private object _threadLock;

        private List<string> _listLinkToXmlFiles;
        private List<string> _listCheckedLink;

        private long _counterPages;
        private byte _counterSave;
        private byte _amountOfTests;



        public ProcessingAddress(string connectionString, byte counterSave = 20, byte amountOfTests = 5)
        {
            this._db = new ContextSite(connectionString);
            this._counterSave = counterSave;
            this._amountOfTests = amountOfTests;

            this._counterPages = 0;
            this._listLinkToXmlFiles = new List<string>();
            this._listCheckedLink = new List<string>();
            this._threadLock = new object();
        }


        /// <summary>
        /// point of enter for processing
        /// </summary>
        /// <param name="siteAddress"></param>
        /// <returns></returns>
        public ICollection<string> LookForSiteAddress(SiteAddress siteAddress)
        {
            siteAddress.UrlAddress = MathcAddress(siteAddress.UrlAddress, _httpwebAddress, 0);

            //check in the existence of the site
            if (SiteIPAddress(siteAddress.UrlAddress).Count == 0)
                return new List<string> { "site does not exist" };

            SiteAddress sa = new SiteAddress { UrlAddress = siteAddress.UrlAddress, GuidString = siteAddress.GuidString };

            //Save request by url
            this._db.SiteAddresses.Add(sa);
            this._db.SaveChanges();

            SearchSitePage(
                SearchSiteMap(siteAddress.UrlAddress),
                sa
                );

            var xml = this._db.SiteMapes.Where(x => x.SiteAddressId == sa.Id).Select(x => x.NameSateMape).ToList();

            List<string> list = new List<string> { "you can go to the history of requests", "URL address: ", sa.UrlAddress, "Found *.xml: " + xml.Count().ToString() };
            list.AddRange(xml);
            list.Add(_counterPages + " pages were found");
            return list;
        }

        private List<string> SearchPattern(string target, string pattern, int num = 0)
        {

            if (string.IsNullOrEmpty(target))
                return new List<string>();

            List<string> list = new List<string>();
            System.Text.RegularExpressions.Regex regEx = new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.MatchCollection matches = regEx.Matches(target);
            foreach (System.Text.RegularExpressions.Match m in matches)
            {
                list.Add(m.Groups[num].Value);
            }
            return list;
        }

        private string MathcAddress(string hostName, string pattern, int number)
        {
            System.Text.RegularExpressions.Regex regEx = new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Match match = regEx.Match(hostName);

            return match.Groups[number].Value;
        }

    }
}