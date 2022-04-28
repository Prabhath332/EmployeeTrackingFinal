using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeTracking.Models
{
    public class UserProfileViewModel
    {
        public ApplicationUser ApplicationUser { get; set; }
        public UserProfile UserProfiles { get; set; }
        public List<EmployeePromotion> EmployeePromotions { get; set; }
        public EducationalInfo EducationalInfos { get; set; }
        public EmployeementInfo EmployeementInfos { get; set; }
        public List<EmployeeAward> EmployeeAwards { get; set; }
        public List<UserCompany> UserCompanies { get; set; }
        public List<SelectListItem> UserDivisions { get; set; }
        public List<SelectListItem> DivisionCodes { get; set; }
    }

    public class mobileprofile{
        //general
        public String userimage { get; set; }
        public String UsersId { get; set; }
        public String Name { get; set; }
        public String EPFNo { get; set; }
        public String Designation { get; set; }
        public String Division { get; set; }
        public String Section { get; set; }
        public String Age { get; set; }
        public String Gender { get; set; }
        public String MaritalStatus { get; set; }
        public String NICNo { get; set; }
        public String DateOfBirth { get; set; }

        //Contact
        public String PermanentAddress { get; set; }
        public String MobileNumbers { get; set; }
        public String FixedTelephoneNumber { get; set; }
        public String Email { get; set; }

        //Educational 
        public String SecondaryEducation { get; set; }
        public String HigherEducation { get; set; }
        public String OtherEducationalQualifications { get; set; }
        public String ExtraCurricularActivities { get; set; }

        //Current  
        public String JobDescription { get; set; }
        public String DateofAppointment { get; set;}
        public String ImmediateReportingPerson { get; set; }
        public String PresentReportingLocation { get; set; }

        public String Remarks { get; set; }

    }
}