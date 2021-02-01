using Clustering.Data;
using Clustering.Dto;
using Clustering.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace Clustering.Controllers
{
    public class PointsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PointsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Points
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Points.Take(200).Include(p => p.Type);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Points/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var point = await _context.Points
                .Include(p => p.Type)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (point == null)
            {
                return NotFound();
            }

            return View(point);
        }

        // GET: Points/Create
        public IActionResult Create()
        {
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name");
            ViewData["TypeId"] = new SelectList(_context.Types, "Id", "Name");
            return View();
        }

        // POST: Points/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Lat,Lan,Description,TypeId,Name,CityId,Geom")] Point point)
        {
            if (ModelState.IsValid)
            {
                _context.Add(point);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Id", point.CityId);
            ViewData["TypeId"] = new SelectList(_context.Types, "Id", "Id", point.TypeId);
            return View(point);
        }

        // GET: Points/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var point = await _context.Points.FindAsync(id);
            if (point == null)
            {
                return NotFound();
            }
            //ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name", point.CityId);
            ViewData["TypeId"] = new SelectList(_context.Types, "Id", "Name", point.TypeId);
            return View(point);
        }

        // POST: Points/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Lat,Lan,Description,TypeId,Name,CityId,Geom")] Point point)
        {
            if (id != point.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(point);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PointExists(point.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            //ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Id", point.CityId);
            ViewData["TypeId"] = new SelectList(_context.Types, "Id", "Id", point.TypeId);
            return View(point);
        }

        // GET: Points/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var point = await _context.Points
                //.Include(p => p.City)
                .Include(p => p.Type)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (point == null)
            {
                return NotFound();
            }

            return View(point);
        }

        // POST: Points/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var point = await _context.Points.FindAsync(id);
            _context.Points.Remove(point);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PointExists(int id)
        {
            return _context.Points.Any(e => e.Id == id);
        }

        public async Task<IActionResult> GetMap()
        {
            var points = await _context.Points.ToListAsync();
            return View(points);
        }

        [HttpGet]
        public async Task<IActionResult> GetClusteringMap()
        {
            var KMeansQuery = "WITH js as ( select \"Id\",\"Lat\",\"Lan\",\"Name\", \"Description\", \"TypeId\", \"Geom\", ST_ClusterKMeans(\"Geom\",5) over () AS ClusterGroup FROM public.\"Points\") select * from js";
            var KMeansNew = await _context.Points.FromSqlRaw(KMeansQuery).ToListAsync();

            var maxClusterNumber = KMeansNew.Select(p => p.ClusterGroup).Max() + 1;
            KMeansNew.Where(p => p.ClusterGroup == null).ToList().ForEach(p => p.ClusterGroup = maxClusterNumber);
            var KMeans = new TypesCities();
            KMeans.Points = _context.Points.Where(p => p.Id % 4 != 0).ToList();
            return View(KMeans);
        }

        [HttpGet]
        public async Task<IActionResult> GetClusteringMapDbScan()
        {
            var DbScanQuery = "with js as (SELECT \"Id\",\"Lat\",\"Lan\",\"Name\", \"Description\", \"TypeId\" , \"Geom\", ST_ClusterDBSCAN(\"Geom\", eps := 0.5, minpoints := 3) over () AS ClusterGroup FROM public.\"Points\") select * from js";
            var dbscanNew = await _context.Points.FromSqlRaw(DbScanQuery).ToListAsync();

            var maxClusterNumber = dbscanNew.Select(p => p.ClusterGroup).Max() + 1;
            dbscanNew.Where(p => p.ClusterGroup == null).ToList().ForEach(p => p.ClusterGroup = maxClusterNumber);
            var dbScan = new TypesCities();
            dbScan.Points = dbscanNew;
            return View("GetClusteringMap", dbScan);
        }






        [HttpGet]
        public async Task<IActionResult> GetClusteringMapDbScanTypeCity(int typeId, int cityId)
        {
            var f = "select ST_X(ST_Centroid(ST_Collect(ARRAY( SELECT \"Geom\" FROM public.\"Points\" where \"TypeId\" = " + typeId + " And ST_Contains((select \"Geom\" from public.\"Cities\" where \"Id\" = " + cityId + "),\"Geom\") )))) as Lan, ST_Y(ST_Centroid(ST_Collect(ARRAY( SELECT \"Geom\" FROM public.\"Points\" where \"TypeId\" = " + typeId + " And ST_Contains((select \"Geom\" from public.\"Cities\" where \"Id\" = " + cityId  + "),\"Geom\") )))) as Lat, 1 as Id";
   

            var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = f;
            _context.Database.OpenConnection();

            using (var rdr = command.ExecuteReader())
            {
                while (rdr.Read())
                {
                    return Json(new { Lan = rdr.GetDouble(0), Lat = rdr.GetDouble(1) });              // Do somthing with this rows string, for example to put them in to a list
                }
            }

            return Ok();
        }
        
        [HttpGet]
        public IActionResult GetClusteringMapDbScanApi(int typeId, int cityId)
        {
            //var f = "SELECT ST_GeomFromText(ST_AsText( ST_Centroid((select cluster_group from (SELECT Id, ST_Collect(\"Geom\") AS cluster_group, array_agg(id) AS ids_in_cluster FROM ( SELECT \"Id\", ST_ClusterDBSCAN(\"Geom\", eps := 0.5, minpoints := 5) over () AS Id, \"Geom\" FROM \"Points\" where \"TypeId\" = " + typeId + " And ST_Contains((select \"Geom\" from public.\"Cities\" where \"Id\" = " + cityId + "),\"Geom\")  ) sq GROUP BY Id) as gg))),4326)";
            
            var g = "select ST_X(ST_GeomFromText(ST_AsText( ST_Centroid((select cluster_group from (SELECT Id, ST_Collect(\"Geom\") AS cluster_group, array_agg(id) AS ids_in_cluster FROM ( SELECT \"Id\", ST_ClusterDBSCAN(\"Geom\", eps := 0.5, minpoints := 5) over () AS Id, \"Geom\" FROM \"Points\" where \"TypeId\" = " + typeId + " And ST_Contains((select \"Geom\" from public.\"Cities\" where \"Id\" = " + cityId + "),\"Geom\")  ) sq GROUP BY Id) as gg))),4326)), ST_Y(ST_GeomFromText(ST_AsText( ST_Centroid((select cluster_group from (SELECT Id, ST_Collect(\"Geom\") AS cluster_group, array_agg(id) AS ids_in_cluster FROM ( SELECT \"Id\", ST_ClusterDBSCAN(\"Geom\", eps := 0.5, minpoints := 5) over () AS Id, \"Geom\" FROM \"Points\" where \"TypeId\" = " + typeId + " And ST_Contains((select \"Geom\" from public.\"Cities\" where \"Id\" = " + cityId + "),\"Geom\")  ) sq GROUP BY Id) as gg))),4326))";

            var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = g;
            _context.Database.OpenConnection();

            var r = new Random();
            using (var rdr = command.ExecuteReader())
            {
                while (rdr.Read())
                {
                    return Json(new { Lan = rdr.GetDouble(0) + 0.001 * r.NextDouble(), Lat = rdr.GetDouble(1) - 0.001 * r.NextDouble() });              // Do somthing with this rows string, for example to put them in to a list
                }
            }

            return Ok();
        }




        [HttpGet]
        public async Task<IActionResult> GetClusteringMapK_Medoidos()
        {
            var dbScan = new TypesCities();
            dbScan.Points = _context.Points.Where(p => p.Id % 4 != 2).ToList();
            return View("GetClusteringMap", dbScan);
        }


        public async Task<IActionResult> GetClusteringMapOptics()
        {
            var dbScan = new TypesCities();
            dbScan.Points = _context.Points.Where(p => p.Id % 4 != 3).ToList();
            return View("GetClusteringMap", dbScan);
        }

        [HttpPost]
        public async Task<IActionResult> GetClusteringMap(TypesCities input)
        {
            var Object = await ForGetClusteringMap(input);
            return View(Object);
        }

        public async Task<IActionResult> GetThisMap(int id)
        {
            var points = await _context.Points.Where(p => p.Id == id).ToListAsync();
            return View("GetMap", points);
        }

        public async Task<IActionResult> GetJson()
        {
            var points = await _context.Points.Take(20).Select(p => new { Lan = p.Lan, Lat = p.Lat }).ToListAsync();
            return Json(points);
        }

        public async Task<IActionResult> PointsInCity(int id)
        {
            var DbScanQuery = "SELECT * FROM public.\"Points\" where ST_Contains((select \"Geom\" from public.\"Cities\" where \"Id\" = " + id + "),\"Geom\")";
            var SubGruopPoints = await _context.Points.FromSqlRaw(DbScanQuery).Include(p => p.Type).ToListAsync();
            var sub = SubGruopPoints.Select(p => new { Name = p.Name,  Lat = p.Lat, Lan = p.Lan, Type = p.TypeId}).ToList();
            return Json(sub);
        }

        
        public async Task<IActionResult> City(int id)
        {
            var city = await _context.Cities
                .FirstOrDefaultAsync(m => m.Id == id);
            return Json(new { city = city.TextPoly });
        }

        public async Task<IActionResult> Citye(string name)
        {
            var city = await _context.Cities
                .FirstOrDefaultAsync(m => m.Name == name);
            return Json(new { city = city.TextPoly });
        }

        public IActionResult NewCluster()
        {
            return View("GetStatisticsMap");
        }



        private async Task<TypesCities> ForGetClusteringMap(TypesCities input)
        {
            var points = await _context.Points.Include(p => p.Type)
                //.Include(p => p.City)
                .Where(p => input.Types == null || !input.Types.Any())
                .Where(p => input.Cities == null || input.Cities.Select(c => c.Value).Contains(p.TypeId.ToString()))
                .ToListAsync();
            return new TypesCities
            {
                Points = points,
                Types = input.Types,
                Cities = new SelectList(_context.Cities, "Id", "Name")
            };
        }
    }
}
