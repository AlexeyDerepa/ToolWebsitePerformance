using System.Linq;

namespace WSP.Entities.Processing
{
    public partial class ProcessingAddress
    {
        System.String patternSiteTarget = "<a.+?\"(.+?)\"";
        System.Collections.Generic.List<string> listVisitedLinks = new System.Collections.Generic.List<string>();
        System.Collections.Generic.List<System.Threading.Tasks.Task> listTasks = new System.Collections.Generic.List<System.Threading.Tasks.Task>();
        private System.Collections.Generic.List<string> CreateSitemape(SiteAddress sa)
        {
            SiteMape sm = new SiteMape { SiteAddress = sa, NameSateMape = "AUTO_GENERATED" };

            this._db.SiteMapes.Add(sm);
            this._db.SaveChanges();

            string target = sa.UrlAddress;

            DeepSarchSitemapFromCreateSitemape(Researching(target, target), target);

            SaveListPageThreadPool(listVisitedLinks, sm);


            return listVisitedLinks;
        }
        private System.Collections.Generic.List<string> CreateSitemapeParallel(SiteAddress sa)
        {
            SiteMape sm = new SiteMape { SiteAddress = sa, NameSateMape = "AUTO_GENERATED" };

            this._db.SiteMapes.Add(sm);
            this._db.SaveChanges();

            string target = sa.UrlAddress;

            DeepSarchSitemapFromCreateSitemapeParallel(Researching(target, target), target, sm);
            System.Threading.Tasks.Task.WaitAll(listTasks.ToArray());
            return listVisitedLinks;
        }


        private System.Collections.Generic.List<string> Researching(System.String address, System.String target)
        {
            System.String sitePage = RequestByUrl(address);
            System.Collections.Generic.List<string> listLink = SearchPattern(sitePage, patternSiteTarget, 1);
            listLink = listLink.Where(x => x.IndexOf(target) > -1).Select(x => x).ToList<string>();
            listLink = listLink.Distinct().ToList<string>();
            return listLink;
        }
        private System.Collections.Generic.List<string> DeepSarchSitemapFromCreateSitemape(System.Collections.Generic.List<string> listLinks, System.String target)
        {
            System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();
            bool flag = false;
            foreach (string link in listLinks)
            {
                if (!listVisitedLinks.Contains(link))
                {
                    listVisitedLinks.Add(link);
                    list.AddRange(Researching(link, target));
                    flag = true;
                }
            }
            list = list.Distinct().ToList<string>();
            if (flag)
            {
                DeepSarchSitemapFromCreateSitemape(list, target);
            }
            return listLinks;
        }
        private System.Collections.Generic.List<string> DeepSarchSitemapFromCreateSitemapeParallel(System.Collections.Generic.List<string> listLinks, System.String target, SiteMape sm)
        {
            listVisitedLinks.AddRange(listLinks);
            listTasks.Add(new System.Threading.Tasks.Task(() => { SaveListPageThreadPool(listLinks, sm); }));
            listTasks.Last().Start();

            System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();

            object objLock = new object();

            System.Threading.Tasks.Parallel.ForEach(listLinks, (address) =>
            {
                System.Collections.Generic.List<string> temp = Researching(address, target);
                lock (objLock)
                {
                    list.AddRange(temp);
                }
            });
            list = list.Distinct().ToList<string>();
            list = list.Except(listVisitedLinks).ToList<string>();
            if (list.Count > 0)
            {
                DeepSarchSitemapFromCreateSitemapeParallel(list, target, sm);
            }

            return listVisitedLinks;
        }

    }
}