using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Trizoob.Data;
namespace Trizoob
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<TrizoobContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("TrizoobContext") ?? throw new InvalidOperationException("Connection string 'TrizoobContext' not found.")));

            // Add services to the container.
            builder.Services.AddRazorPages();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(60);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            //app.Services.GetRequiredService<IHttpContextAccessor>();

            IWebHostEnvironment env = app.Services.GetRequiredService<IWebHostEnvironment>();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }
            app.UseStaticFiles();

            app.UseSession();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}