using Microsoft.EntityFrameworkCore;
using UserProviderLibrary.Data.Entities;

namespace UserProviderLibrary.Data.Context;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<AccountUserEntity> AccountUser { get; set; }
    public DbSet<AddressEntity> Addresses { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AccountUserEntity>()
            .HasMany(a => a.Addresses)
            .WithOne(x => x.AccountUser)
            .HasForeignKey(x => x.AccountId);

    }
}
