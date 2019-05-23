using Biblioteca.ClassesBasicas;
using Biblioteca.Negocio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;

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
                AbrirConexao();
                //instrucao a ser executada
                String sqlQuery = "SELECT CPF" +
                                  " ,NOME" +
                                  " ,TELEFONE" +
                                  " ,DATANASCIMENTO" +
                                  " ,COALESCE (CIDADE, '') AS CIDADE" +
                                  " ,COALESCE (ESTADO, '') AS ESTADO" +
                                  " ,COALESCE (COMPLEMENTO, '') AS COMPLEMENTO" +
                                  " ,COALESCE (LOGRADOURO, '') AS LOGRADOURO" +
                                  " ,COALESCE (NUMERO, 0) AS NUMERO" +
                                  " ,COALESCE (BAIRRO, '') AS BAIRRO" +
                                  " ,COALESCE (CEP, 0) AS CEP" +
                                  " ,COALESCE (SEXO, '') AS SEXO" +
                                  " FROM PACIENTE" +
                                  " WHERE NOME LIKE @NOME" +
                                  " AND CPF LIKE @CPF";

                SqlCommand cmd = new SqlCommand(sqlQuery, SqlConn);

                cmd.Parameters.Add("@NOME", SqlDbType.VarChar).Value = "%" + pFiltro.Nome + "%";

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
                FecharConexao();
            }
            catch (InvalidCastException Ex)
            {
                throw new Exception("Erro ao consultar dados do paciente\n" + Ex.Message);
            }
            catch (SqlException Ex)
            {
                throw new Exception("Erro ao consultar dados do paciente\n" + Ex.Message);
            }
            catch (IOException Ex)
            {
                throw new Exception("Erro ao consultar dados do paciente\n" + Ex.Message);
            }
            catch (InvalidOperationException Ex)
            {
                throw new Exception("Erro ao consultar dados do paciente\n" + Ex.Message);
            }
            finally
            {
                FecharConexao();
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
                AbrirConexao();
                string sqlQuery = "INSERT INTO PACIENTE" +
                                  " (CPF" +
                                  " ,NOME" +
                                  " ,DATANASCIMENTO" +
                                  " ,ESTADO" +
                                  " ,COMPLEMENTO" +
                                  " ,CIDADE" +
                                  " ,TELEFONE" +
                                  " ,LOGRADOURO" +
                                  " ,NUMERO" +
                                  " ,BAIRRO" +
                                  " ,CEP" +
                                  " ,SEXO)" +
                                  "  VALUES" +
                                  " ( @CPF" +
                                  " , @NOME" +
                                  " , @DATANASCIMENTO" +
                                  " , @ESTADO" +
                                  " , @COMPLEMENTO" +
                                  " , @CIDADE" +
                                  " , @TELEFONE" +
                                  " , @LOGRADOURO" +
                                  " , @NUMERO" +
                                  " , @BAIRRO" +
                                  " , @CEP" +
                                  " , @SEXO)";

                //instrucao a ser executada
                SqlCommand cmd = new SqlCommand(sqlQuery, SqlConn);

                //Parametros
                CarregarParametrosComuns(cmd, pPaciente);
                cmd.Parameters.Add("@NOME", SqlDbType.VarChar).Value = pPaciente.Nome;
                cmd.Parameters.Add("@DATANASCIMENTO", SqlDbType.Date).Value = SiteUtil.FormatarData(pPaciente.Date);
                cmd.Parameters.Add("@SEXO", SqlDbType.VarChar).Value = (int)pPaciente.Sexo;

                //executando a instrucao 
                cmd.ExecuteNonQuery();
                //liberando a memoria 
                cmd.Dispose();
                //fechando a conexao
                FecharConexao();
            }
            catch (InvalidCastException Ex)
            {
                throw new Exception("Erro ao cadastra paciente\n" + Ex.Message);
            }
            catch (SqlException Ex)
            {
                throw new Exception("Erro ao cadastra paciente\n" + Ex.Message);
            }
            catch (IOException Ex)
            {
                throw new Exception("Erro ao cadastra paciente\n" + Ex.Message);
            }
            catch (InvalidOperationException Ex)
            {
                throw new Exception("Erro ao cadastra paciente\n" + Ex.Message);
            }
            finally
            {
                FecharConexao();
            }
        }

        #endregion

        #region Consultar CPF cadastrado
        public bool ConsultarCPFExistente(Paciente pFiltro)
        {
            try { 
                bool Retorno = false;
                AbrirConexao();
                //instrucao a ser executada
                String sqlQuery = "SELECT *" +
                                  " FROM PACIENTE" +
                                  " WHERE CPF = @CPF";

                SqlCommand cmd = new SqlCommand(sqlQuery, SqlConn);

                cmd.Parameters.Add("@CPF", SqlDbType.BigInt).Value = pFiltro.Cpf;

                //executando a instrucao e colocando o resultado em um leitor
                SqlDataReader DbReader = cmd.ExecuteReader();
                //lendo o resultado da consulta
                if (DbReader.Read())
                {
                    Retorno = true;
                    throw new Exception("Já existe um paciente cadastrado com o CPF informado ");
                }
                //fechando o leitor de resultados
                DbReader.Close();
                //liberando a memoria 
                cmd.Dispose();
                //fechando a conexao
                FecharConexao();
                return Retorno;
            }
            catch (InvalidCastException Ex)
            {
                throw new Exception("Erro ao verificar CPF\n" + Ex.Message);
            }
            catch (SqlException Ex)
            {
                throw new Exception("Erro ao verificar CPF\n" + Ex.Message);
            }
            catch (IOException Ex)
            {
                throw new Exception("Erro ao verificar CPF\n" + Ex.Message);
            }
            catch (InvalidOperationException Ex)
            {
                throw new Exception("Erro ao verificar CPF\n" + Ex.Message);
            }
            finally
            {
                FecharConexao();
            }
        }
        #endregion

        #region Atualizar dadoss de novo paciente
        public void AtualizarPaciente(Paciente pPaciente)
        {
            try
            {
                AbrirConexao();
                string sqlQuery = "UPDATE PACIENTE" +
                                  " SET TELEFONE = @TELEFONE" +
                                  ",ESTADO       = @ESTADO" +
                                  ",COMPLEMENTO  = @COMPLEMENTO" +
                                  ",CIDADE       = @CIDADE" +
                                  ",LOGRADOURO   = @LOGRADOURO" +
                                  ",NUMERO       = @NUMERO" +
                                  ",BAIRRO       = @BAIRRO" +
                                  ",CEP          = @CEP" +
                                  " WHERE CPF    = @CPF";

                //instrucao a ser executada
                SqlCommand cmd = new SqlCommand(sqlQuery, SqlConn);
                CarregarParametrosComuns(cmd, pPaciente);

                //executando a instrucao 
                cmd.ExecuteNonQuery();
                //liberando a memoria 
                cmd.Dispose();
                //fechando a conexao
                FecharConexao();
            }
            catch (InvalidCastException Ex)
            {
                throw new Exception("Erro ao atualizar os dados do paciente\n" + Ex.Message);
            }
            catch (SqlException Ex)
            {
                throw new Exception("Erro ao atualizar os dados do paciente\n" + Ex.Message);
            }
            catch (IOException Ex)
            {
                throw new Exception("Erro ao atualizar os dados do paciente\n" + Ex.Message);
            }
            catch (InvalidOperationException Ex)
            {
                throw new Exception("Erro ao atualizar os dados do paciente\n" + Ex.Message);
            }
            finally
            {
                FecharConexao();
            }
        }

        #endregion

        #region Consultar Historico do paciente
        public List<HistoricoPaciente> ConsultarHistorico(Paciente pFiltro)
        {
            try
            {
                List<HistoricoPaciente> retorno = new List<HistoricoPaciente>();
                AbrirConexao();
                //instrucao a ser executada
                String sqlQuery = " SELECT B.CODIGO AS CODIGO" +
                                  " ,B.DATAHORA" +
                                  " ,COALESCE (B.DESCRICAO, '')  AS DESCRICAOHISTORICO" +
                                  " FROM PACIENTE_HISTORICO AS A" +
                                  " INNER JOIN HISTORICO AS B" +
                                  " ON B.CODIGO = A.CODIGO_HISTORICO" +
                                  " WHERE A.CPF_PACIENTE = @CPF";

                SqlCommand cmd = new SqlCommand(sqlQuery, SqlConn);
                cmd.Parameters.Add("@CPF", SqlDbType.BigInt).Value = pFiltro.Cpf;

                //executando a instrucao e colocando o resultado em um leitor
                SqlDataReader DbReader = cmd.ExecuteReader();
                //lendo o resultado da consulta
                while (DbReader.Read())
                {
                    HistoricoPaciente historico = new HistoricoPaciente
                    {
                        //acessando os valores das colunas do resultado
                        Codigo = DbReader.GetInt32(DbReader.GetOrdinal("CODIGO")),
                        DataConsulta = DbReader.GetDateTime(DbReader.GetOrdinal("DATAHORA")),
                        DescricaoHistorico = DbReader.GetString(DbReader.GetOrdinal("DESCRICAOHISTORICO"))
                    };
                    retorno.Add(historico);
                }
                //fechando o leitor de resultados
                DbReader.Close();
                //liberando a memoria 
                cmd.Dispose();
                //fechando a conexao
                FecharConexao();
                return retorno;

            }
            catch (InvalidCastException Ex)
            {
                throw new Exception("Erro ao atualizar o historico\n" + Ex.Message);
            }
            catch (SqlException Ex)
            {
                throw new Exception("Erro ao atualizar o historico\n" + Ex.Message);
            }
            catch (IOException Ex)
            {
                throw new Exception("Erro ao atualizar o historico\n" + Ex.Message);
            }
            catch (InvalidOperationException Ex)
            {
                throw new Exception("Erro ao atualizar o historico\n" + Ex.Message);
            }
            finally
            {
                FecharConexao();
            }
        }
        #endregion

        #region Atualizar Historico do paciente
        public void AtualizarHistorico(HistoricoPaciente pHistorico)
        {
            try
            {
                AbrirConexao();
                string sqlQuery = "UPDATE HISTORICO" +
                                  " SET DESCRICAO = @DESCRICAO" +
                                  " WHERE CODIGO  = @CODIGO";

                //instrucao a ser executada
                SqlCommand cmd = new SqlCommand(sqlQuery, SqlConn);
                cmd.Parameters.Add("@CODIGO", SqlDbType.Int).Value = pHistorico.Codigo;
                cmd.Parameters.Add("@DESCRICAO", SqlDbType.VarChar).Value = pHistorico.DescricaoHistorico;

                //executando a instrucao 
                cmd.ExecuteNonQuery();
                //liberando a memoria 
                cmd.Dispose();
                //fechando a conexao
                FecharConexao();
            }
            catch (InvalidCastException Ex)
            {
                throw new Exception("Erro ao atualizar o historico\n" + Ex.Message);
            }
            catch (SqlException Ex)
            {
                throw new Exception("Erro ao atualizar o historico\n" + Ex.Message);
            }
            catch (IOException Ex)
            {
                throw new Exception("Erro ao atualizar o historico\n" + Ex.Message);
            }
            catch (InvalidOperationException Ex)
            {
                throw new Exception("Erro ao atualizar o historico\n" + Ex.Message);
            }
            finally
            {
                FecharConexao();
            }
        }

        #endregion

        private static void CarregarParametrosComuns(SqlCommand cmd, Paciente pPaciente)
        {
            cmd.Parameters.Add("@CPF", SqlDbType.BigInt).Value = pPaciente.Cpf;
            cmd.Parameters.Add("@TELEFONE", SqlDbType.BigInt).Value = pPaciente.Telefone;

            if (null != pPaciente.Estado && !string.Empty.Equals(pPaciente.Estado, StringComparison.Ordinal))
            {
                cmd.Parameters.Add("@ESTADO", SqlDbType.VarChar).Value = pPaciente.Estado;
            }
            else
            {
                cmd.Parameters.Add("@ESTADO", SqlDbType.VarChar).Value = DBNull.Value;
            }

            if (null != pPaciente.Complemento && !string.Empty.Equals(pPaciente.Complemento, StringComparison.Ordinal))
            {
                cmd.Parameters.Add("@COMPLEMENTO", SqlDbType.VarChar).Value = pPaciente.Complemento;
            }
            else
            {
                cmd.Parameters.Add("@COMPLEMENTO", SqlDbType.VarChar).Value = DBNull.Value;
            }

            if (null != pPaciente.Cidade && !string.Empty.Equals(pPaciente.Cidade, StringComparison.Ordinal))
            {
                cmd.Parameters.Add("@CIDADE", SqlDbType.VarChar).Value = pPaciente.Cidade;
            }
            else
            {
                cmd.Parameters.Add("@CIDADE", SqlDbType.VarChar).Value = DBNull.Value;
            }


            if (null != pPaciente.Logradouro && !string.Empty.Equals(pPaciente.Logradouro, StringComparison.Ordinal))
            {
                cmd.Parameters.Add("@LOGRADOURO", SqlDbType.VarChar).Value = pPaciente.Logradouro;
            }
            else
            {
                cmd.Parameters.Add("@LOGRADOURO", SqlDbType.VarChar).Value = DBNull.Value;
            }

            if (!0.Equals(pPaciente.Numero))
            {
                cmd.Parameters.Add("@NUMERO", SqlDbType.BigInt).Value = pPaciente.Numero;
            }
            else
            {
                cmd.Parameters.Add("@NUMERO", SqlDbType.BigInt).Value = DBNull.Value;
            }

            if (null != pPaciente.Bairro && !string.Empty.Equals(pPaciente.Bairro, StringComparison.Ordinal))
            {
                cmd.Parameters.Add("@BAIRRO", SqlDbType.VarChar).Value = pPaciente.Bairro;
            }
            else
            {
                cmd.Parameters.Add("@BAIRRO", SqlDbType.VarChar).Value = DBNull.Value;
            }

            if (!0.Equals(pPaciente.Cep))
            {
                cmd.Parameters.Add("@CEP", SqlDbType.BigInt).Value = pPaciente.Cep;
            }
            else
            {
                cmd.Parameters.Add("@CEP", SqlDbType.BigInt).Value = DBNull.Value;
            }

        }
    }
}
