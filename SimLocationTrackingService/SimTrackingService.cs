using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;


namespace SimLocationTrackingService
{
    public partial class SimTrackingService : ServiceBase
    {
        private System.Timers.Timer timer = null;

        public SimTrackingService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
           
           Error_Log.er_log log = new Error_Log.er_log();

            timer = new System.Timers.Timer();
            timer.Interval = 900000;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
            timer.Enabled = true;

            log.WriteLog("Service Started", DateTime.Now.ToLongTimeString());
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _locator lc = new SimLocationTrackingService._locator();
            lc.CallLocationService();

            Error_Log.er_log log = new Error_Log.er_log();
            log.WriteLog("Location Service", "Locations Returned For " + lc.UserCount + " Users");
        }

        protected override void OnStop()
        {
            timer.Enabled = false;
            Error_Log.er_log log = new Error_Log.er_log();
            log.WriteLog("Service Stop", DateTime.Now.ToLongTimeString());
        }
    }
}
