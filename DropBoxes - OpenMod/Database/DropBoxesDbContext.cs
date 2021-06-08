using DropBoxes.Database.Models;
using Microsoft.EntityFrameworkCore;
using SilK.OpenMod.EntityFrameworkCore;
using System;

namespace DropBoxes.Database
{
    public class DropBoxesDbContext : OpenModPomeloDbContext<DropBoxesDbContext>
    {
        public DropBoxesDbContext(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public DropBoxesDbContext(DbContextOptions<DropBoxesDbContext> options,
            IServiceProvider serviceProvider) : base(options, serviceProvider)
        {
        }

        public DbSet<LootBoxInstance> LootBoxes => Set<LootBoxInstance>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LootBoxInstance>()
                .HasKey(x => new {x.SteamId, x.LootBoxId});

            modelBuilder.Entity<LootBoxInstance>()
                .Property(x => x.Amount)
                .IsConcurrencyToken();
        }
    }
}
