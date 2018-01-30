using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Notifications.DataAccess.Models
{
    [Table("NOTIFICATIONS")]
    public class Notification
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long? Id { get; set; }

        [Column("CHANNEL")]
        [MaxLength(255)]
        [Required]
        public string Channel { get; set; }

        [Column("RECEIVER")]
        [MaxLength(255)]
        [Required]
        public string Receiver { get; set; }

        [Column("TYPE")]
        [MaxLength(255)]
        public string Type { get; set; }

        [MaxLength(500)]
        [Column("TITLE")]
        //[Required]
        public string Title { get; set; }

        [MaxLength(2000)]
        [Column("BODY")]
        [Required]
        public string Body { get; set; }

        [Column("ISREADED")]
        public bool IsReaded { get; set; }

        [Column("CREATEDDATE")]
        public DateTime? CreatedDate { get; set; }

        [Column("PROTOCOL")]
        [MaxLength(255)]
        public string Protocol { get; set; }

    }
}
