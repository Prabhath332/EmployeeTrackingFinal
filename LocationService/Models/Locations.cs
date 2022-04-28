using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LocationService.Models
{
    public class Locations
    {
        public List<RootObject> Location { get; set; }
    }

    public class RootObject
    {
        public string User { get; set; }
        public string Mobile { get; set; }
        public string Error { get; set; }
        public string Max_requests_exeeded { get; set; }
        public string Is_Valid_User { get; set; }
        public string Have_Permission_to_View_Number { get; set; }
        public string CGI { get; set; }
        public string LONGITUDE { get; set; }
        public string STATE { get; set; }
        public string CELL_ID { get; set; }
        public string UPDATE_TIME { get; set; }
        public string SITE_NAME { get; set; }
        public string VLR { get; set; }
        public string LATITUDE { get; set; }
        public string AGE { get; set; }
    }

    public class UserLocationsViewModel
    {
        public string UserId { get; set; }
        public string MobileNo { get; set; }
        public string MobileAccount { get; set; }
    }
}