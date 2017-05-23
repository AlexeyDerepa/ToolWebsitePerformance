using System.Linq;

namespace WSP.Entities.Processing
{
    public partial class ProcessingAddress
    {
        System.String patternSiteTarget = "<a\\s.*?href.*?=.*?[\"\'](.+?)[\"\']";
        
        //System.String patternSiteTarget = "<a\\s.*?href.*?=.*?\"(.+?)\"";
        
        System.Collections.Generic.List<string> listVisitedLinks;
        
        //System.Collections.Generic.List<System.Threading.Tasks.Task> listTasks = new System.Collections.Generic.List<System.Threading.Tasks.Task>();
        
        object objForLock = new object();
        
        private TargetSITE targetSiteObj;

        private void CreateSitemapeParallel(SiteAddress sa)
        {
            listVisitedLinks = new System.Collections.Generic.List<string>();

            targetSiteObj = new TargetSITE(sa.UrlAddress);

            SiteMape sm = new SiteMape { SiteAddress = sa, NameSateMape = "AUTO_GENERATED" };

            this._db.SiteMapes.Add(sm);

            this._db.SaveChanges();



            DeepSearchSitemapParallel(sm, Researching(targetSiteObj.WholeName));

            listVisitedLinks = null;
        }


        private System.Collections.Generic.List<string> Researching(System.String address)
        {
            System.String sitePage = RequestByUrl(address);
            System.Collections.Generic.List<string> listLink = SearchPattern(sitePage, patternSiteTarget, 1);
            listLink = AddressFiltr(listLink);
            return listLink;
        }

        private System.Collections.Generic.List<string> AddressFiltr(System.Collections.Generic.List<string> addresses)
        {
            //============================================(-1-)===============================================================

            addresses = addresses
                        .Where(x => !x.Contains("mailto:") && !x.Contains("file:") && !x.Contains("skype:") && !x.Contains("javascript:"))    //we deleted references to "mailto:" and "file:" and "skype:" and "javascript:"
                        .Select(x => x = deleteGetParam(x))                                                                                   //delete get params
                        .ToList<string>();

            //================================================================================================================
            //============================================(-2-)===============================================================
            //we selected references that starts with "//" and we added protocol http or https
            System.Collections.Generic.List<string> temp1 = addresses.Where(x => x.StartsWith("//"))
                                                            .Select(x => targetSiteObj.Protocol + x)
                                                            .ToList<string>();

            if (temp1.Count > 0)
                addresses = addresses.Where(x => !x.StartsWith("//")).ToList<string>();
            //================================================================================================================
            //============================================(-3-)===============================================================

            //we added to domain name selected root references 
            System.Collections.Generic.List<string> temp = addresses
                                                            .Where(x => x.StartsWith("/"))
                                                            .Select(x => targetSiteObj.WholeName + x)
                                                            .ToList<string>();


            //================================================================================================================
            //============================================(-4-)===============================================================
            //we selected references with domain names or similar 
            System.Collections.Generic.List<string> temp2 = addresses
                                                            .Where(x => isNecessarySite(x, targetSiteObj.JustNameSite))
                                                            .ToList<string>();

            //================================================================================================================
            //============================================(-5-)===============================================================

            //we united the lists 
            temp.AddRange(temp1);
            temp.AddRange(temp2);
            //================================================================================================================
            //============================================(-6-)===============================================================

            //we deleted unnecessary links to files
            temp = temp.Where(x => !isFile(x)).ToList<string>();
            //================================================================================================================
            //============================================(-7-)===============================================================

            //change special characters html to unicod characters
            temp = temp.Select(x => x = checkTheCharacters(x)).ToList<string>();

            //we deleted local links
            temp = temp.Select(x => x = deleteLocalLink2(x)).ToList<string>();

            //we removed duplicate strings
            temp = temp.Distinct().ToList<string>();

            //delete visited addresses
            temp = temp.Except(listVisitedLinks).ToList<string>();

            temp = temp.Where(x => x.Contains(targetSiteObj.WholeName)).ToList<string>();

            return temp;
        }

        #region RULES

        /// <summary>
        /// Determine whether this link is a file
        /// </summary>
        System.Func<string, bool> isFile = (a) =>
        {
            int firstDot = a.IndexOf(".");//find a first dot in string
            int lastDot = a.LastIndexOf(".");//find a last dot in string

            if (firstDot != lastDot)
            {
                int firstSlash = a.IndexOf("http://") == -1 ? 8 : 7;
                int nextSlash = a.IndexOf("/", firstSlash);

                if (nextSlash == -1)
                    nextSlash = a.Length;

                if (firstSlash < firstDot && lastDot < nextSlash)
                    return false;// false - если расширение относится к доменному имени или файлу типа *.html, *.htm, ...
            }
            else
                return false;// false - если расширение относится к доменному имени или файлу типа *.html, *.htm, ...


            if (a.IndexOf("htm", lastDot + 1) != -1)
                return false;

            return true; //true - если расширение относится к типу любыx файлов например *.pdf, *.doc, ...
        };

        System.Func<string, string, bool> isNecessarySite = (a, subStr) =>
        {
            int firstSlash = a.IndexOf("//");
            if (firstSlash == -1)
                return false;
            firstSlash += 2;

            int nextSlash = a.IndexOf("/", firstSlash);
            if (nextSlash == -1)
                nextSlash = a.Length;

            int indexSubString = a.IndexOf(subStr);
            if (indexSubString == -1 || indexSubString > nextSlash)
            {
                return false;
            }
            return true;
        };

