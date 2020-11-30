using Clustering.Data;
using Clustering.Dto;
using Clustering.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
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
            //var t = _context.Points.Where(p => p.Id >= 1).ToList();
            //_context.RemoveRange(t);
            //_context.SaveChanges();
        }

        // GET: Points
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Points.Include(p => p.Type);
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
            var dbScan = new TypesCities();
            dbScan.Points = _context.Points.Where(p => p.Id % 4 != 0).ToList();
            return View(dbScan);
        }

        [HttpGet]
        public async Task<IActionResult> GetClusteringMapDbScan()
        {
            var dbScan = new TypesCities();
            dbScan.Points = _context.Points.Where(p => p.Id % 4 != 1).ToList();
            return View("GetClusteringMap", dbScan);
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
