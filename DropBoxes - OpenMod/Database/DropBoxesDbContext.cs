using DropBoxes.Database.Models;
using Microsoft.EntityFrameworkCore;
using OpenMod.EntityFrameworkCore;
using OpenMod.EntityFrameworkCore.Configurator;
using System;

namespace DropBoxes.Database
{
    public class DropBoxesDbContext : OpenModDbContext<DropBoxesDbContext>
    {
        public DropBoxesDbContext(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public DropBoxesDbContext(IDbContextConfigurator configurator, IServiceProvider serviceProvider) :
            base(configurator, serviceProvider)
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
