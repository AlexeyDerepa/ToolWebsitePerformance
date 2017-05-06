using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WSP.Entities.Context
{
    public class ContextSite : DbContext
    {
        public DbSet<SiteAddress> SiteAddresses { get; set; }
        public DbSet<SiteMape> SiteMapes { get; set; }
        public DbSet<SitePage> SitePages { get; set; }

        public ContextSite(string connectionString) : base(connectionString) { }

    }
}