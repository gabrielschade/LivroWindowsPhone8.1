﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompreAqui.Modelos
{
    public class Loja
    {
        private Loja()
        {}

        private static Loja dados;
        public static Loja Dados
        {
            get
            {
                if (dados == null)
                    dados = new Loja();

                return dados;
            }
            set
            {
                dados = value;
            }
        }

        public List<Produto> Produtos { get; set; }
    }
}
