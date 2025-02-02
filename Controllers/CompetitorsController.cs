﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using simhoppsystemet.Data;
using simhoppsystemet.Models;

namespace simhoppsystemet.Controllers
{
    public class CompetitorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CompetitorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Competitors
        public async Task<IActionResult> Index()
        {

            return View(await _context.Competitor.ToListAsync());
        }


        // ------------------------- SEARCH -----------------------------------------//

        // GET: Competitions/ShowSearchForm
        public async Task<IActionResult> ShowSearchForm()
        {
            return View();
        }
        // POST: Competitions/ShowSearchResult
        public async Task<IActionResult> ShowSearchResults(string SearchPhrase)
        {
            return View("SearchResult", await _context.Competitor.Where(j => j.Name.Contains(SearchPhrase)).ToListAsync());
        }

        // GET: Competitors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var competitor = await _context.Competitor
                .FirstOrDefaultAsync(m => m.Id == id);
            if (competitor == null)
            {
                return NotFound();
            }

            IList<Competition> competitions = GetCompetitions(id);

            ViewData["competitions"] = competitions;
            return View(competitor);
        }
  



        // GET: Competitors/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Competitors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Age,Name,Gender,Organization")] Competitor competitor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(competitor);
                
                
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(competitor);
        }

        // GET: Competitors/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var competitor = await _context.Competitor.FindAsync(id);
            if (competitor == null)
            {
                return NotFound();
            }
            return View(competitor);
        }

        // POST: Competitors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Age,Name,Gender,Organization")] Competitor competitor)
        {
            if (id != competitor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(competitor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompetitorExists(competitor.Id))
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
            return View(competitor);
        }

        // GET: Competitors/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var competitor = await _context.Competitor
                .FirstOrDefaultAsync(m => m.Id == id);
            if (competitor == null)
            {
                return NotFound();
            }

            return View(competitor);
        }

        // POST: Competitors/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var competitor = await _context.Competitor.FindAsync(id);
            _context.Competitor.Remove(competitor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompetitorExists(int id)
        {
            return _context.Competitor.Any(e => e.Id == id);
        }

        private IList<Competition> GetCompetitions(int? id)
        {
            IList<CompetitionCompetitor> competitioncompetitors = _context.CompetitionCompetitor.Where(j => j.CompetitorId == id).ToList();
            IList<Competition> competition = _context.Competition.ToList(); //Full list of all competitions
            IList<Competition> competitions = new List<Competition>(); //Empty list with loads of space and potential!

            competitions = competitioncompetitors.Select(cc => competition.First(c => c.Id == cc.CompetitionId)).ToList();
            return competitions;
        }
    }
}
