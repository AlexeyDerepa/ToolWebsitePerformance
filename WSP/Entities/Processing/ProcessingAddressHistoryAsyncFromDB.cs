using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;


namespace WSP.Entities.Processing
{
    public partial class ProcessingAddress
    {
        public  List<SiteAddress> HistoryRequests()
        {
            List<SiteAddress> allInfo;
            try
            {
                allInfo =  this._db.SiteAddresses.OrderByDescending(x => x.Id).ToList();
            }
            catch (Exception ex)
            {
                allInfo = new List<SiteAddress>();
            }
            return allInfo;
        }
        public async System.Threading.Tasks.Task<List<SiteAddress>> HistoryRequestsAsync()
        {
            List<SiteAddress> allInfo;
            try
            {
                allInfo = await this._db.SiteAddresses.OrderByDescending(x => x.Id).ToListAsync();
            }
            catch (Exception ex)
            {
                allInfo = new List<SiteAddress>();
            }
            return allInfo;
        }
        public List<SiteMape> HistoryXml(int id)
        {
            List<SiteMape> allInfo;
            try
            {
                allInfo =  this._db.SiteMapes.Where(x=>x.SiteAddress.Id == id).OrderByDescending(x=>x.TimeMin).ToList();
            }
            catch (Exception ex)
            {
                allInfo = new List<SiteMape>();
            }
            return allInfo;
        }
        public async System.Threading.Tasks.Task<List<SiteMape>> HistoryXmAsyncl(int id)
        {
            List<SiteMape> allInfo;
            try
            {
                allInfo = await this._db.SiteMapes.Where(x => x.SiteAddress.Id == id).OrderByDescending(x => x.TimeMin).ToListAsync();
            }
            catch (Exception ex)
            {
                allInfo = new List<SiteMape>();
            }
            return allInfo;
        }
        public List<SitePage> SearchSitePage(int id)
        {
            List<SitePage> allInfo;
            int? som = id;
            try
            {
                allInfo = this._db.SitePages.Where(x => x.SiteMapeId == id).OrderByDescending(x => x.TimeMin).Select(x => x).ToList();
            }
            catch (Exception ex)
            {
                allInfo = new List<SitePage>();
            }
            return allInfo;
        }
        public async System.Threading.Tasks.Task<List<SitePage>> SearchSitePageAsync(int id)
        {
            List<SitePage> allInfo;
            try
            {
                allInfo = await this._db.SitePages.Where(x => x.SiteMapeId == id).OrderByDescending(x => x.TimeMin).ToListAsync();
            }
            catch (Exception ex)
            {
                allInfo = new List<SitePage>();
            }
            return allInfo;
        }
        public async System.Threading.Tasks.Task<List<SitePage>> SpecificPartPages(int id)
        {
            List<SitePage> allInfo;
            try
            {
                allInfo = await (from page in this._db.SitePages
                                 join xml in this._db.SiteMapes on page.SiteMapeId equals xml.Id
                                 where page.SiteMapeId == id
                                 orderby page.TimeMin
                                 select page).OrderByDescending(x => x.TimeMin).ToListAsync();
            }
            catch (Exception ex)
            {
                allInfo = new List<SitePage>();
            }
            return allInfo;
        }
    }
}