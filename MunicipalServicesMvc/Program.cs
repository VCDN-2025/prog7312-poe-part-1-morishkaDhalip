using System.IO;
using MunicipalServicesMvc.Services;

namespace MunicipalServicesMvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // MVC
            builder.Services.AddControllersWithViews();

            // Our store (custom DS + manual file persistence)
            builder.Services.AddSingleton<IssueStore>(sp =>
                new IssueStore(sp.GetRequiredService<IHostEnvironment>()));

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles(); // serve wwwroot (uploads)

            app.UseRouting();
            app.UseAuthorization();

            // Conventional MVC routing
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Ensure uploads folder exists on first run
            var webRoot = app.Environment.WebRootPath ?? Path.Combine(app.Environment.ContentRootPath, "wwwroot");
            Directory.CreateDirectory(Path.Combine(webRoot, "uploads"));

            app.Run();
        }
    }
}
