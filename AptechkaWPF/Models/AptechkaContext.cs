using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AptechkaWPF {

    public partial class AptechkaContext : DbContext
    {
        public AptechkaContext()
        {
        }

        public AptechkaContext(DbContextOptions<AptechkaContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Address> Addresses { get; set; } = null!;

        public virtual DbSet<Drug> Drugs { get; set; } = null!;

        public virtual DbSet<Drugstore> Drugstores { get; set; } = null!;

        public virtual DbSet<Producer> Producers { get; set; } = null!;

        public virtual DbSet<Purchase> Purchases { get; set; } = null!;

        public virtual DbSet<Request> Requests { get; set; } = null!;

        public virtual DbSet<Status> Statuses { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

        /*
         * #warning To protect potentially sensitive information in your connection string, 
         * you should move it out of source code. You can avoid scaffolding the connection string
         * by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. 
         * For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
         * 
         */
        => optionsBuilder.UseSqlServer();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>(entity =>
            {
                entity.ToTable("Address");

                entity.Property(e => e.City).HasMaxLength(50);
                entity.Property(e => e.Home).HasMaxLength(50);
                entity.Property(e => e.Street).HasMaxLength(50);
            });

            modelBuilder.Entity<Drug>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_Drugs");

                entity.ToTable("Drug");

                entity.Property(e => e.BestBeforeDate).HasColumnType("date");
                entity.Property(e => e.DateOfManufacture).HasColumnType("date");
                entity.Property(e => e.Name).HasMaxLength(80);
                entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.Producer).WithMany(p => p.Drugs)
                    .HasForeignKey(d => d.ProducerId)
                    .HasConstraintName("FK_Drug_Producer");
            });

            modelBuilder.Entity<Drugstore>(entity =>
            {
                entity.ToTable("Drugstore");

                entity.Property(e => e.Name).HasMaxLength(50);
                entity.Property(e => e.PharmacyInn)
                    .HasMaxLength(50)
                    .HasColumnName("PharmacyINN");
                entity.Property(e => e.Telephone).HasMaxLength(50);

                entity.HasOne(d => d.Address).WithMany(p => p.Drugstores)
                    .HasForeignKey(d => d.AddressId)
                    .HasConstraintName("FK_Drugstore_Address");
            });

            modelBuilder.Entity<Producer>(entity =>
            {
                entity.ToTable("Producer");

                entity.Property(e => e.Email).HasMaxLength(50);
                entity.Property(e => e.LicanceNumber).HasMaxLength(50);
                entity.Property(e => e.Name).HasMaxLength(50);
                entity.Property(e => e.Telephone).HasMaxLength(50);

                entity.HasOne(d => d.Address).WithMany(p => p.Producers)
                    .HasForeignKey(d => d.AddressId)
                    .HasConstraintName("FK_Producer_Address");
            });

            modelBuilder.Entity<Purchase>(entity =>
            {
                entity.ToTable("Purchase");

                entity.Property(e => e.IdDrugs).HasColumnName("idDrugs");
                entity.Property(e => e.IdRequests).HasColumnName("idRequests");

                entity.HasOne(d => d.IdDrugsNavigation).WithMany(p => p.Purchases)
                    .HasForeignKey(d => d.IdDrugs)
                    .HasConstraintName("FK_Purchase_Drug");

                entity.HasOne(d => d.IdRequestsNavigation).WithMany(p => p.Purchases)
                    .HasForeignKey(d => d.IdRequests)
                    .HasConstraintName("FK_Purchase_Request");
            });

            modelBuilder.Entity<Request>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_Requests");

                entity.ToTable("Request");

                entity.Property(e => e.DateFinish).HasColumnType("date");
                entity.Property(e => e.DateIn).HasColumnType("date");

                entity.HasOne(d => d.Drugstore).WithMany(p => p.Requests)
                    .HasForeignKey(d => d.DrugstoreId)
                    .HasConstraintName("FK_Request_Drugstore");

                entity.HasOne(d => d.Status).WithMany(p => p.Requests)
                    .HasForeignKey(d => d.StatusId)
                    .HasConstraintName("FK_Request_Status");
            });

            modelBuilder.Entity<Status>(entity =>
            {
                entity.ToTable("Status");

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }

}