using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EmployeeTracking.Models;
using System.IO;

namespace EmployeeTracking.App_Codes {
    public class _news {
        private static TimeZoneInfo timezone = TimeZoneInfo.FindSystemTimeZoneById("Sri Lanka Standard Time");

        public List<NewsViewModel> GetNews(string UserId)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var newslst = db.News.OrderByDescending(m => m.Id).Take(20).ToList();
                    _usermanager um = new _usermanager();

                    List<NewsViewModel> newsList = new List<NewsViewModel>();
                    foreach (var item in newslst)
                    {
                        var user = um.GetUser(item.UserId);
                        NewsViewModel nw = new NewsViewModel();
                        nw.Id = item.Id;
                        nw.DateStr = item.Date.ToString("dd/MM/yy");
                        nw.Date = item.Date;
                        nw.Title = item.Title;
                        nw.Description = item.Description;
                        nw.Venu = item.Venu;
                        nw.LikeCount = item.LikeCount;
                        nw.Username = user.UserProfiles.FirstName;
                        nw.UserId = item.UserId;

                        if(string.IsNullOrEmpty(user.UserProfiles.Image))
                        {
                            nw.UserImage = "Content/Images/avatar5.png";
                        }
                        else
                        {
                            nw.UserImage = user.UserProfiles.Image;
                        }
                        

                        var islike = db.NewsLikes.Where(m => m.NewsId == item.Id && m.UserId == UserId).FirstOrDefault();
                        if (islike != null)
                        {
                            nw.isLike = "like";
                        }
                        else
                        {
                            nw.isLike = "unlike";
                        }

                        List<imagesliest> imglst = new List<imagesliest>();

                        if(!string.IsNullOrEmpty(item.FolderPath))
                        {
                            DirectoryInfo di = new DirectoryInfo(HttpContext.Current.Server.MapPath(item.FolderPath));
                            FileInfo[] rgFiles = di.GetFiles();
                            if (rgFiles != null)
                            {
                                //int imagecout = 0;
                                foreach (var imageitem in rgFiles)
                                {
                                    var img = imageitem.FullName.Split(new string[] { "Content" }, StringSplitOptions.None)[1];
                                    imglst.Add(new Models.imagesliest { Imageb64 = "Content" + img });

                                }
                                nw.Image64list = imglst;
                            }
                        }
                        
                        newsList.Add(nw);
                    }
                    return newsList;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool DeleteNews(int newsId)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var nw = db.News.Where(x => x.Id == newsId).FirstOrDefault();
                    var nwLikes = db.NewsLikes.Where(x => x.NewsId == newsId).ToList();
                    var nwComm = db.NewsComments.Where(x => x.NewsId == newsId).ToList();

                    db.NewsComments.RemoveRange(nwComm);
                    db.NewsLikes.RemoveRange(nwLikes);
                    db.SaveChanges();

                    db.News.Remove(nw);
                    db.SaveChanges();

                    var folderPath = nw.FolderPath;

                    if (Directory.Exists(HttpContext.Current.Server.MapPath(folderPath)))
                    {
                        Directory.Delete(HttpContext.Current.Server.MapPath(folderPath), true);
                    }
                }

