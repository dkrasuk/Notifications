using System.Data.Entity;
using Notifications.Common;
using Notifications.DataAccess.Migrations;
using Notifications.DataAccess.Models;

namespace Notifications.DataAccess
{
    public class NotificationsContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationsContext"/> class.
        /// </summary>
        public NotificationsContext()
            : base(Constants.NotificationConnectionStringName)
        {
        }

        /// <summary>
        /// Gets or sets the notifications.
        /// </summary>
        /// <value>
        /// The notifications.
        /// </value>
        public virtual DbSet<Notification> Notifications { get; set; }



        /// <summary>
        /// This method is called when the model for a derived context has been initialized, but
        /// before the model has been locked down and used to initialize the context.  The default
        /// implementation of this method does nothing, but it can be overridden in a derived class
        /// such that the model can be further configured before it is locked down.
        /// </summary>
        /// <param name="modelBuilder">The builder that defines the model for the context being created.</param>
        /// <remarks>
        /// Typically, this method is called only once when the first instance of a derived context
        /// is created.  The model for that context is then cached and is for all further instances of
        /// the context in the app domain.  This caching can be disabled by setting the ModelCaching
        /// property on the given ModelBuidler, but note that this can seriously degrade performance.
        /// More control over caching is provided through use of the DbModelBuilder and DbContextFactory
        /// classes directly.
        /// </remarks>

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var schemeName = "";//ConfigurationManager.AppSettings["NotificationsContextDatabaseSchema"];
            if (string.IsNullOrEmpty(schemeName))
            {
                schemeName = "NOTIFICATION";
            }
            modelBuilder.HasDefaultSchema(schemeName);

            base.OnModelCreating(modelBuilder);

            Database.SetInitializer<NotificationsContext>(
                new MigrateDatabaseToLatestVersion<NotificationsContext, Configuration>());
        }
    }
}
