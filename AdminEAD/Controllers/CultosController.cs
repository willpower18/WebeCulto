using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AdminEAD.Models;
using Microsoft.AspNetCore.Authorization;
using AdminEAD.Coomon;
using AdminEAD.ViewModels;

namespace AdminEAD.Controllers
{
    [Authorize]
    public class CultosController : Controller
    {
        private readonly eCultoContext _context;

        public CultosController(eCultoContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var idIgreja = User.Claims.Where(c => c.Type == "IdIgreja").Select(c => c.Value).SingleOrDefault();
                List<Culto> cultos = new List<Culto>();
                DateTime hoje = Util.BrasilDate();
                if (string.IsNullOrEmpty(idIgreja))
                {
                    cultos = await _context.Culto.Where(c => c.DataHora >= hoje).OrderBy(c => c.DataHora).Include(c => c.IdIgrejaNavigation).ToListAsync(); ;
                }
                else
                {
                    int igreja = Convert.ToInt32(idIgreja);
                    cultos = await _context.Culto.Where(c => c.DataHora >= hoje & c.IdIgreja == igreja).OrderBy(c => c.DataHora).Include(c => c.IdIgrejaNavigation).ToListAsync(); ;
                }
                return View(cultos);
            }
            catch
            {
                return NotFound();
            }
        }

        
        public IActionResult Create()
        {
            try
            {
                var idIgreja = User.Claims.Where(c => c.Type == "IdIgreja").Select(c => c.Value).SingleOrDefault();
                if (string.IsNullOrEmpty(idIgreja))
                {
                    ViewData["IdIgreja"] = new SelectList(_context.Igreja.Where(i => i.Ativo == 1), "IdIgreja", "Nome");
                }
                else
                {
                    int igreja = Convert.ToInt32(idIgreja);
                    ViewData["IdIgreja"] = new SelectList(_context.Igreja.Where(i => i.Ativo == 1 & i.IdIgreja == igreja), "IdIgreja", "Nome");
                }
                return View();
            }
            catch
            {
                return NotFound();
            }
           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCulto,IdIgreja,Nome,DataHora,Preletor,Lotacao")] Culto culto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(culto);
                    await _context.SaveChangesAsync();
                    ViewBag.AlertMessage = Util.RenderAlert("Cadastrado com sucesso!", "Sucesso!", "SUCCESS");
                    ViewData["IdIgreja"] = new SelectList(_context.Igreja.Where(i => i.IdIgreja == culto.IdIgreja), "IdIgreja", "Nome", culto.IdIgreja);
                    return View(culto);
                }
                else
                {
                    ViewBag.AlertMessage = Util.RenderAlert("Verifique o preenchimento dos campos e tente novamente!", "Atenção!", "WARNING");
                    ViewData["IdIgreja"] = new SelectList(_context.Igreja.Where(i => i.IdIgreja == culto.IdIgreja), "IdIgreja", "Nome", culto.IdIgreja);
                    return View(culto);
                }
                
            }
            catch(Exception ex)
            {
                ViewBag.AlertMessage = Util.RenderAlert("Ocorreu um erro inesperado, encaminhe a mensagem de erro ao suporte: " + ex.Message, "Erro!", "ERROR");
                ViewData["IdIgreja"] = new SelectList(_context.Igreja.Where(i => i.IdIgreja == culto.IdIgreja), "IdIgreja", "Nome", culto.IdIgreja);
                return View(culto);
            }
        }

        // GET: Cultos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var culto = await _context.Culto.FindAsync(id);
                if (culto == null)
                {
                    return NotFound();
                }
                ViewData["IdIgreja"] = new SelectList(_context.Igreja.Where(i => i.IdIgreja == culto.IdIgreja), "IdIgreja", "Nome", culto.IdIgreja);
                return View(culto);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCulto,IdIgreja,Nome,DataHora,Preletor,Lotacao")] Culto culto)
        {
            if (id != culto.IdCulto)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    _context.Update(culto);
                    await _context.SaveChangesAsync();
                    ViewBag.AlertMessage = Util.RenderAlert("Cadastrado com sucesso!", "Sucesso!", "SUCCESS");
                    ViewData["IdIgreja"] = new SelectList(_context.Igreja.Where(i => i.IdIgreja == culto.IdIgreja), "IdIgreja", "Nome", culto.IdIgreja);
                    return View(culto);
                }
                else
                {
                    ViewBag.AlertMessage = Util.RenderAlert("Verifique o preenchimento dos campos e tente novamente!", "Atenção!", "WARNING");
                    ViewData["IdIgreja"] = new SelectList(_context.Igreja.Where(i => i.IdIgreja == culto.IdIgreja), "IdIgreja", "Nome", culto.IdIgreja);
                    return View(culto);
                }
            }
            catch(Exception ex)
            {
                ViewBag.AlertMessage = Util.RenderAlert("Ocorreu um erro inesperado, encaminhe a mensagem de erro ao suporte: " + ex.Message, "Erro!", "ERROR");
                ViewData["IdIgreja"] = new SelectList(_context.Igreja.Where(i => i.IdIgreja == culto.IdIgreja), "IdIgreja", "Nome", culto.IdIgreja);
                return View(culto);
            }
        }

        // GET: Cultos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var culto = await _context.Culto
                    .Include(c => c.IdIgrejaNavigation)
                    .FirstOrDefaultAsync(m => m.IdCulto == id);
                if (culto == null)
                {
                    return NotFound();
                }

                return View(culto);
            }
            catch
            {
                return NotFound();
            }
        }

        // POST: Cultos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var culto = await _context.Culto.FindAsync(id);
            if(culto == null)
            {
                return NotFound();
            }

            List<Participacao> participacoes = await _context.Participacao.Where(p => p.IdCulto == culto.IdCulto).ToListAsync();
            if(participacoes.Count > 0)
            {
                foreach(Participacao p in participacoes)
                {
                    _context.Participacao.Remove(p);
                }

                await _context.SaveChangesAsync();
                _context.Culto.Remove(culto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                _context.Culto.Remove(culto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }

        private bool CultoExists(int id)
        {
            return _context.Culto.Any(e => e.IdCulto == id);
        }

        public IActionResult AddPresenca(int Id)
        {
            try
            {

                ViewData["IdCulto"] = new SelectList(_context.Culto.Where(c => c.IdCulto == Id), "IdCulto", "Nome");
                return View();
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPresenca([Bind("IdParticipacao,IdCulto,Nome,Telefone,ChaveApp,QtdCriancas,QtdAdultos")] Participacao participacao)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var idIgreja = User.Claims.Where(c => c.Type == "IdIgreja").Select(c => c.Value).SingleOrDefault();
                    int idIgj = Convert.ToInt32(idIgreja);
                    int capacidade = await _context.Igreja.Where(i => i.IdIgreja == idIgj).Select(i => i.Capacidade).FirstOrDefaultAsync();
                    Culto culto = await _context.Culto.FindAsync(participacao.IdCulto);
                    int qtd = culto.Lotacao + participacao.QtdAdultos;
                    qtd += participacao.QtdCriancas;
                    if(qtd > capacidade)
                    {
                        ViewBag.AlertMessage = Util.RenderAlert("Não foi possível salvar pois o limite da capacidade da igreja foi atingido!", "Atenção!", "WARNING");
                        ViewData["IdCulto"] = new SelectList(_context.Culto.Where(c => c.IdCulto == participacao.IdCulto), "IdCulto", "Nome");
                        return View(participacao);
                    }

                    _context.Add(participacao);
                    await _context.SaveChangesAsync();
                    int qtdPessoas = participacao.QtdAdultos + participacao.QtdCriancas;
                    culto.Lotacao += qtdPessoas;
                    _context.Update(culto);
                    await _context.SaveChangesAsync();
                    ViewBag.AlertMessage = Util.RenderAlert("Participação Incluída com sucesso!", "Sucesso!", "SUCCESS");
                    ViewData["IdCulto"] = new SelectList(_context.Culto.Where(c => c.IdCulto == participacao.IdCulto), "IdCulto", "Nome");
                    return View(participacao);
                }
                else
                {
                    ViewBag.AlertMessage = Util.RenderAlert("Verifique o preenchimento dos campos e tente novamente!", "Atenção!", "WARNING");
                    ViewData["IdCulto"] = new SelectList(_context.Culto.Where(c => c.IdCulto == participacao.IdCulto), "IdCulto", "Nome");
                    return View(participacao);
                }
                
            }
            catch(Exception ex)
            {
                ViewBag.AlertMessage = Util.RenderAlert("Ocorreu um erro inesperado, encaminhe a mensagem de erro ao suporte: " + ex.Message, "Erro!", "ERROR");
                return View(participacao);
            }
        }

        public async Task<IActionResult> RemovePresenca(int Id)
        {
            try
            {
                Culto culto = await _context.Culto.FindAsync(Id);
                if (culto == null)
                {
                    return NotFound();
                }
                
                var idIgreja = User.Claims.Where(c => c.Type == "IdIgreja").Select(c => c.Value).SingleOrDefault();
                if (!string.IsNullOrEmpty(idIgreja))
                {
                    int igreja = Convert.ToInt32(idIgreja);
                    if(igreja != culto.IdIgreja)
                    {
                        return RedirectToAction("AccessDenied","Account");
                    }
                }

                List<Participacao> participacoes = await _context.Participacao.Where(p => p.IdCulto == culto.IdCulto).ToListAsync();

                ViewBag.NomeEvento = culto.Nome;
                ViewBag.DataEvento = culto.DataHora.ToString("dd/MM/yyyy HH:mm");
                return View(participacoes);
            }
            catch
            {
                return NotFound();
            }
        }

        public async Task<JsonResult> RemoverPresenca(int Id)
        {
            if(Id == 0)
            {
                var retorno = new
                {
                    res = 0,
                    msg = "Id Inválido"
                };

                return Json(retorno);
            }
            else
            {
                Participacao part = await _context.Participacao.FindAsync(Id);
                if(part == null)
                {
                    var retorno = new
                    {
                        res = 0,
                        msg = "Objeto não encontrado"
                    };

                    return Json(retorno);
                }
                else
                {
                    _context.Remove(part);
                    await _context.SaveChangesAsync();
                    var retorno = new
                    {
                        res = 1,
                        msg = "Participação Removida Com Sucesso!"
                    };

                    return Json(retorno);
                }
            }
        }
    }
}
