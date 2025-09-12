using HomeFromRecords.Core.Data;
using HomeFromRecords.Core.Data.Entities;
using HomeFromRecords.Core.Interfaces;
using HomeFromRecords.Core.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<HomeFromRecordsContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, IdentityRole<Guid>>()
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<HomeFromRecordsContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    var jwtKey = builder.Configuration["Jwt:Key"];
    if (string.IsNullOrEmpty(jwtKey))
        throw new InvalidOperationException("JWT key not configured");

    options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddScoped<IAlbum, AlbumRepos>();
builder.Services.AddScoped<IArtist, ArtistRepos>();
builder.Services.AddScoped<IRecordLabel, RecordLabelRepos>();

builder.Services.AddCors(options => {
    options.AddPolicy(name: "AllowAngularOrigin",
        builder => {
            builder.WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles(new StaticFileOptions {
    FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "Uploads", "Images", "AlbumCovers")),
    RequestPath = "/Images"
});

app.UseHttpsRedirection();

app.UseCors("AllowAngularOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
