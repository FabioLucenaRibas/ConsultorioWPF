using Biblioteca.ClassesBasicas;
using Biblioteca.Negocio;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Biblioteca.Dados
{
    public class DadosPaciente : Conexao
    {
        #region selecionando os registros da tabela
        public List<Paciente> Consultar(Paciente pFiltro)
        {
            List<Paciente> retorno = new List<Paciente>();
            try
            {
                this.abrirConexao();
                //instrucao a ser executada
                String sqlQuery = "SELECT CPF";
                sqlQuery += " ,NOME";
                sqlQuery += " ,TELEFONE";
                sqlQuery += " ,DATANASCIMENTO";
                sqlQuery += " ,COALESCE (CIDADE, '') AS CIDADE";
                sqlQuery += " ,COALESCE (ESTADO, '') AS ESTADO";
                sqlQuery += " ,COALESCE (COMPLEMENTO, '') AS COMPLEMENTO";
                sqlQuery += " ,COALESCE (LOGRADOURO, '') AS LOGRADOURO";
                sqlQuery += " ,COALESCE (NUMERO, 0) AS NUMERO";
                sqlQuery += " ,COALESCE (BAIRRO, '') AS BAIRRO";
                sqlQuery += " ,COALESCE (CEP, 0) AS CEP";
                sqlQuery += " ,COALESCE (SEXO, '') AS SEXO";
                sqlQuery += " FROM PACIENTE";
                sqlQuery += " WHERE NOME LIKE '%"+ pFiltro .Nome + "%'";
                sqlQuery += " AND CPF LIKE '%" + pFiltro.Cpf + "%'";
                //sqlQuery += "";

                SqlCommand cmd = new SqlCommand(sqlQuery, sqlConn);
                //executando a instrucao e colocando o resultado em um leitor
                SqlDataReader DbReader = cmd.ExecuteReader();
                //lendo o resultado da consulta
                while (DbReader.Read())
                {
                    Paciente paciente = new Paciente
                    {
                        //acessando os valores das colunas do resultado
                        Nome = DbReader.GetString(DbReader.GetOrdinal("NOME")),
                        Cpf = DbReader.GetInt64(DbReader.GetOrdinal("CPF")),
                        Telefone = DbReader.GetInt64(DbReader.GetOrdinal("TELEFONE")),
                        Date = DbReader.GetDateTime(DbReader.GetOrdinal("DATANASCIMENTO")),
                        Cidade = DbReader.GetString(DbReader.GetOrdinal("CIDADE")),
                        Estado = DbReader.GetString(DbReader.GetOrdinal("ESTADO")),
                        Complemento = DbReader.GetString(DbReader.GetOrdinal("COMPLEMENTO")),
                        Logradouro = DbReader.GetString(DbReader.GetOrdinal("LOGRADOURO")),
                        Numero = DbReader.GetInt64(DbReader.GetOrdinal("NUMERO")),
                        Bairro = DbReader.GetString(DbReader.GetOrdinal("BAIRRO")),
                        Cep = DbReader.GetInt64(DbReader.GetOrdinal("CEP"))
                    };
                    int sexo = DbReader.GetInt32(DbReader.GetOrdinal("SEXO"));
                    paciente.Sexo = SiteUtil.ConverterParaTipoEnum<Sexo> (sexo);
                    retorno.Add(paciente);
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
        #endregion

        #region Inclusão de novo paciente
        public void InserirPaciente(Paciente pPaciente)
        {
            ConsultarCPFExistente(pPaciente);

            try
            {
                this.abrirConexao();
                string sqlQuery = "INSERT INTO PACIENTE";
                sqlQuery += " (CPF";
                sqlQuery += " ,NOME";
                sqlQuery += " ,DATANASCIMENTO";
                sqlQuery += " ,ESTADO";
                sqlQuery += " ,COMPLEMENTO";
                sqlQuery += " ,CIDADE";
                sqlQuery += " ,TELEFONE";
                sqlQuery += " ,LOGRADOURO";
                sqlQuery += " ,NUMERO";
                sqlQuery += " ,BAIRRO";
                sqlQuery += " ,CEP";
                sqlQuery += " ,SEXO)";
                sqlQuery += "  VALUES";
                sqlQuery += " (" + pPaciente.Cpf;
                sqlQuery += " ,'" + pPaciente.Nome + "'";
                sqlQuery += " ,'" + SiteUtil.formatarData(pPaciente.Date) + "'";

                if (null != pPaciente.Estado && !string.Empty.Equals(pPaciente.Estado))
                {
                    sqlQuery += " ,'" + pPaciente.Estado + "'";
                }
                else
                {
                    sqlQuery += " ,NULL";
                }

                if (null != pPaciente.Complemento && !string.Empty.Equals(pPaciente.Complemento))
                {
                    sqlQuery += " ,'" + pPaciente.Complemento + "'";
                }
                else
                {
                    sqlQuery += " ,NULL";
                }

                if (null != pPaciente.Cidade && !string.Empty.Equals(pPaciente.Cidade))
                {
                    sqlQuery += " ,'" + pPaciente.Cidade + "'";
                }
                else
                {
                    sqlQuery += " ,NULL";
                }

                sqlQuery += " ," + pPaciente.Telefone;

                if (null != pPaciente.Logradouro && !string.Empty.Equals(pPaciente.Logradouro))
                {
                    sqlQuery += " ,'" + pPaciente.Logradouro + "'";
                }
                else
                {
                    sqlQuery += " ,NULL";
                }

                if (!0.Equals(pPaciente.Numero))
                {
                    sqlQuery += " ," + pPaciente.Numero;
                }
                else
                {
                    sqlQuery += " ,NULL";
                }

                if (null != pPaciente.Bairro && !string.Empty.Equals(pPaciente.Bairro))
                {
                    sqlQuery += " ,'" + pPaciente.Bairro + "'";
                }
                else
                {
                    sqlQuery += " ,NULL";
                }

                if (!0.Equals(pPaciente.Cep))
                {
                    sqlQuery += " ," + pPaciente.Cep;
                }
                else
                {
                    sqlQuery += " ,NULL";
                }
                sqlQuery += " ,'" + (int) pPaciente.Sexo +"')";

                //instrucao a ser executada
                SqlCommand cmd = new SqlCommand(sqlQuery, this.sqlConn);
                //executando a instrucao 
                cmd.ExecuteNonQuery();
                //liberando a memoria 
                cmd.Dispose();
                //fechando a conexao
                this.fecharConexao();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao conectar e inserir\n" + ex.Message);
            }
        }

        #endregion

        #region Consultar CPF cadastrado
        public void ConsultarCPFExistente(Paciente pFiltro)
        {
               this.abrirConexao();
                //instrucao a ser executada
                String sqlQuery = "SELECT *";
                sqlQuery += " FROM PACIENTE";
                sqlQuery += " WHERE CPF = " + pFiltro.Cpf;

                SqlCommand cmd = new SqlCommand(sqlQuery, sqlConn);
                //executando a instrucao e colocando o resultado em um leitor
                SqlDataReader DbReader = cmd.ExecuteReader();
                //lendo o resultado da consulta
                while (DbReader.Read())
                {
                    throw new Exception("Existe um paciente cadastrado com o CPF informado ");
                }
                //fechando o leitor de resultados
                DbReader.Close();
                //liberando a memoria 
                cmd.Dispose();
                //fechando a conexao
                this.fecharConexao();
        }
        #endregion
    }
}
