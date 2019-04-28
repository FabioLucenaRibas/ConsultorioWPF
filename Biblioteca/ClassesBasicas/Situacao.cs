using System;

namespace Biblioteca.ClassesBasicas
{
    public class Situacao
    {

        private int codigo;
        private String descricao;

        public int Codigo
        {
            get { return codigo; }
            set { codigo = value; }
        }

        public String Descricao
        {
            get { return descricao; }
            set { descricao = value; }
        }

    }
}
