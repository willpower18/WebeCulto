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
using System.Text;
using Rotativa.AspNetCore;

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
        public async Task<IActionResult> Index(int Id)
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
                    if (igreja != culto.IdIgreja)
                    {
                        return RedirectToAction("AccessDenied", "Account");
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

        public async Task<JsonResult> Confirma(int Id)
        {
            if (Id == 0)
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
                if (part == null)
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
                    if(part.Confirmado == 1)
                    {
                        part.Confirmado = 0;
                    }
                    else
                    {
                        part.Confirmado = 1;
                    }                    
                    _context.Update(part);
                    await _context.SaveChangesAsync();
                    var retorno = new
                    {
                        res = 1,
                        conf = part.Confirmado,
                        msg = "Operação Realizada Com Sucesso Sucesso!"
                    };

                    return Json(retorno);
                }
            }
        }

        public async Task<IActionResult> Cultos()
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

        public async Task<IActionResult> Print(int Id)
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
                    Igreja igj = await _context.Igreja.FindAsync(culto.IdIgreja);
                    List<Participacao> participacoes = await _context.Participacao.Where(p => p.IdCulto == culto.IdCulto & p.Confirmado == 1)
                        .OrderBy(c => c.Nome)
                        .ToListAsync();
                    StringBuilder html = new StringBuilder();
                    if(participacoes.Count > 0)
                    {
                        int totalPessoas = 0;
                        foreach(Participacao p in participacoes)
                        {
                            totalPessoas = p.QtdAdultos + p.QtdCriancas;
                            totalPessoas = totalPessoas - 1;
                            html.Append("<tr>");
                            html.Append("<td>" + p.Nome + "</td>");
                            html.Append("<td>" + p.Telefone + "</td>");
                            html.Append("<td>" + totalPessoas + "</td>");
                            html.Append("</tr>");
                        }
                    }
                    else
                    {
                        html.Append("<tr>");
                        html.Append("<td colspan=\"3\" class=\"text-center\">Não houve participantes</td>");
                        html.Append("</tr>");
                    }
                    CultoPresenca model = new CultoPresenca
                    {
                        igreja = igj,
                        culto = culto,
                        tabela = html.ToString()
                    };

                    var relatorio = new ViewAsPdf("Print")
                    {
                        PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                        PageSize = Rotativa.AspNetCore.Options.Size.A4,
                        PageMargins = { Left = 10, Right = 10 },
                        Model = model
                    };
                    return relatorio;
                }
                else
                {
                    return NotFound();
                }
            }
            catch
            {
                return NotFound();
            }
        }
    }
}
