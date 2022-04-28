using Microsoft.VisualStudio.TestTools.UnitTesting;
using EmployeeTracking.App_Codes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.App_Codes.Tests
{
    [TestClass()]
    public class _leavs_leaves
    {
        [TestMethod()]
        public void IsSaturdayIsSaturday()
        {
            _leavs lv = new _leavs();
            var res = lv.IsSaturday(DateTime.Now.AddDays(1).AddHours(10), DateTime.Now.AddDays(2));

            Assert.AreEqual(true, res);

        }

        [TestMethod()]
        public void ActualDayCountActualDayCount()
        {
            _holidays hl = new _holidays();

            var bal = hl.ActualDayCount(DateTime.Now.AddDays(5), DateTime.Now.AddDays(10));
            Assert.Fail();
        }
    }
}