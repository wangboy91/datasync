using Microsoft.EntityFrameworkCore;
using DataSync.Domain.Entities;

namespace DataSync.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<Source> Sources => Set<Source>();
    public DbSet<Target> Targets => Set<Target>();
    public DbSet<Rule> Rules => Set<Rule>();
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<SchemaTable> SchemaTables => Set<SchemaTable>();
    public DbSet<SchemaColumn> SchemaColumns => Set<SchemaColumn>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SchemaTable>(b =>
        {
            b.HasMany(t => t.Columns)
             .WithOne()
             .HasForeignKey(c => c.SchemaTableId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Rule>(b =>
        {
            b.OwnsMany(r => r.Mappings, a =>
            {
                a.ToJson();
            });
            
            b.OwnsMany(r => r.MergeStrategies, a =>
            {
                a.ToJson();
            });

            // PostgreSQL 数组支持
            b.Property(r => r.DedupeBy); 
        });

        modelBuilder.Entity<Source>(b =>
        {
            // Connection 可能是 JSON 字符串或加密字符串，这里暂按普通字符串处理
        });
        
        base.OnModelCreating(modelBuilder);
    }
}
