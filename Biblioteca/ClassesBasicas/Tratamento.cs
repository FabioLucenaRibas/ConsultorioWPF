using System;

namespace Biblioteca.ClassesBasicas
{
    public class Tratamento
    {

        private int codigo;
        private String nome;
        private String descricao;
        private decimal preco;
        private DateTime duracao;


        public int Codigo
        {
            get { return codigo; }
            set { codigo = value; }
        }

        public String Nome
        {
            get { return nome; }
            set { nome = value; }
        }

        public String Descricao
        {
            get { return descricao; }
            set { descricao = value; }
        }

        public decimal Preco
        {
            get { return preco; }
            set { preco = value; }
        }

        public DateTime Duracao
        {
            get { return duracao; }
            set { duracao = value; }
        }

    }
}
