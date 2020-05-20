using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace UrlShortenerNETCore.Models
{
    public partial class ShortthemupContext : DbContext
    {
        public ShortthemupContext()
        {
        }

        public ShortthemupContext(DbContextOptions<ShortthemupContext> options)
            : base(options)
        {
        }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<Campaigns> Campaigns { get; set; }
        public virtual DbSet<UrlClicks> UrlClicks { get; set; }
        public virtual DbSet<Urls> Urls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:DefaultSchema", "shortthemupuser");            

            modelBuilder.Entity<AspNetUsers>(entity =>
            {
                entity.ToTable("AspNetUsers", "dbo");

                entity.Property(e => e.Id).HasMaxLength(128);

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.LockoutEndDateUtc).HasColumnType("datetime");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<Campaigns>(entity =>
            {
                entity.ToTable("Campaigns", "dbo");

                entity.Property(e => e.Id).HasMaxLength(128);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.Campaigns)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_Campaigns_AspNetUsers");
            });

            modelBuilder.Entity<UrlClicks>(entity =>
            {
                entity.ToTable("UrlClicks", "dbo");

                entity.Property(e => e.Id).HasMaxLength(128);

                entity.Property(e => e.ClickedAt).HasColumnType("datetime");

                entity.Property(e => e.UrlId)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.HasOne(d => d.Url)
                    .WithMany(p => p.UrlClicks)
                    .HasForeignKey(d => d.UrlId)
                    .HasConstraintName("FK_UrlClicks_Urls");
            });

            modelBuilder.Entity<Urls>(entity =>
            {
                entity.ToTable("Urls", "dbo");

                entity.Property(e => e.Id).HasMaxLength(128);

                entity.Property(e => e.CampaignId).HasMaxLength(128);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.ExpiresAt).HasColumnType("datetime");

                entity.Property(e => e.Ipaddress)
                    .IsRequired()
                    .HasColumnName("IPAddress")
                    .HasMaxLength(128);

                entity.Property(e => e.LongUrl)
                    .IsRequired()
                    .HasMaxLength(512);

                entity.Property(e => e.ShortUrl)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.Property(e => e.UserId).HasMaxLength(128);

                entity.HasOne(d => d.Campaign)
                    .WithMany(p => p.Urls)
                    .HasForeignKey(d => d.CampaignId)
                    .HasConstraintName("FK_Urls_Campaigns");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Urls)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Urls_AspNetUsers");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
