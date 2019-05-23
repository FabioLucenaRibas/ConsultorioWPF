using Biblioteca.Negocio;
using System;
using System.Collections.Generic;

namespace Biblioteca.ClassesBasicas
{
    public class Paciente
    {

        public long Cpf { get; set; }
        public String Nome { get; set; }
        public DateTime Date { get; set; }
        public String Estado { get; set; }
        public String Complemento { get; set; }
        public String Cidade { get; set; }
        public long Telefone { get; set; }
        public String Logradouro { get; set; }
        public long Numero { get; set; }
        public String Bairro { get; set; }
        public long Cep { get; set; }
        public Sexo Sexo { get; set; }

    }
}
