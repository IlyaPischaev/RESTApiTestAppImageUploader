using System.ComponentModel.DataAnnotations;

namespace RESTApiTestAppImageUploader.Models
{
    public class Base
    {
        [Key]
        public int Id { get; set; }
    }
}
