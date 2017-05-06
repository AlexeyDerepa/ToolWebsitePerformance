using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace WSP.Entities.Processing
{
    public partial class ProcessingAddress
    {
        public List<long[]> GetArrayForFlot(string guid)
        {
            var allInfo = this._db.SitePages
                .Where(x => x.SiteMape.SiteAddress.GuidString == guid)
                .ToList();
            var allInfo2 = allInfo
                .Select(x => new long[] { (long)x.Number, x.TimeMin.Value.Ticks })
                .ToList();

            return allInfo2.Count < 305 ? allInfo2 : allInfo2.GetRange(allInfo2.Count - 302, 300);
        }
        public async System.Threading.Tasks.Task<List<long[]>> GetArrayForFlotAsync(string guid)
        {
            var allInfo =await this._db.SitePages
                .Where(x => x.SiteMape.SiteAddress.GuidString == guid)
                .ToListAsync();
            var allInfo2 = allInfo
                .Select(x => new long[] { (long)x.Number, x.TimeMin.Value.Ticks })
                .ToList();

            return allInfo2.Count < 305 ? allInfo2 : allInfo2.GetRange(allInfo2.Count - 302, 300);

        }

    }
}