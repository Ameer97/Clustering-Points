using Clustering.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Clustering.Dto
{
    public class TypesCities
    {
        public SelectList Types { get; set; }
        public SelectList Cities { get; set; }
        public List<Point> Points { get; set; }
    }
}
