using Microsoft.EntityFrameworkCore;
using RESTApiTestAppImageUploader.Data;
using RESTApiTestAppImageUploader.Services;

namespace RESTApiTestAppImageUploader
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.AddDbContext<TestAppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("TestAppConnection")));
            builder.Services.AddScoped<IImageService, ImageService>();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var dataContext = scope.ServiceProvider.GetRequiredService<TestAppDbContext>();

                dataContext.Database.Migrate();
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}