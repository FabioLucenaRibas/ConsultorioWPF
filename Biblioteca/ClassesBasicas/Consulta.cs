using System;

namespace Biblioteca.ClassesBasicas
{
    public class Consulta
    {
        public int Codigo { get; set; }
        public DateTime DataConsulta { get; set; }
        public int Pagemento { get; set; }
        public Situacao Situacao { get; set; }
        public Paciente Paciente { get; set; }
        public Tratamento Tratamento { get; set; }

    }
}
