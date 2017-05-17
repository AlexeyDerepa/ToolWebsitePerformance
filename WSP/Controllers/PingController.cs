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
    public class PingController : ApiController
    {
        ProcessingAddress proc = new ProcessingAddress("DefaultConnection2");

        public IEnumerable<string> POST([FromBody] SiteAddress hostName)
        {
            return proc.Ping(hostName.UrlAddress);
        }
    }
}
