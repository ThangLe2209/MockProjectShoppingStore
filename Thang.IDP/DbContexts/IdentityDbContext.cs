using Microsoft.EntityFrameworkCore;
using Thang.IDP.Entities;

namespace Thang.IDP.DbContexts
{
    public class IdentityDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<UserClaim> UserClaims { get; set; }         
        public DbSet<UserLogin> UserLogins { get; set; }         
        public DbSet<UserSecret> UserSecrets { get; set; }         
        public DbSet<UserRole> UserRoles { get; set; }         

        public IdentityDbContext(
          DbContextOptions<IdentityDbContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
            .HasIndex(u => u.Subject)
            .IsUnique();

            modelBuilder.Entity<User>()
            .HasIndex(u => u.UserName)
            .IsUnique();

            modelBuilder.Entity<User>().HasData(
                new User()
                {
                    Id = new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"),
                    //Password = "password",
                    Password = "AQAAAAIAAYagAAAAEA2UtwPl2ClVXS2GxZAimtkI1J9LDURK7V5VUi/jjwSl3DA7ILndLMPYR5JxVKJDxQ==", // 123456
                    Subject = "d860efca-22d9-47fd-8249-791ba61b07c7",
                    UserName = "David",
                    Email = "david@someprovider.com",
                    Active = true,
                    UserRoleId = new Guid("d65cf2bd-3840-41d3-af42-f7ff1a7122ef")
                },
                new User()
                {
                    Id = new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"),
                    //Password = "password",
                    Password = "AQAAAAIAAYagAAAAEA2UtwPl2ClVXS2GxZAimtkI1J9LDURK7V5VUi/jjwSl3DA7ILndLMPYR5JxVKJDxQ==",
                    Subject = "b7539694-97e7-4dfe-84da-b4256e1ff5c7",
                    UserName = "Emma",
                    Email = "emma@someprovider.com",
                    Active = true,
                    UserRoleId = new Guid("d7ab6668-2af4-4ea4-a93b-3d96dc475d8e")
                });

            modelBuilder.Entity<UserClaim>().HasData(
             new UserClaim()
             {
                 Id = Guid.NewGuid(),
                 UserId = new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"),
                 Type = "given_name",
                 Value = "David"
             },
             new UserClaim()
             {
                 Id = Guid.NewGuid(),
                 UserId = new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"),
                 Type = "email",
                 Value = "david@someprovider.com"
			 },
             new UserClaim()
             {
                 Id = Guid.NewGuid(),
                 UserId = new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"),
                 Type = "family_name",
                 Value = "Flagg"
             }, 
             new UserClaim()
             {
                 Id = Guid.NewGuid(),
                 UserId = new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"),
                 Type = "country",
                 Value = "nl"
             },
             new UserClaim()
             {
                 Id = Guid.NewGuid(),
                 UserId = new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"),
                 Type = "role",
                 Value = "Admin"
             },
             new UserClaim()
             {
                 Id = Guid.NewGuid(),
                 UserId = new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"),
                 Type = "email",
                 Value = "emma@someprovider.com"
			 },
             new UserClaim()
             {
                 Id = Guid.NewGuid(),
                 UserId = new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"),
                 Type = "given_name",
                 Value = "Emma"
             },
             new UserClaim()
             {
                 Id = Guid.NewGuid(),
                 UserId = new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"),
                 Type = "family_name",
                 Value = "Flagg"
             }, 
             new UserClaim()
             {
                 Id = Guid.NewGuid(),
                 UserId = new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"),
                 Type = "country",
                 Value = "be"
             }, 
             new UserClaim()
             {
                 Id = Guid.NewGuid(),
                 UserId = new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"),
                 Type = "role",
                 Value = "PayingUser"
             });

            modelBuilder.Entity<UserRole>().HasData(
             new UserRole()
             {
                 Id = new Guid("1069eee8-509a-46f9-9800-da3d0e12d560"),
                 Value = "FreeUser"
             },
             new UserClaim()
             {
                 Id = new Guid("d7ab6668-2af4-4ea4-a93b-3d96dc475d8e"),
                 Value = "PayingUser"
             },
             new UserClaim()
             {
                 Id = new Guid("d65cf2bd-3840-41d3-af42-f7ff1a7122ef"),
                 Value = "Admin"
             });
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // get updated entries
            var updatedConcurrencyAwareEntries = ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Modified)
                    .OfType<IConcurrencyAware>();

            foreach (var entry in updatedConcurrencyAwareEntries)
            {
                entry.ConcurrencyStamp = Guid.NewGuid().ToString();
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
