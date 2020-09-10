using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Clustering.Models
{
    public class Point
    {
        [Key]
        public int Id { get; set; }
        public double Lat { get; set; }
        public double Lan { get; set; }
        public string Description { get; set; }
        public int TypeId { get; set; }
        [ForeignKey(nameof(TypeId))]
        public Type Type { get; set; }
        public string Name { get; set; }
        public int CityId { get; set; }
        [ForeignKey(nameof(CityId))]
        public City City { get; set; }
        public Geometry Geom { get; set; }
    }
}
