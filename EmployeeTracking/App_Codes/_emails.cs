using EmployeeTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Web;

namespace EmployeeTracking.App_Codes
{
    public class _emails
    {
        public bool PasswordResetMail(UserProfileViewModel user)
        {
            try
            {
                if(!string.IsNullOrEmpty(user.ApplicationUser.Email))
                {
                    SmtpClient smtpClient = new SmtpClient();

                    smtpClient.Port = 25;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new System.Net.NetworkCredential("admin@sems.siot.lk", "s3M57#s$S");
                    smtpClient.Host = "mail.sems.siot.lk";

                    MailMessage NetMail = PasswordReset();
                    NetMail.IsBodyHtml = true;

                    NetMail.From = new MailAddress("admin@sems.siot.lk", "SEMS");
                    NetMail.To.Add(new MailAddress(user.ApplicationUser.Email));
                    NetMail.Subject = "Password Reset";

                    Thread t1 = new Thread(delegate ()
                    {

                        smtpClient.Send(NetMail);

                    });

                    t1.Start();


                    return true;
                }
                else
                {
                    return false;
                }
                                
            }
            catch
            {
                return false;
            }
        }

        public MailMessage PasswordReset()
        {
            MailMessage NetMail = new MailMessage();
            NetMail.IsBodyHtml = true;

            NetMail.Body = "<html><body><div style=\"width: 600px; padding: 10px; font-family: 'Segoe UI', Calibri, Arial, Helvetica, sans-serif; font-size: 12px;\">" +
                        "<div style=\"padding: 10px\">" +
                            "<h2>Password Reset</h2><hr />" +
                            "Your SEMS Password Has Been Reset to Default.Please Change the Default Password to a More Secure Password Once You Login." +
                            "<br />" +
                            "<b>Default Password : </b>abc@123" +
                            "<br />" +
                            "<br />" +
                            "SEMS - Admin" +
                        "</div>" +
                    "</div></body></html>";

            return NetMail;
        }
    }
}