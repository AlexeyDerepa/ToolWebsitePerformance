using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WSP.Entities.Context
{
    public class ContextSiteInitializer : CreateDatabaseIfNotExists<ContextSite>
    //public class ContextSiteInitializer : DropCreateDatabaseAlways<ContextSite>
    //public class ContextSiteInitializer : DropCreateDatabaseIfModelChanges<ContextSite>
    {
        protected override void Seed(ContextSite db)
        {
            base.Seed(db);
        }
    }
}