using System;

namespace Biblioteca.ClassesBasicas
{
    public class Usuario
    {
        private int codigo;
        private String email;
        private String senha;

        public int Codigo
        {
            get { return codigo; }
            set { codigo = value; }
        }

        public String Email
        {
            get { return email; }
            set { email = value; }
        }

        public String Senha
        {
            get { return senha; }
            set { senha = value; }
        }

    }
}
