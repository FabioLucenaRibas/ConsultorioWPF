using Biblioteca.Negocio;
using System;

namespace Biblioteca.ClassesBasicas
{
    public class Paciente
    {

        private long cpf;
        private String nome;
        private DateTime date;
        private String estado;
        private String complemento;
        private String cidade;
        private long telefone;
        private String logradouro;
        private long numero;
        private String bairro;
        private long cep;
        private Sexo sexo;

        public long Cpf
        {
            get { return cpf; }
            set { cpf = value; }
        }

        public String Nome
        {
            get { return nome; }
            set { nome = value; }
        }

        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        public String Estado
        {
            get { return estado; }
            set { estado = value; }
        }

        public String Complemento
        {
            get { return complemento; }
            set { complemento = value; }
        }

        public String Cidade
        {
            get { return cidade; }
            set { cidade = value; }
        }

        public long Telefone
        {
            get { return telefone; }
            set { telefone = value; }
        }

        public String Logradouro
        {
            get { return logradouro; }
            set { logradouro = value; }
        }

        public long Numero
        {
            get { return numero; }
            set { numero = value; }
        }

        public String Bairro
        {
            get { return bairro; }
            set { bairro = value; }
        }

        public long Cep
        {
            get { return cep; }
            set { cep = value; }
        }

        public Sexo Sexo
        {
            get { return sexo; }
            set { sexo = value; }
        }
    }
}
