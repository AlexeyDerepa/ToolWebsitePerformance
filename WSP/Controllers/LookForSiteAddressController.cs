using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WSP.Entities;
using WSP.Entities.Processing;

namespace WSP.Controllers
{
    public class LookForSiteAddressController : ApiController
    {
        ProcessingAddress proc = new ProcessingAddress("DefaultConnection2");
        public IEnumerable<SiteAddress> Get()
        {
            return proc.HistoryRequests().Select(x => new SiteAddress { Id = x.Id, UrlAddress = x.UrlAddress });
        }
        public IEnumerable<long[]> Get(string guid)
        {
            return proc.GetArrayForFlot(guid);
        }

        public IEnumerable<string> POST([FromBody] SiteAddress hostName)
        {
            return proc.LookForSiteAddress(hostName);
        }

        public  IEnumerable<SiteMape> Delete(int id)
        {
            return proc.HistoryXml(id).Select(x => new SiteMape { Id = x.Id, TimeMin = x.TimeMin, TimeMax = x.TimeMax, NameSateMape = x.NameSateMape }); ;
        }

        public IEnumerable<SitePage> Put(int id, [FromBody] SiteAddress hostName)
        {
            return proc.SearchSitePage(id).Select(x => new SitePage{Id = x.Id, TimeMin = x.TimeMin, TimeMax = x.TimeMax, NameSitePage = x.NameSitePage, Number = x.Number});
        }

    }
}
