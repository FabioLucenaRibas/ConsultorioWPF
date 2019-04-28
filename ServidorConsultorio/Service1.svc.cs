using System.Collections.Generic;
using Biblioteca.Dados;
using Biblioteca.ClassesBasicas;

namespace ServidorConsultorio
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        public List<Consulta> Consultar(ConsultaFiltro pFiltro)
        {
            return new DadosConsulta().Consultar(pFiltro);
        }

        public List<Paciente> ConsultarPaciente(Paciente pFiltro)
        {
            return new DadosPaciente().Consultar(pFiltro);
        }

        public CEP ConsultarCEP(string CEP)
        {
            return new DadosBuscaCEP().ConsultarCEP(CEP);
        }

        public void InserirPaciente(Paciente pFiltro)
        {
            new DadosPaciente().InserirPaciente(pFiltro);
        }

    }
}
