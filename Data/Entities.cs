using Microsoft.EntityFrameworkCore;

namespace test.Data;

public interface IProjectItem {
     Guid Id { get; } 
     string Name { get; }
     int Ord { get; }
     DateTime Start { get; }
     DateTime Updated { get; }
     DateTime Stop { get; }
}

public interface IActivity {
    ActivityStatus Stage { get; }    
}

public enum ActivityStatus { Pending, Active, Complete }

public class Project : IProjectItem {
   public Guid Id { get; set; }
   public string Name { get; set; }
   public int Ord { get; set; }
   public DateTime Start { get; set; }
   public DateTime Updated { get; set; }
   public DateTime Stop { get; set; }
   public List<Operation> Operations { get; set; }
}

public class Operation : IProjectItem {
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Project Project { get; set; }
    public string Name { get; set; }
    public int Ord { get; set; }
    public DateTime Start { get; set; }
    public DateTime Updated { get; set; }
    public DateTime Stop { get; set; }
    public List<Step> Steps { get; set; }
}

public abstract class Step : IProjectItem, IActivity {
    public Guid Id { get; set; }
    public Guid OperationId { get; set; }
    public Operation Operation { get; set; }
    public string Name { get; set; }
    public int Ord { get; set; }
    public DateTime Start { get; set; }
    public DateTime Updated { get; set; }
    public DateTime Stop { get; set; }
    public ActivityStatus Stage { get; set; }
}

public class DrawingStep : Step { }

public class PaintingStep : Step { }

public class Context : DbContext
{
    string DbPath { get; }
    
    public DbSet<Project> Projects { get; set; }
    public DbSet<Operation> Operations { get; set; }
    public DbSet<Step> Steps { get; set; }
    public DbSet<DrawingStep> DrawingSteps { get; set; }
    public DbSet<PaintingStep> PaintingSteps { get; set; }
    
    public Context() {
        var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        DbPath = Path.Join(path, "drawing.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) => 
        modelBuilder.Entity<Step>().UseTpcMappingStrategy();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlite($"Data source={DbPath}");
}