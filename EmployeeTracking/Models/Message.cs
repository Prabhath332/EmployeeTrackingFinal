namespace EmployeeTracking.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Message
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Message()
        {
            MessageRecipients = new HashSet<MessageRecipient>();
        }

        public int Id { get; set; }

        [StringLength(128)]
        public string MsgFrom { get; set; }

        [Column("Message")]
        public string Message1 { get; set; }

        public DateTime  Date { get; set; }

        [NotMapped]
        public string Recipient { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MessageRecipient> MessageRecipients { get; set; }
    }

    public class messageusers {
        public String RoalId { get; set; }
        public String UserId { get; set; }
        public String Name { get; set; }
        public String Image { get; set; }
        public String LastMessage { get; set; }
        public String MessageTime { get; set; }
        public String profileImage { get; set; }        
    }
    public class Messages {
        [NotMapped]
        public int Id { get; set; }
        [NotMapped]
        public DateTime Date { get; set; }
        public String time { get; set; }
        public String message { get; set; }
        public String msgtype { get; set; }
        public String userid { get; set; }
        public String reid { get; set; }
        public String fromid { get; set; }
       
        List<Messages> msglist { get; set; }
    }

    public class MsgThread
    {
        public int Id { get; set; }
        public string MsgUser { get; set; }
        public string UserImage { get; set; }        
        public string Msg { get; set; }
        public DateTime Date { get; set; }
        public bool IsRecieved { get; set; }
    }

    public class MsgIn
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string FromImage { get; set; }        
        public string Msg { get; set; }
        public DateTime Date { get; set; }
    }

}
