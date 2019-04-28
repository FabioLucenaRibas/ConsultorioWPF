using System;

namespace Biblioteca.ClassesBasicas
{
    public class Consulta
    {
        private int codigo;
        private DateTime dataConsulta;
        private int pagemento;
        private Situacao situacao;
        private Paciente paciente;
        private Tratamento tratamento;


        public int Codigo
        {
            get { return codigo; }
            set { codigo = value; }
        }

        public DateTime DataConsulta
        {
            get { return dataConsulta; }
            set { dataConsulta = value; }
        }

        public int Pagemento
        {
            get { return pagemento; }
            set { pagemento = value; }
        }

        public Situacao Situacao
        {
            get { return situacao; }
            set { situacao = value; }
        }

        public Paciente Paciente
        {
            get { return paciente; }
            set { paciente = value; }
        }

        public Tratamento Tratamento
        {
            get { return tratamento; }
            set { tratamento = value; }
        }

    }
}
