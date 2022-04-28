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

namespace SimLocationTrackingService
{
    class _locator
    {
        private static int _UserCount;

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

        public void CallLocationService()
        {
            Error_Log.er_log log = new Error_Log.er_log();
            svcref.GetLocationByNumberRequest obj = new svcref.GetLocationByNumberRequest();
            svcref.ProviderClient client = new svcref.ProviderClient();
            svcref.GetLocationByNumberRequest req = new svcref.GetLocationByNumberRequest();

            try
            {
                string userjs = CallRestMethod("http://sems.siot.lk/api/tracker/GetSimLocationUsers?key=77b1e470de2545d7bbf188072784f1de");
                var users = JsonConvert.DeserializeObject<List<UserLocationsViewModel>>(userjs);

                log.WriteLog("Users Recived : " + userjs.Count());
                GetLocatorService(client, users).Wait();
            }
            catch(Exception er)
            {
               log.WriteLog("CallLocationService : " + er);
            }

            
        }

        private static async Task GetLocatorService(svcref.ProviderClient client, List<UserLocationsViewModel> users)
        {
            List<RootObject> lst = new List<SimLocationTrackingService.RootObject>();

            foreach (var item in users)
            {
                if (item.MobileAccount != "N/A" || string.IsNullOrEmpty(item.MobileAccount))
                {
                    var res = client.GetLocationByNumber(item.MobileAccount, "123456", item.MobileNo);
                    res = "[" + res + "]";

                    var Location = JsonConvert.DeserializeObject<List<RootObject>>(res);

                    if (Location.FirstOrDefault().LONGITUDE != null)
                    {
                        Location.FirstOrDefault().User = item.UserId;
                        lst.Add(Location.FirstOrDefault());
                    }

                }

            }

            _UserCount = lst.Count;
            await UploadAsync(lst);
        }

        public static async Task UploadAsync(List<RootObject> lst)
        {
            using (var client = new HttpClient())
            {
                Error_Log.er_log log = new Error_Log.er_log();

                client.BaseAddress = new Uri("http://sems.siot.lk");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

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
