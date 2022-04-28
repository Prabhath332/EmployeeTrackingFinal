using LocationService.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Web;

namespace LocationService.App_Codes
{
    public class _service
    {
        private static int _UserCount;
        private static TimeZoneInfo timezone = TimeZoneInfo.FindSystemTimeZoneById("Sri Lanka Standard Time");

        public int UserCount
        {
            get
            {
                return _UserCount;
            }
            set
            {
                _UserCount = value;
            }
        }

        public void svcMethod()
        {
            System.Timers.Timer t = new System.Timers.Timer();
            t.Elapsed += new System.Timers.ElapsedEventHandler(TimerWorker);
            //t.Interval = 1800000;
            t.Interval = 900000;
            t.Enabled = true;
            t.AutoReset = true;
            t.Start();
        }

        private void TimerWorker(object sender, ElapsedEventArgs e)
        {
            CallLocationService();
        }

        public void CallLocationService()
        {
            Error_Log.er_log log = new Error_Log.er_log();
            svcref.ProviderClient client = new svcref.ProviderClient();

            try
            {
                string userjs = CallRestMethod("http://sems.siot.lk/api/tracker/GetSimLocationUsers?key=77b1e470de2545d7bbf188072784f1de");
                var users = JsonConvert.DeserializeObject<List<UserLocationsViewModel>>(userjs);

                log.WriteLog("Users Recived : " + users.Count());
                GetLocatorService(client, users).Wait();
            }
            catch (Exception er)
            {
                log.WriteLog("CallLocationService : " + er);
            }


        }

        private static async Task GetLocatorService(svcref.ProviderClient client, List<UserLocationsViewModel> users)
        {
            Error_Log.er_log log = new Error_Log.er_log();

            try
            {
                List<RootObject> lst = new List<LocationService.Models.RootObject>();
                //var res1 = client.GetLocationByNumber("17042031", "123456", "0713073946");
                string userRes = "";
                foreach (var item in users)
                {
                    if (item.MobileAccount != "N/A" || string.IsNullOrEmpty(item.MobileAccount))
                    {
                        var res = client.GetLocationByNumber(item.MobileAccount, "123456", item.MobileNo);
                        res = "[" + res + "]";
                        userRes += res;
                        
                        var Location = JsonConvert.DeserializeObject<List<RootObject>>(res);

                        if (Location.FirstOrDefault().LONGITUDE != null)
                        {
                            Location.FirstOrDefault().User = item.UserId;
                            //Location.FirstOrDefault().da
                            lst.Add(Location.FirstOrDefault());
                        }

                    }

                }

                _UserCount = lst.Count;
                log.WriteLog("Responce For User : " + userRes);
                await UploadAsync(lst);
            }
            catch(Exception er)
            {
                log.WriteLog("GetLocatorService Method : " + er.Message);
            }
            
        }

        public static async Task UploadAsync(List<RootObject> lst)
        {
            using (var client = new HttpClient())
            {
                Error_Log.er_log log = new Error_Log.er_log();

                client.BaseAddress = new Uri("http://sems.siot.lk");
                client.DefaultRequestHeaders.Accept.Clear();
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                StringContent content = new StringContent(JsonConvert.SerializeObject(lst), Encoding.UTF8, "application/json");

                try
                {
                    HttpResponseMessage response = await client.PostAsync("api/tracker/GetSimLocations", content);

                    if (response.IsSuccessStatusCode)
                    {
                        log.WriteLog("Success POST");
                        // "Success", MessageBoxButtons.OK);
                    }
                    else
                    {
                        log.WriteLog("REsponce : " + response.ReasonPhrase);
                    }
                }
                catch (Exception er)
                {
                    log.WriteLog("UploadAsync : " + er);
                }
            }
        }

        public static string CallRestMethod(string url)
        {
            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(url);
            webrequest.Method = "GET";
            webrequest.ContentType = "application/x-www-form-urlencoded";
            HttpWebResponse webresponse = (HttpWebResponse)webrequest.GetResponse();
            Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader responseStream = new StreamReader(webresponse.GetResponseStream(), enc);
            string result = string.Empty;
            result = responseStream.ReadToEnd();
            webresponse.Close();
            return result;
        }
    }
}