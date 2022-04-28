namespace EmployeeTracking.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class MessageRecipient
    {
        public int Id { get; set; }

        [StringLength(128)]
        public string MsgTo { get; set; }

        public int MessageId { get; set; }

        public virtual Message Message { get; set; }
    }
}
