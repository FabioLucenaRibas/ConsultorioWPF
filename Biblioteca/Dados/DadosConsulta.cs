using Biblioteca.ClassesBasicas;
using Biblioteca.Negocio;
using System;
using System.Collections.Generic;
using System.Data;
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
                this.AbrirConexao();

                //instrucao a ser executada
                String sqlQuery = "SELECT P.NOME AS NOME" +
                                ", P.CPF AS CPF" +
                                ", P.TELEFONE AS TELEFONE" +
                                ", P.DATANASCIMENTO AS DATANASCIMENTO" +
                                ", A.DATAHORA AS DATACONSULTA" +
                                ", T.NOME AS NOMECTRATAMENTO" +
                                ", S.DESCRICAO AS DESCSITUACAO" +
                                " FROM ATENDIMENTO AS A" +
                                " LEFT JOIN  PACIENTE AS P ON A.FK_PACIENTE_CPF = P.CPF" +
                                " LEFT JOIN  SITUACAO AS S ON A.FK_SITUACAO_CODIGO = S.CODIGO" +
                                " LEFT JOIN  TRATAMENTO AS T ON A.FK_TRATAMENTO_CODIGO = T.CODIGO" +
                                " WHERE (A.DATAHORA >= @DATAINICIO" +
                                "   AND  A.DATAHORA <= @DATAFIM)" +
                                "   AND  P.NOME LIKE @NOMEPACIENTE" +
                                "   AND  P.CPF LIKE @CPF" +
                                " ORDER  BY A.DATAHORA";
                SqlCommand cmd = new SqlCommand(sqlQuery, SqlConn);

                cmd.Parameters.Add("@DATAINICIO", SqlDbType.DateTime).Value = SiteUtil.FormatarData(pFiltro.DataInicio);
                cmd.Parameters.Add("@DATAFIM", SqlDbType.DateTime).Value = SiteUtil.FormatarData(pFiltro.DataFim) + " 23:59:59";
                cmd.Parameters.Add("@NOMEPACIENTE", SqlDbType.VarChar).Value = "%" + pFiltro.NomePaciente + "%";

                if (0L.Equals(pFiltro.Cpf))
                {
                    cmd.Parameters.Add("@CPF", SqlDbType.VarChar).Value = "%%";
                }
                else
                {
                    cmd.Parameters.Add("@CPF", SqlDbType.VarChar).Value = "%" + pFiltro.Cpf + "%";
                }

                //executando a instrucao e colocando o resultado em um leitor
                 SqlDataReader DbReader = cmd.ExecuteReader();
                //lendo o resultado da consulta
                while (DbReader.Read())
                {
                    Consulta consulta = new Consulta();
                    Paciente paciente = new Paciente
                    {
                        //acessando os valores das colunas do resultado
                        Nome = DbReader.GetString(DbReader.GetOrdinal("NOME")),
                        Cpf = DbReader.GetInt64(DbReader.GetOrdinal("CPF")),
                        Telefone = DbReader.GetInt64(DbReader.GetOrdinal("TELEFONE")),
                        Date = DbReader.GetDateTime(DbReader.GetOrdinal("DATANASCIMENTO"))
                    };
                    consulta.Paciente = paciente;
                    consulta.DataConsulta = DbReader.GetDateTime(DbReader.GetOrdinal("DATACONSULTA"));
                    Tratamento tratamento = new Tratamento
                    {
                        Nome = DbReader.GetString(DbReader.GetOrdinal("NOMECTRATAMENTO"))
                    };
                    consulta.Tratamento = tratamento;
                    Situacao situacao = new Situacao
                    {
                        Descricao = DbReader.GetString(DbReader.GetOrdinal("DESCSITUACAO"))
                    };
                    consulta.Situacao = situacao;
                    retorno.Add(consulta);
                }
                //fechando o leitor de resultados
                DbReader.Close();
                //liberando a memoria 
                cmd.Dispose();
                //fechando a conexao
                this.FecharConexao();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao conectar e selecionar " + ex.Message);
            }
            return retorno;
        }
        #endregion
    }
}
