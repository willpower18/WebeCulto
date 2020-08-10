using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AdminEAD.Coomon
{
    public class Util
    {
        public static string GetHash(string text) // Metodo Para retornar Hash de Senha
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        public static string RenderAlert(string message, string title, string type)
        {
            StringBuilder html = new StringBuilder();
            if (type == "ERROR")
            {
                html.Append("<div class=\"alert alert-danger fade show\" role=\"alert\"><div class=\"alert-icon\"><i class=\"fas fa-window-close\"></i></div><div class=\"alert-text\"><b>" + title + "</b> " + message + "</div><div class=\"alert-close\"><button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-label=\"Close\"><span aria-hidden=\"true\"><i class=\"la la-close\"></i></span></button></div></div>");
            }
            else if (type == "WARNING")
            {
                html.Append("<div class=\"alert alert-warning fade show\" role=\"alert\"><div class=\"alert-icon\"><i class=\"fas fa-exclamation-triangle\"></i></div><div class=\"alert-text\"><b>" + title + "</b> " + message + "</div><div class=\"alert-close\"><button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-label=\"Close\"><span aria-hidden=\"true\"><i class=\"la la-close\"></i></span></button></div></div>");
            }
            else if (type == "SUCCESS")
            {
                html.Append("<div class=\"alert alert-success fade show\" role=\"alert\"><div class=\"alert-icon\"><i class=\"fas fa-check-circle\"></i></div><div class=\"alert-text\"><b>" + title + "</b> " + message + "</div><div class=\"alert-close\"><button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-label=\"Close\"><span aria-hidden=\"true\"><i class=\"la la-close\"></i></span></button></div></div>");
            }

            return html.ToString();
        }

        public static DateTime BrasilDate()
        {
            string system = System.Environment.OSVersion.ToString();
            if (system.Contains("Windows"))
            {
                return TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            }
            else
            {
                return TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo"));
            }
        }
    }
}
