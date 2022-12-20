using Microsoft.EntityFrameworkCore;
using RESTApiTestAppImageUploader.Models;

namespace RESTApiTestAppImageUploader.Data
{
    public class TestAppDbContext : DbContext
    {
        public TestAppDbContext(DbContextOptions<TestAppDbContext> options) : base(options) { }

        public DbSet<ImageUpload> ImageUploads { get; set; }
    }
}
