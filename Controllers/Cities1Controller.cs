using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Clustering.Data;
using Clustering.Models;

namespace Clustering.Controllers
{
    public class Cities1Controller : Controller
    {
        private readonly ApplicationDbContext _context;

        public Cities1Controller(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Cities1
        public async Task<IActionResult> Index()
        {
            return View(await _context.Cities.ToListAsync());
        }
        
        // GET: Cities1
        public async Task<IActionResult> GetAll()
        {
            return Json(await _context.Cities.Select(c => new { 
                Id = c.Id, 
                Name = c.Name, 
                TextPoly = c.TextPoly }).ToListAsync());
        }

        // GET: Cities1/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await _context.Cities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (city == null)
            {
                return NotFound();
            }

            return View(city);
        }

        // GET: Cities1/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cities1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Geom,TextFormat,TextPoly")] City city)
        {
            if (ModelState.IsValid)
            {
                //city.TextPoly = city.TextFormat.Replace("MULTIPOLYGON", "")
                //    .Replace("(((", "[[")
                //    .Replace(")))", "]]")
                //    .Replace(", ", ",")
                //    .Replace(" ,", ",")
                //    .Replace(",", "],[")
                //    .Replace(" ", ",")
                //    ;

                //city.TextFormat = "MULTIPOLYGON" + city.TextPoly
                //    .Replace("[[", "(((")
                //    .Replace("]]", ")))")
                //    .Replace(", ", ",")
                //    .Replace(" ,", ",")
                //    .Replace("],[", "*")
                //    .Replace(",", " ")
                //    .Replace("*", ",")
                //    ;

                _context.Add(city);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(city);
        }

        // GET: Cities1/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await _context.Cities.FindAsync(id);
            if (city == null)
            {
                return NotFound();
            }
            return View(city);
        }

        // POST: Cities1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Geom,TextFormat,TextPoly")] City city)
        {
            if (id != city.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(city);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CityExists(city.Id))
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
            return View(city);
        }

        // GET: Cities1/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await _context.Cities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (city == null)
            {
                return NotFound();
            }

            return View(city);
        }

        // POST: Cities1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var city = await _context.Cities.FindAsync(id);
            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CityExists(int id)
        {
            return _context.Cities.Any(e => e.Id == id);
        }
    }
}
