using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clustering.Models
{
    public class Point
    {
        [Key]
        public int Id { get; set; }
        public double Lat { get; set; }
        public double Lan { get; set; }
        public string Description { get; set; }
        public int? ClusterGroup { get; set; }
        public int TypeId { get; set; }
        [ForeignKey(nameof(TypeId))]
        public Type Type { get; set; }
        public string Name { get; set; }
        public Geometry Geom { get; set; }
    }
}
