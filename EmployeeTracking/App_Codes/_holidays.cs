using EmployeeTracking.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EmployeeTracking.App_Codes
{
    public class _holidays
    {
        public List<Holiday> GetHolidays(int Year = 0, int Month = 0)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    if(Year > 0 && Month == 0)
                    {
                        return db.Holidays.Where(x => x.Date.Year == Year).ToList();
                    }
                    else if(Year == 0 && Month > 0)
                    {
                        int lastDate = DateTime.DaysInMonth(DateTime.Now.Year, Month);
                        DateTime firstDayInMonth = new DateTime(DateTime.Now.Year, Month, 1);
                        DateTime lastDayInMonth = new DateTime(DateTime.Now.Year, Month, lastDate);
                        return db.Holidays.Where(x => x.Date >= firstDayInMonth && x.Date <= lastDayInMonth).ToList();
                    }
                    else if (Year > 0 && Month > 0)
                    {
                        int lastDate = DateTime.DaysInMonth(Year, Month);
                        DateTime firstDayInMonth = new DateTime(Year, Month, 1);
                        DateTime lastDayInMonth = new DateTime(Year, Month, lastDate);
                        return db.Holidays.Where(x => x.Date >= firstDayInMonth && x.Date <= lastDayInMonth).ToList();
                    }
                    else
                    {
                        return db.Holidays.ToList();
                    }
                    
                }
            }
            catch
            {
                return null;
            }
        }

        public bool SaveHolidays(List<Holiday> lst)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    foreach(var item in lst)
                    {
                        var extDate = db.Holidays.Where(x => x.Date == item.Date).FirstOrDefault();

                        if(extDate != null)
                        {
                            extDate.Description = item.Description;
                            db.SaveChanges();
                        }
                        else
                        {
                            db.Holidays.Add(item);
                            db.SaveChanges();
                        }
                    }

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool IsHoliday(DateTime date)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var holiday = db.Holidays.Where(x => DbFunctions.TruncateTime(x.Date) == DbFunctions.TruncateTime(date)).FirstOrDefault();
                    var res = false;

                    if(holiday != null)
                    {
                        res = true;
                    }

                    return res;
                }
            }
            catch(Exception er)
            {
                return false;
            }
        }

        public double ActualDayCount(DateTime from, DateTime to)
        {
            var dateList = Enumerable.Range(0, 1 + to.Subtract(from).Days).Select(offset => from.AddDays(offset)).ToList();
            double dayCount = 0;
            _leavs lv = new _leavs();

            foreach(var item in dateList)
            {
                if(!IsHoliday(item))
                {
                    dayCount += 1;
                    //if(item.DayOfWeek == DayOfWeek.Saturday)
                    //{
                    //    dayCount += 0.5;
                    //}
                    //else if (item.DayOfWeek != DayOfWeek.Sunday)
                    //{
                    //    dayCount += 1;
                    //}

                }
            }
            return dayCount;
        }

        internal bool DeleteHoliday(int holidayId)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var holiday = db.Holidays.Where(x => x.Id == holidayId).FirstOrDefault();

                    db.Holidays.Remove(holiday);
                    db.SaveChanges();

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}