using Biblioteca.ClassesBasicas;
using Biblioteca.Negocio;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Biblioteca.Dados
{
    public class DadosConsulta : Conexao
    {
        #region selecionando os registros da tabela
        public List<Consulta> Consultar(ConsultaFiltro pFiltro)
        {
            List<Consulta> retorno = new List<Consulta>();
            try
            {
                this.abrirConexao();
                //instrucao a ser executada
                String sqlQuery = "SELECT P.NOME AS NOME, P.CPF AS CPF, P.TELEFONE AS TELEFONE, P.DATANASCIMENTO AS DATANASCIMENTO, A.DATAHORA AS DATACONSULTA, T.NOME AS NOMECTRATAMENTO, S.DESCRICAO AS DESCSITUACAO FROM ATENDIMENTO AS A";
                sqlQuery += " LEFT JOIN  PACIENTE AS P ON A.FK_PACIENTE_CPF = P.CPF";
                sqlQuery += " LEFT JOIN  SITUACAO AS S ON A.FK_SITUACAO_CODIGO = S.CODIGO";
                sqlQuery += " LEFT JOIN  TRATAMENTO AS T ON A.FK_TRATAMENTO_CODIGO = T.CODIGO";
                sqlQuery += " WHERE A.DATAHORA >= '" + SiteUtil.formatarData(pFiltro.DataInicio)+"'";
                sqlQuery += "   AND A.DATAHORA <= '" + SiteUtil.formatarData(pFiltro.DataFim) + " 23:59:59'";
                sqlQuery += "   AND P.NOME LIKE '%" + pFiltro.NomePaciente+ "%'";
                sqlQuery += "   AND P.CPF LIKE '%" + pFiltro.Cpf + "%'";
                sqlQuery += " ORDER BY A.DATAHORA";

                //sqlQuery += "";

                SqlCommand cmd = new SqlCommand(sqlQuery, sqlConn);
                //executando a instrucao e colocando o resultado em um leitor
                SqlDataReader DbReader = cmd.ExecuteReader();
                //lendo o resultado da consulta
                while (DbReader.Read())
                {
                    Consulta consulta = new Consulta();
                    Paciente paciente = new Paciente();
                    //acessando os valores das colunas do resultado
                    paciente.Nome = DbReader.GetString(DbReader.GetOrdinal("NOME"));
                    paciente.Cpf = DbReader.GetInt64(DbReader.GetOrdinal("CPF"));
                    paciente.Telefone = DbReader.GetInt64(DbReader.GetOrdinal("TELEFONE"));
                    paciente.Date = DbReader.GetDateTime(DbReader.GetOrdinal("DATANASCIMENTO"));
                    consulta.Paciente = paciente;
                    consulta.DataConsulta = DbReader.GetDateTime(DbReader.GetOrdinal("DATACONSULTA"));
                    Tratamento tratamento = new Tratamento();
                    tratamento.Nome = DbReader.GetString(DbReader.GetOrdinal("NOMECTRATAMENTO"));
                    consulta.Tratamento = tratamento;
                    Situacao situacao = new Situacao();
                    situacao.Descricao = DbReader.GetString(DbReader.GetOrdinal("DESCSITUACAO"));
                    consulta.Situacao = situacao;
                    retorno.Add(consulta);
                }
                //fechando o leitor de resultados
                DbReader.Close();
                //liberando a memoria 
                cmd.Dispose();
                //fechando a conexao
                this.fecharConexao();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao conectar e selecionar " + ex.Message);
            }
            return retorno;
        }

        public List<Consulta> Consultar(ConsultaFiltro consultaFiltro, object pFiltro)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