        /// <summary>
        /// Delet local links
        /// </summary>
        System.Func<string, string> deleteLocalLink = (x) =>
        {
            int index = x.IndexOf("/#");
            return index == -1 ? x : x.Substring(0, index) + "/";
        };
        System.Func<string, string> deleteLocalLink2 = (x) =>
        {
            int index = x.IndexOf("#");
            if (index == -1 || x[index - 1] == '&')
                return x;

            if (x[index - 1] == '/')
                return x.Substring(0, index);
            else if (x.IndexOf("htm") > 0)
                return x.Substring(0, index);
            else
                return x.Substring(0, index) + "/";
        };
        System.Func<string, string> deleteGetParam = (x) =>
        {
            int index = x.IndexOf("?");
            if (index == -1)
                return x;
            return x.Substring(0, index);
        };

        System.Func<string, string> checkTheCharacters = (str) =>
        {

            int index = str.IndexOf("&#");
            if (index == -1) return str;

            string pattern = @"(&#\d+\;)";
            System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();
            System.Text.RegularExpressions.Regex regEx = new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.MatchCollection matches = regEx.Matches(str);
            foreach (System.Text.RegularExpressions.Match m in matches)
            {
                list.Add(m.Groups[1].Value);
            }

            System.IO.StringWriter myWriter = new System.IO.StringWriter();

            // Decode the encoded string.
            foreach (string item in list)
            {
                System.Web.HttpUtility.HtmlDecode(item, myWriter);
                str = str.Replace(item, myWriter.ToString());
            }

            return str;
        };
        #endregion

        private System.Collections.Generic.List<string> DeepSearchSitemapParallel(SiteMape sm, System.Collections.Generic.List<string> externalListLinks, System.Boolean flag = false, System.Int32 QUANTITY = 250)
        {
            //local list of links
            System.Collections.Generic.List<string> innerList = new System.Collections.Generic.List<string>();

            if (externalListLinks.Count > QUANTITY)
            {
                QueryLimiter(sm, externalListLinks.ToList<string>(), QUANTITY, innerList);
            }
            else
            {
                ExecuteLimitedQuerys(sm, externalListLinks, innerList);
            }

            listVisitedLinks.AddRange(externalListLinks);
            listVisitedLinks = listVisitedLinks.Distinct().ToList<string>();

            //delete rows that listVisitedLinks has
            innerList = innerList.Except(listVisitedLinks).ToList<string>();

            if (flag == false && innerList.Count > 0)
            {
                innerList.AddRange(DeepSearchSitemapParallel(sm, innerList, flag, QUANTITY).Except(innerList));
                innerList = innerList.Except(listVisitedLinks).ToList<string>();
            }

            return innerList;
        }

        private void ExecuteLimitedQuerys(SiteMape sm, System.Collections.Generic.List<string> externalListLinks, System.Collections.Generic.List<string> innerList)
        {
            //CreateTask(externalListLinks);
            SaveListPageThreadPool(externalListLinks, sm);

            System.Threading.Tasks.Parallel.For(0, externalListLinks.Count, (INDEX) =>
            {
                string address = externalListLinks[INDEX];
                System.Collections.Generic.List<string> temp = Researching(address);
                lock (objForLock)
                {
                    innerList.AddRange(temp.Except(innerList));
                }
            });
        }
        private void QueryLimiter(SiteMape sm, System.Collections.Generic.List<string> externalListLinks, System.Int32 QUANTITY, System.Collections.Generic.List<string> innerList)
        {
            int amount = QUANTITY;
            do
            {
                innerList.AddRange(DeepSearchSitemapParallel(sm, externalListLinks.GetRange(0, amount), flag: true));

                innerList = innerList.Except(listVisitedLinks).ToList<string>();

                externalListLinks = externalListLinks.Except(listVisitedLinks).ToList<string>();

                if (amount > externalListLinks.Count)
                {
                    amount = externalListLinks.Count;
                    if (amount <= 0)
                    {
                        break;
                    }
                }

            } while (true);
        }



        class TargetSITE
        {
            public string WholeName { get; private set; }
            public string Protocol { get; private set; }
            public string JustNameSite { get; private set; }
            public string JustNameAndDomainSite { get; private set; }

            public TargetSITE(string ts)
            {
                ts = ts.Trim();
                this.WholeName = ts.EndsWith("/") ? ts.Substring(0, ts.Length - 1) : ts;
                this.Protocol = GetProtocol();
                this.JustNameAndDomainSite = GetJustNameAndDomainSite();
                this.JustNameSite = GetJustNameWithoutDomainOfSite();
            }

            private string GetJustNameWithoutDomainOfSite()
            {
                return this.JustNameAndDomainSite.Substring(0, JustNameAndDomainSite.LastIndexOf("."));
            }

            private string GetJustNameAndDomainSite()
            {
                if (this.WholeName.LastIndexOf("/") == -1)
                    return this.WholeName.Substring(Protocol.Length + 3, this.WholeName.Length - 1);
                return this.WholeName.Substring(Protocol.Length + 3, this.WholeName.Length - Protocol.Length - 3); ;
            }
            string GetProtocol()
            {
                return this.WholeName.Substring(0, this.WholeName.IndexOf("//") - 1);
            }
        }


    }
}