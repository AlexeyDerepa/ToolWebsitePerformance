using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSP.Entities
{
    public class SiteMape
    {
        public int Id { get; set; }
        public string NameSateMape { get; set; }
        public TimeSpan? TimeMin { get; set; }
        public TimeSpan? TimeMax { get; set; }
        public TimeSpan? TimeAverage { get; set; }
        public string Contents { get; set; }


        public int? SiteAddressId { get; set; }
        public virtual SiteAddress SiteAddress { get; set; }

        public virtual ICollection<SitePage> SitePages { get; set; }
        public SiteMape()
        {
            SitePages = new List<SitePage>();
        }

    }
}