using Application.API.Consumer;
using Application.Models;
using Application.Models.Identity;
using Application.Models.Implementations;
using Application.Models.Suscription;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System;

namespace Application.MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //endpoint de Music API
            Crud<Application.Models.Music>.EndPoint = "https://localhost:7095/api/Musics";
            Crud<Application.Models.Album>.EndPoint = "https://localhost:7095/api/Albums"; 
            Crud<Application.Models.Playlist>.EndPoint = "https://localhost:7095/api/Playlists";

            Crud<Download>.EndPoint = "https://localhost:7095/api/Downloads";
            Crud<Follow>.EndPoint = "https://localhost:7095/api/Follows";
            Crud<Notification>.EndPoint = "https://localhost:7095/api/Notifications";
            Crud<SubscriptionPlan>.EndPoint = "https://localhost:7095/api/SubscriptionPlans";
            Crud<UserSubscription>.EndPoint = "https://localhost:7095/api/UserSubscriptions";

            
            

            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("SqlConnection")
                ?? throw new InvalidOperationException("Connection string not found.");

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            //  Identity directo en MVC
            builder.Services.AddDefaultIdentity<User>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            })
            .AddDefaultUI()  
            .AddRoles<Role>()
            .AddEntityFrameworkStores<AppDbContext>();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";       
            });


            
            
            builder.Services.AddScoped<Application.MVC.Services.IEmailService, Application.MVC.Services.EmailService>();
            
            // Add services to the container.
            builder.Services.AddControllersWithViews();


            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 100_000_000; // 100MB
                options.ValueLengthLimit = int.MaxValue;
                options.ValueCountLimit = int.MaxValue;
            });

            builder.Services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = 100_000_000; // 100MB
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            //
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "Storage")),
                RequestPath = "/files"
            });

            

            app.UseRouting();

            //
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
