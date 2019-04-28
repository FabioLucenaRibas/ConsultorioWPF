using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Biblioteca.Negocio
{
    public static class SiteUtil
    {

        public static bool isValidCPF(String CPF)
        {
            int[] mt1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] mt2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string TempCPF;
            string Digito;
            int soma;
            int resto;

            CPF = CPF.Trim();
            CPF = Regex.Replace(CPF, "[^0-9,]", "");

            if (CPF.Length != 11)
                return false;

            TempCPF = CPF.Substring(0, 9);
            soma = 0;
            for (int i = 0; i < 9; i++)
                soma += int.Parse(TempCPF[i].ToString()) * mt1[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            Digito = resto.ToString();
            TempCPF = TempCPF + Digito;
            soma = 0;

            for (int i = 0; i < 10; i++)
                soma += int.Parse(TempCPF[i].ToString()) * mt2[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            Digito = Digito + resto.ToString();

            return CPF.EndsWith(Digito);
        }

        public static String formatarDataHora(DateTime dataHora)
        {
            return String.Format("{0:g}", dataHora);
        }

        public static String formatarData(DateTime dataHora)
        {
            return String.Format("{0:d}", dataHora);
        }

        public static String formatarHora(DateTime dataHora)
        {
            return String.Format("{0:t}", dataHora);
        }

        public static string obterDescricaoEnum(this Enum valorEnum)
        {
            return valorEnum.ObterAtributoDoTipo<DescriptionAttribute>().Description;
        }

        public static string formatarCPF(String cpf)
        {
            return string.Format(cpf, @"000\.000\.000\-00");
        }

        public static string formatarCPF(long cpf)
        {
            return cpf.ToString(@"000\.000\.000\-00");
        }

        public static string formatarCEP(String cep)
        {
            return string.Format(cep, @"00\.000\-000");
        }

        public static string formatarCEP(long cep)
        {
            if (!0L.Equals(cep))
            {
                return cep.ToString(@"00\.000\-000");
            }
            return string.Empty;
        }

        public static string formatarTelefone(String telefone)
        {
            return string.Format(telefone, @"\(00\) 0\.0000\-0000");
        }

        public static string formatarTelefone(long telefone)
        {
            return telefone.ToString(@"\(00\) 0\.0000\-0000");
        }

        public static T ObterAtributoDoTipo<T>(this Enum valorEnum) where T : System.Attribute
        {
            var type = valorEnum.GetType();
            var memInfo = type.GetMember(valorEnum.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            return (attributes.Length > 0) ? (T)attributes[0] : null;
        }

        public static T ConverterParaTipoEnum<T>(int codigo) where T : Enum
        {
            return (T)Enum.ToObject(typeof(T), codigo);
        }

        public static String removerCaracteresEspecial(String value)
        {
            return Regex.Replace(value, "[^0-9,]", "").Trim();
        }
    }
}
