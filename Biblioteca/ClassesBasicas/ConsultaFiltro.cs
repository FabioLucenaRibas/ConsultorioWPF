using System;

namespace Biblioteca.ClassesBasicas
{
    public class ConsultaFiltro
    {
        private DateTime dataInicio = DateTime.Now;
        private DateTime dataFim = DateTime.Now;
        public String NomePaciente { get; set; }
        public long Cpf { get; set; }

        public DateTime DataInicio
        {
            get { return dataInicio; }
            set { dataInicio = value; }
        }

        public DateTime DataFim
        {
            get { return dataFim; }
            set { dataFim = value; }
        }
    }
}
