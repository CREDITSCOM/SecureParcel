using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace SecureParcel.Classes.Database
{
    public class DatabaseContext : IdentityDbContext<ApplicationUser>
    {
        public DatabaseContext() : base("MySqlContext", throwIfV1Schema: false)
        {

        }

        public static DatabaseContext Create()
        {
            return new DatabaseContext();
        }

        public DbSet<Parcel> Parcels { get; set; }
        public DbSet<TrackEvent> TrackEvents { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
}