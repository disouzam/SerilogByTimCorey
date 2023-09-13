using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;

namespace SerilogDemo
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("serilogdemo.settings.json", false, true)
                .AddJsonFile($"serilogdemo.settings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true, true)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            try
            {
                Log.Information("Application Starting up...");

                var builder = WebApplication.CreateBuilder(args);

                builder.Host.UseSerilog();

                // Add services to the container.
                builder.Services.AddRazorPages();

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if(!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }

                app.UseHttpsRedirection();
                app.UseStaticFiles();

                app.UseSerilogRequestLogging();

                app.UseRouting();

                app.UseAuthorization();

                app.MapRazorPages();

                app.Run();
            }
            catch(Exception ex)
            {
                Log.Fatal(ex, "The application failed to start correctly!");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}