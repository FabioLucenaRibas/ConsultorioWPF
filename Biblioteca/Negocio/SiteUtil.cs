using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Biblioteca.Negocio
{
    public static class SiteUtil
    {

        public const string CAMPOOBRIGATORIO = "Campos obrigatórios não preenchidos!";

        public static bool IsValidCPF(String CPF)
        {
            int[] mt1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] mt2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string TempCPF;
            string Digito;
            int soma;
            int resto;

            CPF = RemoverCaracteresEspecial(CPF);

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
            TempCPF += Digito;
            soma = 0;

            for (int i = 0; i < 10; i++)
                soma += int.Parse(TempCPF[i].ToString()) * mt2[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            Digito += resto.ToString();

            return CPF.EndsWith(Digito);
        }

        public static String FormatarDataHora(DateTime dataHora)
        {
            return String.Format("{0:g}", dataHora);
        }

        public static String FormatarData(DateTime dataHora)
        {
            return String.Format("{0:d}", dataHora);
        }

        public static String FormatarHora(DateTime dataHora)
        {
            return String.Format("{0:t}", dataHora);
        }

        public static string ObterDescricaoEnum(this Enum valorEnum)
        {
            return valorEnum.ObterAtributoDoTipo<DescriptionAttribute>().Description;
        }

        public static string FormatarCPF(String cpf)
        {
            string value = RemoverCaracteresEspecial(cpf);
            return Convert.ToUInt64(value).ToString(@"000\.000\.000\-00");
        }

        public static string FormatarCPF(long cpf)
        {
            return cpf.ToString(@"000\.000\.000\-00");
        }

        public static string FormatarCEP(String cep)
        {
            string value = RemoverCaracteresEspecial(cep);
            return Convert.ToUInt64(value).ToString(@"00\.000\-000");
        }

        public static string FormatarCEP(long cep)
        {
            if (!0L.Equals(cep))
            {
                return cep.ToString(@"00\.000\-000");
            }
            return string.Empty;
        }

        public static string FormatarTelefone(string telefone)
        {
            string value = RemoverCaracteresEspecial(telefone);
            return Convert.ToUInt64(value).ToString(@"\(00\) 0\.0000\-0000");
        }

        public static string FormatarTelefone(long telefone)
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

        public static String RemoverCaracteresEspecial(String value)
        {
            return Regex.Replace(value, "[^0-9,]", "").Trim();
        }

        public static long ConverterStringParaLong(string value)
        {
            if (string.Empty.Equals(value))
            {
                return 0L;
            }
            else
            {
                return Convert.ToInt64(value);
            }

        }

        public static SolidColorBrush BorderBrushPadrao()
        {
            SolidColorBrush retono = new SolidColorBrush();
            Color cor = new Color
            {
                A = 137,
                ScA = 0.5372549F
            };
            retono.Color = cor;
            return retono;
        }

        public static void ValidacaoTextBox(TextBox campo, string msg)
        {
            ValidacaoTextBox(campo);
            campo.ToolTip = msg;
        }

        public static void ValidacaoTextBox(TextBox campo)
        {
            campo.BorderBrush = Brushes.Red;
        }

        public static void ValidacaoTextBoxReset(TextBox campo)
        {
            campo.BorderBrush = BorderBrushPadrao();
            campo.ToolTip = null;
        }

        public static void FormatarCPFPreviewTextInput(TextBox campo, TextCompositionEventArgs e)
        {

            if (!char.IsDigit(Convert.ToChar(e.Text)) && Convert.ToChar(e.Text) != (char)8)
            {
                e.Handled = true;
            }
            if (char.IsNumber(Convert.ToChar(e.Text)) == true)
            {
                switch (campo.Text.Length)
                {
                    case 0:
                        campo.Text = string.Empty;
                        break;
                    case 3:
                        campo.Text += ".";
                        campo.SelectionStart = 5;
                        break;
                    case 7:
                        campo.Text += ".";
                        campo.SelectionStart = 9;
                        break;
                    case 11:
                        campo.Text += "-";
                        campo.SelectionStart = 13;
                        break;
                }
            }
        }

        public static void FormatarCPFLostFocus(TextBox campo)
        {
            if (!14.Equals(campo.Text.Length) && !11.Equals(campo.Text.Length) && !10.Equals(campo.Text.Length))
            {
                campo.Text = string.Empty;
            }
            else
            {
                campo.Text = FormatarCPF(campo.Text);
            }
        }

        public static void TxtCEPPreviewTextInput(TextBox campo, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(Convert.ToChar(e.Text)) && Convert.ToChar(e.Text) != (char)8)
            {
                e.Handled = true;
            }

            if (char.IsNumber(Convert.ToChar(e.Text)) == true)
            {
                switch (campo.Text.Length)
                {
                    case 0:
                        campo.Text = string.Empty;
                        break;
                    case 2:
                        campo.Text += ".";
                        campo.SelectionStart = 4;
                        break;
                    case 6:
                        campo.Text += "-";
                        campo.SelectionStart = 8;
                        break;
                }
            }
        }

        public static void FormatarCampoTelefonePreviewTextInput(TextBox campo, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(Convert.ToChar(e.Text)) && Convert.ToChar(e.Text) != (char)8)
            {
                e.Handled = true;
            }

            if (char.IsNumber(Convert.ToChar(e.Text)) == true)
            {
                switch (campo.Text.Length)
                {
                    case 0:
                        campo.Text = "";
                        break;
                    case 1:
                        campo.Text = "(" + campo.Text;
                        campo.SelectionStart = 3;
                        break;
                    case 3:
                        campo.Text += ") ";
                        campo.SelectionStart = 7;
                        break;
                    case 6:
                        campo.Text += ".";
                        campo.SelectionStart = 8;
                        break;
                    case 11:
                        campo.Text += "-";
                        campo.SelectionStart = 13;
                        break;
                }
            }
        }

        public static void CampoNumericoPreviewTextInput(TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(Convert.ToChar(e.Text)) && Convert.ToChar(e.Text) != (char)8)
            {
                e.Handled = true;
            }
        }
    }
}
