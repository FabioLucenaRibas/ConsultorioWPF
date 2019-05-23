using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using Biblioteca.ClassesBasicas;

namespace ServidorConsultorio
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {

        [OperationContract]
        List<Consulta> Consultar(ConsultaFiltro pFiltro);

        [OperationContract]
        List<Paciente> ConsultarPaciente(Paciente pFiltro);

        [OperationContract]
        CEP ConsultarCEP(string CEP);

        [OperationContract]
        void InserirPaciente(Paciente pFiltro);

        [OperationContract]
        void AtualizarPaciente(Paciente pFiltro);

        [OperationContract]
        List<HistoricoPaciente> ConsultarHistorico(Paciente pFiltro);

        [OperationContract]
        void AtualizarHistorico(HistoricoPaciente pHistorico);

        // TODO: Add your service operations here
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
