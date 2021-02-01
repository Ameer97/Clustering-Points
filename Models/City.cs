using System.ComponentModel.DataAnnotations;
using NetTopologySuite.Geometries;
namespace Clustering.Models
{
    public class City
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public Geometry Geom { get; set; }
        public string TextFormat { get; set; }
        public string TextPoly { get; set; }
    }

}
