using Microsoft.EntityFrameworkCore;
using AptechkaAPI;

namespace AptechkaAPI
{

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

            => optionsBuilder.UseSqlServer("server=(localdb)\\MSSQLLocalDB;user id=developer;password=;database=Aptechka;TrustServerCertificate=True");

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

            });

            modelBuilder.Entity<Drugstore>(entity =>
            {
                entity.ToTable("Drugstore");

                entity.Property(e => e.Name).HasMaxLength(50);
                entity.Property(e => e.PharmacyInn)
                    .HasMaxLength(50)
                    .HasColumnName("PharmacyINN");
                entity.Property(e => e.Telephone).HasMaxLength(50);

           });

            modelBuilder.Entity<Producer>(entity =>
            {
                entity.ToTable("Producer");

                entity.Property(e => e.Email).HasMaxLength(50);
                entity.Property(e => e.LicanceNumber).HasMaxLength(50);
                entity.Property(e => e.Name).HasMaxLength(50);
                entity.Property(e => e.Telephone).HasMaxLength(50);

           });

            modelBuilder.Entity<Purchase>(entity =>
            {
                entity.ToTable("Purchase");

                entity.Property(e => e.IdDrugs).HasColumnName("idDrugs");
                entity.Property(e => e.IdRequests).HasColumnName("idRequests");

             });

            modelBuilder.Entity<Request>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_Requests");

                entity.ToTable("Request");

                entity.Property(e => e.DateFinish).HasColumnType("date");
                entity.Property(e => e.DateIn).HasColumnType("date");

             });

            modelBuilder.Entity<Status>(entity =>
            {
                entity.ToTable("Status");

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        public DbSet<AptechkaAPI.Basket>? Basket { get; set; }
    }

}