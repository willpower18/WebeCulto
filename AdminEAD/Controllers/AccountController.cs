using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AdminEAD.Models;
using AdminEAD.Coomon;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace AdminEAD.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly eCultoContext dbContext;

        public AccountController(eCultoContext db)
        {
            this.dbContext = db;
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string usuario, string senha, bool conectado, string returnUrl = null)
        {
            StringBuilder msg = new StringBuilder();
            try
            {
                if (string.IsNullOrEmpty(usuario) && string.IsNullOrEmpty(senha))
                {
                    msg.Append("<div class=\"alert alert-danger alert-dismissible\" role=\"alert\"><div class=\"alert-text\">Informe o Usuário e Senha para autenticação.</div><div class=\"alert-close\"><i class=\"flaticon2-cross kt-icon-sm\" data-dismiss=\"alert\"></i></div></div>");
                    ViewBag.ErrorMessage = msg.ToString();
                    return View();
                }

                string hash = Util.GetHash(senha);

                var user = dbContext.Usuario.Where(x => x.Login == usuario && x.Senha == hash);

                if (user != null && user.Count() > 1)
                {
                    msg.Append("<div class=\"alert alert-danger alert-dismissible\" role=\"alert\"><div class=\"alert-text\">Usuário duplicado, procure o administrador do sistema.</div><div class=\"alert-close\"><i class=\"flaticon2-cross kt-icon-sm\" data-dismiss=\"alert\"></i></div></div>");
                    ViewBag.ErrorMessage = msg.ToString();
                    return View();
                }
                else if (user != null && user.FirstOrDefault() != null)
                {
                    if (user.FirstOrDefault().Ativo == 0)
                    {
                        msg.Append("<div class=\"alert alert-danger alert-dismissible\" role=\"alert\"><div class=\"alert-text\">Usuário bloqueado, procure o administrador do sistema.</div><div class=\"alert-close\"><i class=\"flaticon2-cross kt-icon-sm\" data-dismiss=\"alert\"></i></div></div>");
                        ViewBag.ErrorMessage = msg.ToString();
                        return View();
                    }
                }
                else if (user.FirstOrDefault() == null)
                {
                    msg.Append("<div class=\"alert alert-danger alert-dismissible\" role=\"alert\"><div class=\"alert-text\">Usuário e/ou senha inválidos.</div><div class=\"alert-close\"><i class=\"flaticon2-cross kt-icon-sm\" data-dismiss=\"alert\"></i></div></div>");
                    ViewBag.ErrorMessage = msg.ToString();
                    return View();
                }

                var cadastro = user.FirstOrDefault();
                TipoUsuario tipousuario = new TipoUsuario();
                tipousuario = dbContext.TipoUsuario.Find(cadastro.IdTipoUsuario);

                List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, cadastro.Nome, ClaimValueTypes.String),
                    new Claim(ClaimTypes.Role, tipousuario.Nome, ClaimValueTypes.String),
                    new Claim("Idusuario",cadastro.IdUsuario.ToString(), ClaimValueTypes.String),
                    new Claim("IdIgreja",cadastro.IdIgreja.ToString(), ClaimValueTypes.String),
                    new Claim("IdtipoUsuario", cadastro.IdTipoUsuario.ToString(), ClaimValueTypes.String),
                    new Claim(ClaimTypes.Role, tipousuario.Nome)
                };

                ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(120),
                    IsPersistent = true,
                    RedirectUri = "https://localhost:44318/Account/Logout"
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), authProperties);
                //Redireciona para a tela principal
                return RedirectToLocal(returnUrl);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            //Realiza o LogOut
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Account");
        }

        [Authorize]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
