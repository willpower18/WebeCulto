using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AdminEAD.Models;
using Microsoft.AspNetCore.Authorization;

namespace AdminEAD.Controllers
{
    [Authorize]
    public class ParticipacaoController : Controller
    {
        private readonly eCultoContext _context;

        public ParticipacaoController(eCultoContext context)
        {
            _context = context;
        }

        // GET: Participacao
        public async Task<IActionResult> Index()
        {
            var eCultoContext = _context.Participacao.Include(p => p.IdCultoNavigation);
            return View(await eCultoContext.ToListAsync());
        }

        // GET: Participacao/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var participacao = await _context.Participacao
                .Include(p => p.IdCultoNavigation)
                .FirstOrDefaultAsync(m => m.IdParticipacao == id);
            if (participacao == null)
            {
                return NotFound();
            }

            return View(participacao);
        }

        // GET: Participacao/Create
        public IActionResult Create()
        {
            ViewData["IdCulto"] = new SelectList(_context.Culto, "IdCulto", "Nome");
            return View();
        }

        // POST: Participacao/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdParticipacao,IdCulto,Nome,Telefone,ChaveApp,QtdCriancas,QtdAdultos")] Participacao participacao)
        {
            if (ModelState.IsValid)
            {
                _context.Add(participacao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdCulto"] = new SelectList(_context.Culto, "IdCulto", "Nome", participacao.IdCulto);
            return View(participacao);
        }

        // GET: Participacao/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var participacao = await _context.Participacao.FindAsync(id);
            if (participacao == null)
            {
                return NotFound();
            }
            ViewData["IdCulto"] = new SelectList(_context.Culto, "IdCulto", "Nome", participacao.IdCulto);
            return View(participacao);
        }

        // POST: Participacao/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdParticipacao,IdCulto,Nome,Telefone,ChaveApp,QtdCriancas,QtdAdultos")] Participacao participacao)
        {
            if (id != participacao.IdParticipacao)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(participacao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ParticipacaoExists(participacao.IdParticipacao))
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
            ViewData["IdCulto"] = new SelectList(_context.Culto, "IdCulto", "Nome", participacao.IdCulto);
            return View(participacao);
        }

        // GET: Participacao/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var participacao = await _context.Participacao
                .Include(p => p.IdCultoNavigation)
                .FirstOrDefaultAsync(m => m.IdParticipacao == id);
            if (participacao == null)
            {
                return NotFound();
            }

            return View(participacao);
        }

        // POST: Participacao/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var participacao = await _context.Participacao.FindAsync(id);
            _context.Participacao.Remove(participacao);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ParticipacaoExists(int id)
        {
            return _context.Participacao.Any(e => e.IdParticipacao == id);
        }
    }
}
