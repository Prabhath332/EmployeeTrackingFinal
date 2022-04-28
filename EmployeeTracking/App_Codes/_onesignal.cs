using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EmployeeTracking.Models;

using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace EmployeeTracking.App_Codes {
    public class _onesignal {
        public Boolean SaveNewUser(String UserId,String OnesignalId) {
            try {
                using (ApplicationDbContext db = new ApplicationDbContext()) {
                    //var user = db.OneSignal.Where(m => m.UserId == UserId && m.OnesignalId == OnesignalId).FirstOrDefault();
                    var user = db.OneSignal.Where(m => m.UserId == UserId).FirstOrDefault();
                    if (user == null) {
                        OneSignal model = new OneSignal();
                        model.UserId = UserId;
                        model.OnesignalId = OnesignalId;
                        model.Status = "Acticve";
                        db.OneSignal.Add(model);
                        db.SaveChanges();
                    }
                    else
                    {
                        user.OnesignalId = OnesignalId;
                        db.SaveChanges();
                    }
                    return true;
                }
            }catch(Exception ex) {
                return false;
            }
        }

        public Boolean Removeuser(String UserId, String OnesignalId) {
            try {
                using (ApplicationDbContext db = new ApplicationDbContext()) {
                    var user = db.OneSignal.Where(m => m.UserId == UserId && m.OnesignalId == OnesignalId).FirstOrDefault();
                    if (user != null) {
                        //OneSignal model = new OneSignal();
                        //model.UserId = UserId;
                        //model.OnesignalId = OnesignalId;
                        //model.Status = "Acticve";
                        //db.OneSignal.Add(model);
                        db.OneSignal.Remove(user);
                        db.SaveChanges();
                    }
                    return true;
                }
            } catch (Exception ex) {
                return false;
            }
        }

        public String PushNotification(String[] PlayerIds,String Title,String Description) {
            var request = WebRequest.Create("https://onesignal.com/api/v1/notifications") as HttpWebRequest;

            request.KeepAlive = true;
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";

            request.Headers.Add("authorization", "Basic MTY5OGEyNzYtNWI3MS00ODQ2LWE4YWItNzU1YzlmYTE5MTY2");
            var objData = new { notiType = Title };
            var serializer = new JavaScriptSerializer();
            var obj = new
            {               
                app_id = "f70012fb-5244-4126-8520-48c3ce7417af",
                contents = new { en = Description },
                include_player_ids = PlayerIds,
                Title= Title,
                data= objData
            };
            var param = serializer.Serialize(obj);
            byte[] byteArray = Encoding.UTF8.GetBytes(param);

            string responseContent = null;

            try
            {
                using (var writer = request.GetRequestStream())
                {
                    writer.Write(byteArray, 0, byteArray.Length);
                }

                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseContent = reader.ReadToEnd();
                    }
                }
                return "Done " + responseContent;
            }
            catch (WebException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());
                return "OneSignal api " + ex.Message + " responce " + responseContent;
            }
            
        }
    }
}