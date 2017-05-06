using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSP.Entities
{
    public class SitePage
    {
        public int Id { get; set; }
        public long? Number { get; set; }
        public string NameSitePage { get; set; }
        public TimeSpan? TimeMin { get; set; }
        public TimeSpan? TimeMax { get; set; }
        public TimeSpan? TimeAverage { get; set; }

        public int? SiteMapeId { get; set; }
        public virtual SiteMape SiteMape { get; set; }
    }
}