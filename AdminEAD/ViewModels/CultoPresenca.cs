using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminEAD.Models;

namespace AdminEAD.ViewModels
{
    public class CultoPresenca
    {
        public Culto culto { get; set; }
        public List<Participacao> participacoes { get; set; }
    }
}
