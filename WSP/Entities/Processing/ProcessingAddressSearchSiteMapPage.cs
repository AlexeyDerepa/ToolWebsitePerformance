using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSP.Entities.Processing
{
    public partial class ProcessingAddress
    {
        private int SearchSitePage(List<string> listSiteMap, SiteAddress sa)
        {
            List<TimeSpan> lts = new List<TimeSpan>();
            List<string> l_s_m;//list site mape
            string str = "";
            object o = new object();
            int countFoundPages = 0;

            foreach (string xmlPage in listSiteMap)
            {
                System.Threading.Tasks.Parallel.For(0, this._amountOfTests, (x) =>
                {
                    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                    sw.Reset();
                    sw.Start();
                    str = RequestByUrl(xmlPage);
                    sw.Stop();
                    lock (o)
                    {
                        lts.Add(sw.Elapsed);
                    }
                });

                l_s_m = SearchPattern(str, _httpwebPage, 1);

                if (l_s_m.Count == 0)
                    continue;
                countFoundPages++;
                SiteMape sm = new SiteMape { SiteAddress = sa, NameSateMape = xmlPage, TimeMax = lts.Max(), TimeMin = lts.Min(), TimeAverage = new TimeSpan((long)lts.Average(x => x.Ticks)) };

                this._db.SiteMapes.Add(sm);
                this._db.SaveChanges();

                SaveListPageThreadPool(l_s_m, sm);
            }
            return countFoundPages;
        }

        private void SaveListPageThreadPool(List<string> list, SiteMape sm)
        {
            if (list.Count == 0)
                return;

            int count = 0;
            System.Threading.Tasks.Parallel.ForEach(list, (page) => {
                                List<TimeSpan> lts = new List<TimeSpan>();

                                object objLock = new object();

                                System.Threading.Tasks.Parallel.For(0, this._amountOfTests, (x) =>
                                {
                                    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                                    sw.Reset();
                                    sw.Start();
                                    RequestByUrl(page);
                                    sw.Stop();
                                    lock (objLock)
                                    {
                                        lts.Add(sw.Elapsed);
                                    }
                                });
                
                                lock (this._threadLock)
                                {
                                    count++;
                                    this._db.SitePages.Add(new SitePage { SiteMape = sm, NameSitePage = page, TimeMax = lts.Max(), TimeMin = lts.Min(), TimeAverage = new TimeSpan((long)lts.Average(x => x.Ticks)), Number = _counterPages++ });
                                    if (_counterSave <= count)
                                    {
                                        this._db.SaveChanges();
                                        count = 0;
                                    }
                                }
            });

            lock (this._threadLock)
            {
                    this._db.SaveChanges();
            }

        }

        private List<string> SearchSiteMap(string hostName)
        {

            List<string> listSiteMap = new List<string>();
           
            listSiteMap.AddRange(SearchPattern(RequestByUrl(hostName + "/robots.txt"), _httpwebXml));    //search in robots.txt entries about files with *.xml
            listSiteMap.AddRange(SearchPattern(RequestByUrl(hostName + "/sitemap.xml"), _httpwebXml));   //search in sitemap.xml entries about files with *.xml 
            listSiteMap.Add(hostName + "/sitemap.xml");                                                     //add a new path */sitemap.xml
            listSiteMap = listSiteMap.Distinct().ToList<string>();                                          //delete repeated items

            listSiteMap = SearchDeepSiteMap(listSiteMap);

            return listSiteMap;
        }


        private List<string> SearchDeepSiteMap(List<string> listSiteMap)
        {
            List<string> additionXml = listSiteMap.ToList();
            List<string> infos = new List<string>();
            bool flag = false;

            System.Threading.Tasks.Parallel.For(0,listSiteMap.Count, (id) => {
                string Xml = listSiteMap[id];

                if (this._listCheckedLink.Contains(Xml) == false)
                {
                    this._listCheckedLink.Add(Xml);

                    List<string> inf = SearchPattern(RequestByUrl(Xml), _httpwebXml);
                    if (inf.Count != 0)
                        lock (this._threadLock)
                        {
                            if (this._listLinkToXmlFiles.Contains(Xml) == false)
                            {
                                this._listLinkToXmlFiles.Add(Xml);
                                additionXml.Remove(Xml);
                                additionXml.AddRange(inf);
                                flag = true;
                            }
                        }
                }
            });

            additionXml = additionXml.Distinct().ToList<string>();
            if (flag)
            {
                additionXml = SearchDeepSiteMap(additionXml);
            }
            return additionXml;
        }

    }
}