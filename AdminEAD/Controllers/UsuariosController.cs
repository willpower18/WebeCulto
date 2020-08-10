using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AdminEAD.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using AdminEAD.Coomon;

namespace AdminEAD.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : Controller
    {
        private readonly eCultoContext _context;

        public UsuariosController(eCultoContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Index()
        {
            var gestorObrasContext = _context.Usuario.Where(u => u.Ativo == 1).Include(u => u.IdTipoUsuarioNavigation);
            return View(await gestorObrasContext.ToListAsync());
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            ViewData["IdTipoUsuario"] = new SelectList(_context.TipoUsuario, "IdTipoUsuario", "Descricao");
            ViewData["IdIgreja"] = new SelectList(_context.Igreja.Where(d => d.Ativo == 1), "IdIgreja", "Nome");
            return View();
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdUsuario,IdTipoUsuario,Nome,Login,Senha,Ativo,IdIgreja")] Usuario usuario)
        {
            try
            {
                string Hash = Util.GetHash(usuario.Senha);
                usuario.Senha = Hash;
                usuario.Ativo = 1;
                if (ModelState.IsValid)
                {
                    _context.Add(usuario);
                    await _context.SaveChangesAsync();
                    ViewData["IdTipoUsuario"] = new SelectList(_context.TipoUsuario, "IdTipoUsuario", "Descricao", usuario.IdTipoUsuario);
                    ViewData["IdIgreja"] = new SelectList(_context.Igreja.Where(d => d.Ativo == 1), "IdIgreja", "Nome", usuario.IdIgreja);
                    ViewBag.AlertMessage = Util.RenderAlert("Usuário cadastrado com sucesso!", "Sucesso!", "SUCCESS");
                    return View(usuario);
                }
                else
                {
                    ViewData["IdTipoUsuario"] = new SelectList(_context.TipoUsuario, "IdTipoUsuario", "Descricao", usuario.IdTipoUsuario);
                    ViewData["IdIgreja"] = new SelectList(_context.Igreja.Where(d => d.Ativo == 1), "IdIgreja", "Nome", usuario.IdIgreja);
                    ViewBag.AlertMessage = Util.RenderAlert("Veriique o preenchimento dos campos e tente novamente!", "Atenção!", "WARNING");
                    return View(usuario);
                }
            }
            catch
            {
                ViewData["IdTipoUsuario"] = new SelectList(_context.TipoUsuario, "IdTipoUsuario", "Descricao", usuario.IdTipoUsuario);
                ViewData["IdIgreja"] = new SelectList(_context.Igreja.Where(d => d.Ativo == 1), "IdIgreja", "Nome", usuario.IdIgreja);
                ViewBag.AlertMessage = Util.RenderAlert("Algo deu errado, tente novamente mais tarde!", "Erro!", "ERROR");
                return View(usuario);
            }
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            ViewData["IdTipoUsuario"] = new SelectList(_context.TipoUsuario, "IdTipoUsuario", "Descricao", usuario.IdTipoUsuario);
            ViewData["IdIgreja"] = new SelectList(_context.Igreja.Where(d => d.Ativo == 1), "IdIgreja", "Nome", usuario.IdIgreja);
            return View(usuario);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdUsuario,IdTipoUsuario,Nome,Login,Senha,Ativo,IdIgreja")] Usuario usuario)
        {
            if (id != usuario.IdUsuario)
            {
                return NotFound();
            }
            try
            {
                if (usuario.Senha != null)
                {
                    string Hash = Util.GetHash(usuario.Senha);
                    usuario.Senha = Hash;
                }
                else
                {
                    var senha = _context.Usuario.Where(u => u.IdUsuario == id).Select(u => u.Senha).First();
                    usuario.Senha = senha;
                }

                if (ModelState.IsValid)
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                    
                    ViewData["IdTipoUsuario"] = new SelectList(_context.TipoUsuario, "IdTipoUsuario", "Descricao", usuario.IdTipoUsuario);
                    ViewData["IdIgreja"] = new SelectList(_context.Igreja.Where(d => d.Ativo == 1), "IdIgreja", "Nome", usuario.IdIgreja);
                    ViewBag.AlertMessage = Util.RenderAlert("Usuário atualizado com sucesso!", "Sucesso!", "SUCCESS");
                    return View(usuario);
                }
                else
                {
                    ViewData["IdTipoUsuario"] = new SelectList(_context.TipoUsuario, "IdTipoUsuario", "Descricao", usuario.IdTipoUsuario);
                    ViewData["IdIgreja"] = new SelectList(_context.Igreja.Where(d => d.Ativo == 1), "IdIgreja", "Nome", usuario.IdIgreja);
                    ViewBag.AlertMessage = Util.RenderAlert("Veriique o preenchimento dos campos e tente novamente!", "Atenção!", "WARNING");
                    return View(usuario);
                }
            }
            catch
            {
                ViewData["IdTipoUsuario"] = new SelectList(_context.TipoUsuario, "IdTipoUsuario", "Descricao", usuario.IdTipoUsuario);
                ViewData["IdIgreja"] = new SelectList(_context.Igreja.Where(d => d.Ativo == 1), "IdIgreja", "Nome", usuario.IdIgreja);
                ViewBag.AlertMessage = Util.RenderAlert("Algo deu errado, tente novamente mais tarde!", "Erro!", "ERROR");
                return View(usuario);
            }

        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            var Resultado = 0;
            Usuario usuario = new Usuario();
            TipoUsuario tipo = new TipoUsuario();
            try
            {
                var userId = User.Claims.Where(c => c.Type == "Idusuario").Select(c => c.Value).SingleOrDefault();
                usuario = _context.Usuario.Find(Convert.ToInt32(userId));
                if (usuario != null)
                {
                    tipo = _context.TipoUsuario.Find(usuario.IdTipoUsuario);
                }
            }
            catch
            {
                return Json(Resultado);
            }

            if (tipo.Nome == "Administrador")
            {
                if (id == 0)
                {
                    return Json(Resultado);
                }
                else
                {
                    Usuario user = new Usuario();
                    try
                    {
                        user = _context.Usuario.Find(id);
                        if (user == null)
                        {
                            return Json(Resultado);
                        }
                        else
                        {
                            user.Ativo = 0;
                            _context.Update(user);
                            _context.SaveChanges();
                            Resultado = 1;
                            return Json(Resultado);
                        }
                    }
                    catch
                    {
                        return Json(Resultado);
                    }
                }
            }
            else
            {
                Resultado = 2;
                return Json(Resultado);
            }
        }

        //GET Metodo Para Alteração de Senha
        public async Task<IActionResult> ChangePwd(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.Claims.Where(c => c.Type == "Idusuario").Select(c => c.Value).SingleOrDefault();

            if (Convert.ToInt32(userId) == id)
            {
                Usuario usuario = new Usuario();
                usuario = await _context.Usuario.FindAsync(id);
                if (usuario != null)
                {
                    return View(usuario);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return RedirectToAction("AccessDenied", "Account");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePwd(int id, string Senha, string NovaSenha, string ConfirmSenha)
        {
            if (id != 0 && Senha != null && NovaSenha != null && ConfirmSenha != null)
            {
                Senha = Util.GetHash(Senha);
                NovaSenha = Util.GetHash(NovaSenha);
                ConfirmSenha = Util.GetHash(ConfirmSenha);

                Usuario usuario = new Usuario();
                usuario = await _context.Usuario.FindAsync(id);
                if (usuario != null)
                {
                    if (Senha == usuario.Senha)
                    {
                        if (NovaSenha == ConfirmSenha)
                        {
                            try
                            {
                                usuario.Senha = NovaSenha;
                                _context.Update(usuario);
                                await _context.SaveChangesAsync();
                                return RedirectToAction("Index", "Home");
                            }
                            catch
                            {
                                ViewBag.Message = Util.RenderAlert("Algo Deu Errado, Tente Novamente Mais Tarde.", "Erro!", "ERROR");
                                return View(usuario);
                            }
                        }
                        else
                        {
                            ViewBag.Message = Util.RenderAlert("Nova Senha e Confirmação da Senha Não Conferem.", "Atenção!", "WARNING");
                            return View(usuario);
                        }
                    }
                    else
                    {
                        ViewBag.Message = Util.RenderAlert("A Senha Atual Não Confere.", "Atenção!", "WARNING");
                        return View(usuario);
                    }
                }
                else
                {
                    ViewBag.Message = Util.RenderAlert("Usuário Não Encontrado, Não é Possível Alterar a Senha.", "Erro!", "ERROR");
                    return View(usuario);
                }
            }
            else
            {
                ViewBag.Message = Util.RenderAlert("Preencha Todos os Dados do Formulário Para Alterar a Senha.", "Atenção!", "WARNING");
                return View();
            }
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuario.Any(e => e.IdUsuario == id);
        }
    }
}
