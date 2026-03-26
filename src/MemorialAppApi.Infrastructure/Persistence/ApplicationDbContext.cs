namespace MemorialAppApi.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Memorial> Memorials { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Cemetery> Cemeteries { get; set; } = null!;
    public DbSet<Burial> Burials { get; set; } = null!;
    public DbSet<Contact> Contacts { get; set; } = null!;
    public DbSet<MemorialTimeline> MemorialTimelines { get; set; } = null!;
    public DbSet<Event> Events { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Memorial>(entity =>
        {
            entity.ToTable("Memorials");

            entity.HasKey(e => e.Id);

            // Profile properties
            entity.Property(e => e.ProfileType)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.IsPublic)
                .IsRequired()
                .HasDefaultValue(false);

            entity.Property(e => e.FullName)
                .IsRequired()
                .HasMaxLength(500);

            // Detailed information properties (all NVARCHAR(MAX))
            entity.Property(e => e.BirthDetails);
            entity.Property(e => e.PassingDetails);
            entity.Property(e => e.AppearanceAtBirth);
            entity.Property(e => e.Family);
            entity.Property(e => e.Visitors);
            entity.Property(e => e.ParentThoughts);
            entity.Property(e => e.Letters);
            entity.Property(e => e.Notes);
            entity.Property(e => e.Personalities);
            entity.Property(e => e.Hobbies);
            entity.Property(e => e.LifeDetails);
            entity.Property(e => e.Media);

            // System fields
            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.UpdatedAt);

            entity.Property(e => e.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            entity.Property(e => e.CreatedBy);

            // Indexes
            entity.HasIndex(e => e.FullName);
            entity.HasIndex(e => e.ProfileType);
            entity.HasIndex(e => e.IsPublic);
            entity.HasIndex(e => e.CreatedBy);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.IsDeleted);

            // Query filter for soft delete
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.PublicName)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(e => e.ProfilePic)
                .HasMaxLength(500);

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.UpdatedAt);

            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Indexes
            entity.HasIndex(e => e.Email)
                .IsUnique();

            entity.HasIndex(e => e.IsActive);
        });

        modelBuilder.Entity<Cemetery>(entity =>
        {
            entity.ToTable("Cemeteries");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.Location)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.StreetAddress)
                .HasMaxLength(500);

            entity.Property(e => e.Longitude)
                .HasColumnType("DECIMAL(10, 7)");

            entity.Property(e => e.Latitude)
                .HasColumnType("DECIMAL(10, 7)");

            entity.Property(e => e.Description);

            entity.Property(e => e.AdditionalInfo);

            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Active");

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.UpdatedAt);

            entity.Property(e => e.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            // Relationships
            entity.HasOne(e => e.Contact)
                .WithOne(c => c.Cemetery)
                .HasForeignKey<Contact>(c => c.CemeteryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.Location);
            entity.HasIndex(e => e.IsDeleted);
            entity.HasIndex(e => new { e.Latitude, e.Longitude });

            // Query filter for soft delete
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.ToTable("Contacts");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Email)
                .HasMaxLength(255);

            entity.Property(e => e.Phone)
                .HasMaxLength(50);

            entity.Property(e => e.Website)
                .HasMaxLength(500);

            entity.Property(e => e.OfficeAddress)
                .HasMaxLength(500);

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.UpdatedAt);

            // Indexes
            entity.HasIndex(e => e.CemeteryId);
        });

        modelBuilder.Entity<Burial>(entity =>
        {
            entity.ToTable("Burials");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.BurialType)
                .HasMaxLength(100);

            entity.Property(e => e.PlotNumber)
                .HasMaxLength(100);

            entity.Property(e => e.Longitude)
                .HasColumnType("DECIMAL(10, 7)");

            entity.Property(e => e.Latitude)
                .HasColumnType("DECIMAL(10, 7)");

            entity.Property(e => e.Inscription);

            entity.Property(e => e.Gravesite)
                .HasMaxLength(255);

            entity.Property(e => e.Cenotaph)
                .IsRequired()
                .HasDefaultValue(false);

            entity.Property(e => e.Monument)
                .IsRequired()
                .HasDefaultValue(false);

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.UpdatedAt);

            // Indexes
            entity.HasIndex(e => e.PlotNumber);
        });

        modelBuilder.Entity<MemorialTimeline>(entity =>
        {
            entity.ToTable("MemorialTimeline");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.MemorialId)
                .IsRequired();

            entity.Property(e => e.Title)
                .HasMaxLength(500);

            entity.Property(e => e.Date);

            entity.Property(e => e.Description);

            entity.Property(e => e.Photos);

            entity.Property(e => e.Video);

            entity.Property(e => e.Audio);

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.UpdatedAt);

            entity.Property(e => e.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            entity.Property(e => e.CreatedBy);
            entity.Property(e => e.UpdatedBy);

            // Relationships
            entity.HasOne(e => e.Memorial)
                .WithMany(m => m.Timelines)
                .HasForeignKey(e => e.MemorialId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            entity.HasIndex(e => e.MemorialId);

            // Query filters
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.ToTable("Events");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.MemorialId)
                .IsRequired();

            entity.Property(e => e.EventName)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.EventDate);

            entity.Property(e => e.LocationName)
                .HasMaxLength(255);

            entity.Property(e => e.Latitude)
                .HasMaxLength(50);

            entity.Property(e => e.Longitude)
                .HasMaxLength(50);

            entity.Property(e => e.Address)
                .HasMaxLength(500);

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.UpdatedAt);

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(255);

            // Relationships
            entity.HasOne(e => e.Memorial)
                .WithMany(m => m.Events)
                .HasForeignKey(e => e.MemorialId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            entity.HasIndex(e => e.MemorialId);
            entity.HasIndex(e => e.EventDate);
            entity.HasIndex(e => e.EventName);
        });
    }
}
