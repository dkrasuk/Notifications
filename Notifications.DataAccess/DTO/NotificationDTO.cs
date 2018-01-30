using System;

namespace Notifications.DataAccess.DTO
{
    public class NotificationDTO
    {
        public long? Id { get; set; }

        public string Channel { get; set; }

        public string Receiver { get; set; }

        public string Type { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public bool IsReaded { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string Protocol { get; set; }
    }
}
