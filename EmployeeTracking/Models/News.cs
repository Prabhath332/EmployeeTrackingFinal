namespace EmployeeTracking.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;


    public partial class News
    {
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
       // public News()
       // {
          //  NewsComments = new HashSet<NewsComment>();
          //  NewsImages = new HashSet<NewsImage>();
          //  NewsLikes = new HashSet<NewsLike>();
        //}

        public int Id { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        [StringLength(200)]
        public string Title { get; set; }

        public DateTime  Date { get; set; }

        public string Description { get; set; }

        [StringLength(200)]
        public string Venu { get; set; }
         
        public string FolderPath { get; set; }

        public int LikeCount { get; set; }
        
        public String UpdateBy { get; set; }

        public DateTime? UpdateOn { get; set; }

        [NotMapped]
        public List<imagesliest> Image64list { get; set; }
        [NotMapped]
        public String DateStr { get; set; }
        [NotMapped]
        public String isLike  { get; set; }
      //  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
      //   public virtual ICollection<NewsComment> NewsComments { get; set; }

        //  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        // public virtual ICollection<NewsImage> NewsImages { get; set; }

        // [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        // public virtual ICollection<NewsLike> NewsLikes { get; set; }
    }

    public class imagesliest {
       public String Imageb64 { get; set; }
        public string ImageName { get; set; }
    }

    public class NewsViewModel
    {
        public int Id { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        public string Username { get; set; }

        public string UserImage { get; set; }

        [StringLength(200)]
        public string Title { get; set; }

        public DateTime Date { get; set; }

        public string Description { get; set; }

        [StringLength(200)]
        public string Venu { get; set; }

        public string FolderPath { get; set; }

        public int LikeCount { get; set; }

        [NotMapped]
        public List<imagesliest> Image64list { get; set; }
        [NotMapped]
        public String DateStr { get; set; }
        [NotMapped]
        public String isLike { get; set; }
    }
}
