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
    [Authorize(Roles = "Administrador")]
    public class IgrejasController : Controller
    {
        private readonly eCultoContext _context;

        public IgrejasController(eCultoContext context)
        {
            _context = context;
        }

        // GET: Igrejas
        public async Task<IActionResult> Index()
        {
            return View(await _context.Igreja.ToListAsync());
        }

        // GET: Igrejas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var igreja = await _context.Igreja
                .FirstOrDefaultAsync(m => m.IdIgreja == id);
            if (igreja == null)
            {
                return NotFound();
            }

            return View(igreja);
        }

        // GET: Igrejas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Igrejas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdIgreja,Nome,Endereco,Numero,Bairro,Cidade,Uf,Responsavel,Tradicao,Capacidade,Ativo")] Igreja igreja)
        {
            if (ModelState.IsValid)
            {
                _context.Add(igreja);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(igreja);
        }

        // GET: Igrejas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var igreja = await _context.Igreja.FindAsync(id);
            if (igreja == null)
            {
                return NotFound();
            }
            return View(igreja);
        }

        // POST: Igrejas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdIgreja,Nome,Endereco,Numero,Bairro,Cidade,Uf,Responsavel,Tradicao,Capacidade,Ativo")] Igreja igreja)
        {
            if (id != igreja.IdIgreja)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(igreja);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IgrejaExists(igreja.IdIgreja))
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
            return View(igreja);
        }

        // GET: Igrejas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var igreja = await _context.Igreja
                .FirstOrDefaultAsync(m => m.IdIgreja == id);
            if (igreja == null)
            {
                return NotFound();
            }

            return View(igreja);
        }

        // POST: Igrejas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var igreja = await _context.Igreja.FindAsync(id);
            _context.Igreja.Remove(igreja);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IgrejaExists(int id)
        {
            return _context.Igreja.Any(e => e.IdIgreja == id);
        }
    }
}