                return true;
            }
            catch(Exception er)
            {
                return false;
            }
        }

        public bool DeleteNewsImage(int newsId, string imgName)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var nw = db.News.Where(x => x.Id == newsId).FirstOrDefault();
                    
                    if(nw != null)
                    {
                        var folderPath = nw.FolderPath;
                        var filePath = HttpContext.Current.Server.MapPath(Path.Combine(folderPath, imgName));

                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
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

        public News GetNews(int NewsId)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    News nw = new News();
                    var news = db.News.Where(x => x.Id == NewsId).FirstOrDefault();

                    List<imagesliest> imglst = new List<imagesliest>();

                    DirectoryInfo di = new DirectoryInfo(HttpContext.Current.Server.MapPath(news.FolderPath));
                    FileInfo[] rgFiles = di.GetFiles();
                    if (rgFiles != null)
                    {
                        foreach (var imageitem in rgFiles)
                        {
                            var img = imageitem.FullName.Split(new string[] { "Content" }, StringSplitOptions.None)[1];
                            var imgName = Path.GetFileName(imageitem.FullName);
                            imglst.Add(new Models.imagesliest { Imageb64 = "Content" + img, ImageName = imgName });

                        }

                        news.Image64list = imglst;
                    }

                    return news;
                }
            }
            catch
            {
                News nw = new News();
                nw.Image64list = new List<imagesliest>();
                return nw;
            }
        }

        public bool SaveNews(News news, IList<HttpPostedFileBase> files, string UserId)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var nw = db.News.Where(x => x.Id == news.Id).FirstOrDefault();

                    if(nw != null)
                    {
                        if(!string.IsNullOrEmpty(nw.FolderPath))
                        {
                            string folderPath = nw.FolderPath;

                            if (files[0].ContentLength > 0)
                            {
                                foreach (var item in files)
                                {
                                    string imagename = "newsImage_" + DateTime.Now.Ticks + Path.GetExtension(item.FileName);
                                    item.SaveAs(Path.Combine(HttpContext.Current.Server.MapPath(folderPath), imagename));
                                }

                            }

                            news.FolderPath = nw.FolderPath;
                        }
                        else
                        {
                            string folderName = "News" + DateTime.Now.Ticks + Guid.NewGuid();
                            string folderPath = "~/Content/NewsUpload/" + folderName;

                            if (files != null)
                            {
                                if (!Directory.Exists(HttpContext.Current.Server.MapPath(folderPath)))
                                {
                                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath(folderPath));
                                }

                                foreach (var item in files)
                                {
                                    string imagename = "newsImage_" + DateTime.Now.Ticks + Path.GetExtension(item.FileName);
                                    item.SaveAs(Path.Combine(HttpContext.Current.Server.MapPath(folderPath), imagename));
                                }

                                news.FolderPath = folderPath;
                            }
                        }

                        news.Date = nw.Date;
                        news.UserId = nw.UserId;
                        news.LikeCount = nw.LikeCount;
                        db.Entry(nw).CurrentValues.SetValues(news);
                        db.SaveChanges();

                    }
                    else
                    {
                        string folderName = "News" + DateTime.Now.Ticks + Guid.NewGuid();
                        string folderPath = "~/Content/NewsUpload/" + folderName;

                        if (files[0].ContentLength > 0)
                        {
                            if (!Directory.Exists(HttpContext.Current.Server.MapPath(folderPath)))
                            {
                                Directory.CreateDirectory(HttpContext.Current.Server.MapPath(folderPath));
                            }

                            foreach(var item in files)
                            {
                                string imagename = "newsImage_" + DateTime.Now.Ticks + Path.GetExtension(item.FileName);
                                item.SaveAs(Path.Combine(HttpContext.Current.Server.MapPath(folderPath), imagename));
                            }

                            news.FolderPath = folderPath;
                        }

                        var dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timezone);
                        news.Date = dateTime;
                        news.UserId = UserId;
                        news.LikeCount = 0;
                        db.News.Add(news);
                        db.SaveChanges();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //public Boolean SaveNews(News news) {
        //    try {
        //        using (ApplicationDbContext db = new ApplicationDbContext()) {

        //            return true;       
        //        }
        //    }catch(Exception ex) {
        //        return false;
        //    }
        //}

        public Boolean Like(String UserId,int NewsId) {
            try {
                using (ApplicationDbContext db = new ApplicationDbContext()) {
                    var news = db.News.Where(m => m.Id == NewsId).FirstOrDefault();
                    if (news != null) {
                        news.LikeCount = (news.LikeCount + 1);
                        db.SaveChanges();

                        NewsLike nlk = new NewsLike();
                        nlk.NewsId = NewsId;
                        nlk.UserId = UserId;
                        db.NewsLikes.Add(nlk);
                        db.SaveChanges();
                        return true;
                    } else {
                        return false;
                    }
                  
                }
            } catch (Exception ex) {
                return false;
            }
        }
    }
}