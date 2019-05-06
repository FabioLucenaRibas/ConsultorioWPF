using System.Data.SqlClient;

namespace Biblioteca.Dados
{
    public class Conexao
    {
        #region connection string
        //http://www.connectionstrings.com/
        #region variáveis
        public SqlConnection SqlConn { get; set; }
        #endregion
        //string de conexão obtida para o sql sever
        readonly string connectionStringSqlServer = "Data Source=FABIOLUCENARIBA;Initial Catalog=Consultorio;Trusted_Connection=Yes;";

        //string connectionStringSqlServer = "Data Source=PC\\SQLEXPRESS;Initial Catalog=Consultorio;UId=admin;Password=admin;";

        public void AbrirConexao()
        {
            //iniciando uma conexão com o sql server, utilizando os parâmetros da string de conexão
            SqlConn = new SqlConnection(connectionStringSqlServer);
            //abrindo a conexão com a base de dados
            SqlConn.Open();
        }

        public void FecharConexao()
        {
            //fechando a conexao com a base de dados
            SqlConn.Close();

            //liberando a conexao da memoria
            SqlConn.Dispose();

        }
        #endregion
    }
}
