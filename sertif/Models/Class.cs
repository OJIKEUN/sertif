using System.ComponentModel.DataAnnotations;

namespace sertif.Models
{
    public class Class
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
    }
}
