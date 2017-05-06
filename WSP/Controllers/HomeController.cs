using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WSP.Entities;
using WSP.Entities.Processing;

namespace WSP.Controllers
{
    public class HomeController : Controller
    {
        //ContextSite db = new ContextSite("WSP");
        ProcessingAddress proc = new ProcessingAddress("WSP");
        public ActionResult Index()
        {
            //db.SiteAddresses.Add(new SiteAddress { UrlAddress = "https://translate.yandex.ru"});
            //db.SaveChanges();
            //proc.LookForSiteAddress(new SiteAddress { UrlAddress = "https://www.google.com.ua/se" });
            return View();
        }
        public ActionResult SearchDeepSiteMape(SiteAddress hostName)
        {
            return PartialView(proc.LookForSiteAddress(hostName));
        }

        [HttpPost]
        public ActionResult Ping(WSP.Entities.SiteAddress hostName)
        {
            return PartialView(proc.Ping(hostName.UrlAddress));
        }
    }
}