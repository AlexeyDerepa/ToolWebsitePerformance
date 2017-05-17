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
        ProcessingAddress proc = new ProcessingAddress("DefaultConnection2");
        public ActionResult Index()
        {
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