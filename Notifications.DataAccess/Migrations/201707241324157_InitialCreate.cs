namespace Notifications.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "NOTIFICATION.NOTIFICATIONS",
                c => new
                {
                    ID = c.Decimal(nullable: false, precision: 19, scale: 0, identity: true),
                    CHANNEL = c.String(nullable: false, maxLength: 255),
                    RECEIVER = c.String(nullable: false, maxLength: 255),
                    TYPE = c.String(maxLength: 255),
                    TITLE = c.String(nullable: true, maxLength: 500),
                    BODY = c.String(nullable: false, maxLength: 2000),
                    ISREADED = c.Decimal(nullable: false, precision: 1, scale: 0),
                    CREATEDDATE = c.DateTime()
                })
                .PrimaryKey(t => t.ID);

        }

        public override void Down()
        {
            DropTable("NOTIFICATION.NOTIFICATIONS");
        }
    }
}
