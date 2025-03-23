using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PNE_core.Models;

namespace PNE_DataAccess
{
    public class NeonContext : DbContext
    {
        public NeonContext(DbContextOptions<NeonContext> options) : base(options)
        {
        }

        public DbSet<Embarcation> Embarcations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Embarcation>(entity =>
            {
                entity.ToTable("embarcation");

                entity.HasKey(e => e.IdEmbarcation);
                entity.Property(e => e.IdEmbarcation).HasColumnName("id_embarcation");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.Marque).HasColumnName("marque");
                entity.Property(e => e.Longueur).HasColumnName("longueur");
                entity.Property(e => e.Photo).HasColumnName("photo");
            });
        }
    }
}
