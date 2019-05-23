using Biblioteca.ClassesBasicas;
using System.Linq;
using System.Xml.Linq;

namespace Biblioteca.Dados
{
    public static class DadosBuscaCEP
    {
        public static CEP ConsultarCEP(string pCep)
        {
            CEP modeloRetorno = new CEP();

            string caminhoXML = "http://cep.republicavirtual.com.br/web_cep.php?cep=" + pCep + "&formato=xml";
            XDocument documentoXML = XDocument.Load(caminhoXML);

            modeloRetorno.Logradouro = documentoXML.Descendants().Elements("logradouro").First().Value;
            modeloRetorno.TipoLogradouro = documentoXML.Descendants().Elements("tipo_logradouro").First().Value;
            modeloRetorno.Bairro = documentoXML.Descendants().Elements("bairro").First().Value;
            modeloRetorno.Cidade = documentoXML.Descendants().Elements("cidade").First().Value;
            modeloRetorno.UF = documentoXML.Descendants().Elements("uf").First().Value;
            modeloRetorno.Resultado = documentoXML.Descendants().Elements("resultado").First().Value;
            modeloRetorno.ResultadoMensagem = documentoXML.Descendants().Elements("resultado_txt").First().Value;

            return modeloRetorno;
        }
      }

}

