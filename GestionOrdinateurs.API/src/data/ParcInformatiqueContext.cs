namespace GestionOrdinateurs.API.src.data;
using Microsoft.EntityFrameworkCore;
using GestionOrdinateurs.Shared.src.model;
public class ParcInformatiqueContext : DbContext
{
public DbSet<Ordinateur> Ordinateurs { get; set; }

public DbSet<Reparation> Reparations { get; set; }

public DbSet <User> Users {get; set;}

public ParcInformatiqueContext(DbContextOptions<ParcInformatiqueContext> options)
    : base(options)
{
}
protected override void OnConfiguring(
    DbContextOptionsBuilder optionsBuilder)
{
    if (!optionsBuilder.IsConfigured)
        {
    optionsBuilder.UseSqlite("Data Source=parcinfo.db");
        }
}

protected override void OnModelCreating(ModelBuilder modelBuilder)
{  
        modelBuilder.Entity<Ordinateur>()  
            .Property(b => b.Stock)  
            .HasDefaultValue(0);

        modelBuilder.Entity<Ordinateur>()  
            .Property(b => b.Prix)  
            .HasDefaultValue(500);
        
        modelBuilder.Entity<Ordinateur>()
            .Property(b => b.Vendu)
            .HasDefaultValue(false);
            
        modelBuilder.Entity<Ordinateur>()
            .OwnsOne(o => o.Caracteristiques);

        modelBuilder.Entity<Ordinateur>()
            .HasMany(o => o.Reparations)
            .WithOne(r => r.UnOrdinateur);

        modelBuilder.Entity<User>()
            .HasIndex(u=>u.Login)
            .IsUnique();
        }
                
}