﻿using System;
using System.Collections.Generic;

namespace AdminEAD.Models
{
    public partial class Participacao
    {
        public int IdParticipacao { get; set; }
        public int IdCulto { get; set; }
        public string Nome { get; set; }
        public string Telefone { get; set; }
        public string ChaveApp { get; set; }
        public int QtdCriancas { get; set; }
        public int QtdAdultos { get; set; }

        public virtual Culto IdCultoNavigation { get; set; }
    }
}
