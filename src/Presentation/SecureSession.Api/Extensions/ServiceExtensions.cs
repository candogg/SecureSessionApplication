using Microsoft.AspNetCore.ResponseCompression;
using MongoDB.Driver;
using SecureSession.Api.Contracts;
using SecureSession.Api.Middlewares;
using SecureSession.Api.Persistence.Repositories;
using System.IO.Compression;
using System.Reflection;

namespace SecureSession.Api.Extensions
{
    /// <summary>
    /// Author: Can DOĞU
    /// </summary>
    public static class ServiceExtensions
    {
        public static void RegisterApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });
        }

        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ErrorHandlerMiddleware>();
        }

        public static void RegisterResponseCompression(this IServiceCollection services)
        {
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;

                options.Providers.Clear();
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();

                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                [
                    "application/json",
                    "application/problem+json",
                    "application/xml",
                    "application/javascript",
                    "text/javascript",
                ]);
            });

            services.Configure<BrotliCompressionProviderOptions>(o =>
            {
                o.Level = CompressionLevel.SmallestSize;
            });
            services.Configure<GzipCompressionProviderOptions>(o =>
            {
                o.Level = CompressionLevel.Fastest;
            });
        }

        public static void RegisterPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IMongoClient>(_ =>
            new MongoClient(configuration.GetConnectionString("MongoDb")));
            services.AddScoped(_ =>
            {
                var client = _.GetRequiredService<IMongoClient>();
                return client.GetDatabase("SecureSessionApplicationDb");
            });

            services.AddScoped<IMongoUnitOfWork, MongoUnitOfWork>();
        }
    }
}
