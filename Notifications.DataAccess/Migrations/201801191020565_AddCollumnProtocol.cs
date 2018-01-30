namespace Notifications.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCollumnProtocol : DbMigration
    {
        public override void Up()
        {
            AddColumn("NOTIFICATION.NOTIFICATIONS", "PROTOCOL", c => c.String(maxLength: 255));
        }
        
        public override void Down()
        {
            DropColumn("NOTIFICATION.NOTIFICATIONS", "PROTOCOL");
        }
    }
}
