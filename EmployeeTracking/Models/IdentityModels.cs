using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace EmployeeTracking.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public virtual string MobileNumber { get; set; }
        public virtual string FaxNumber { get; set; }
        public virtual string Address { get; set; }
        public virtual string MobileAccount { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        //87yTr10s6s8q db Password
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            //this.Configuration.LazyLoadingEnabled = false;
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<EducationalInfo> EducationalInfos { get; set; }
        public virtual DbSet<EmployeeAward> EmployeeAwards { get; set; }
        public virtual DbSet<EmployeeLocation> EmployeeLocations { get; set; }
        public virtual DbSet<EmployeementInfo> EmployeementInfos { get; set; }
        public virtual DbSet<EmployeePromotion> EmployeePromotions { get; set; }
        public virtual DbSet<Meeting> Meetings { get; set; }
        public virtual DbSet<MeetingUser> MeetingUsers { get; set; }
        public virtual DbSet<MessageRecipient> MessageRecipients { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<News> News { get; set; }
        public virtual DbSet<NewsComment> NewsComments { get; set; }
        public virtual DbSet<NewsImage> NewsImages { get; set; }
        public virtual DbSet<NewsLike> NewsLikes { get; set; }
        public virtual DbSet<PastExperiance> PastExperiances { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<ProjectTeam> ProjectTeams { get; set; }
        public virtual DbSet<TeamMember> TeamMembers { get; set; }
        public virtual DbSet<UserCompany> UserCompanies { get; set; }
        public virtual DbSet<UserProfile> UserProfiles { get; set; }
        public virtual DbSet<RoleLevels> RoleLevels { get; set; }
        public virtual DbSet<UserModules> UserModules { get; set; }
        public virtual DbSet<ModuleUsers> ModuleUsers { get; set; }
        public virtual DbSet<Notifications> Notifications { get; set; }
        public virtual DbSet<LeaveHistory> LeaveHistories { get; set; }
        public virtual DbSet<LeaveType> LeaveTypes { get; set; }
        public virtual DbSet<UserLeaves> UserLeaves { get; set; }
        public virtual DbSet<UserLevelLeaves> UserLevelLeaves { get; set; }
        public virtual DbSet<OneSignal> OneSignal { get; set; }
        public virtual DbSet<LeaveApproval> LeaveApprovals { get; set; }
        public virtual DbSet<AttendenceCorrections> AttendenceCorrections { get; set; }
        public virtual DbSet<EmployeeTransfer> EmployeeTransfers { get; set; }
        public virtual DbSet<RejectedLeave> RejectedLeaves { get; set; }
        public virtual DbSet<SiteLocation> SiteLocations { get; set; }

        public virtual DbSet<Holiday> Holidays { get; set; }

        public virtual DbSet<Employee_NoPay_Leaves> NoPayLeaves { get; set; }

    }
}