using EmployeeTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeTracking.App_Codes
{
    public class _siteLocations
    {
        public List<SiteLocation> GetSiteLocations()
        {
            try
            {
                using (ApplicationDbContext db = new Models.ApplicationDbContext())
                {
                    return db.SiteLocations.ToList();
                }

            }
            catch (Exception er)
            {
                return null;
            }
        }

        //public SiteLocation GetSiteLocation(int LocationId)
        //{
        //    try
        //    {
        //        using (ApplicationDbContext db = new Models.ApplicationDbContext())
        //        {
        //            return db.SiteLocations.ToList();
        //        }

        //    }
        //    catch (Exception er)
        //    {
        //        return null;
        //    }
        //}
    }
}