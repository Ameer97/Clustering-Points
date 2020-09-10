using System.ComponentModel.DataAnnotations;

namespace Clustering.Models
{
    public class City
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
