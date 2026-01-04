using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using DataSync.Domain.Repositories;
using DataSync.Infrastructure.Repositories;
using DataSync.Infrastructure.Persistence;
using DataSync.Infrastructure.SchemaProviders;
using DataSync.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

// 根据环境变量加载对应的配置文件
var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
var envFile = envName == "Production" ? ".env.pro" : ".env.dev";
Env.Load(envFile);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>  
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ISourceRepository, EfSourceRepository>();
builder.Services.AddScoped<ITargetRepository, EfTargetRepository>();
builder.Services.AddScoped<IRuleRepository, EfRuleRepository>();
builder.Services.AddScoped<IJobRepository, EfJobRepository>();

// Schema Providers
builder.Services.AddScoped<IDbSchemaProvider, PostgresSchemaProvider>();
builder.Services.AddScoped<IDbSchemaProvider, MySqlSchemaProvider>();
builder.Services.AddScoped<IDbSchemaProvider, SqlServerSchemaProvider>();
builder.Services.AddScoped<SchemaService>();

var app = builder.Build();

// 自动执行数据库迁移
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "ok");

app.MapControllers();

app.Run();
