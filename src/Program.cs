using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using enova_academy.Data;

var builder = WebApplication.CreateBuilder(args);

// Configuração MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Se o argumento --migrate for passado, aplica as migrations
    if (args.Contains("--migrate"))
    {
        Console.WriteLine("===> Executando migrations...");
        db.Database.Migrate();
        Console.WriteLine("===> Migrations aplicadas com sucesso!");
        return; // Sai depois de aplicar migrations
    }

    var services = scope.ServiceProvider;
    await SeedData.InitializeAsync(services);
}

app.Run();
