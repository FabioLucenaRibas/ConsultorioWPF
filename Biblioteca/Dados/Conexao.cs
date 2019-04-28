using System.Data.SqlClient;

namespace Biblioteca.Dados
{
    public class Conexao
    {
        #region connection string
        //http://www.connectionstrings.com/
        #region variáveis
        public SqlConnection sqlConn;
        #endregion
        //string de conexão obtida para o sql sever
        string connectionStringSqlServer = "Data Source=FABIOLUCENARIBA;Initial Catalog=Consultorio;Trusted_Connection=Yes;";
        //string connectionStringSqlServer = "Data Source=PC\\SQLEXPRESS;Initial Catalog=Consultorio;UId=admin;Password=admin;";

        public void abrirConexao()
        {
            //iniciando uma conexão com o sql server, utilizando os parâmetros da string de conexão
            this.sqlConn = new SqlConnection(connectionStringSqlServer);
            //abrindo a conexão com a base de dados
            this.sqlConn.Open();
        }


        public void fecharConexao()
        {
            //fechando a conexao com a base de dados
            sqlConn.Close();
            //liberando a conexao da memoria
            sqlConn.Dispose();
        }

        #endregion
    }
}
