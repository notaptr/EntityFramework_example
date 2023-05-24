using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace AptechkaWPF
{
    /// <summary>
    /// Класс содержит функции проверки значений
    /// на принадлежность к определённому шаблону.
    /// </summary>
    public static class Validation
    {
        /// <summary>
        /// Метод проверяет, является ли строка допустимой для
        /// преобразования в тип decimal. Пустая строка недопустима.
        /// </summary>
        /// <param name="param">Проверяемая строка</param>
        /// <returns>Возвращает True в случае допустимости</returns>
        public static bool CheckDecimal(String param)
        {
            bool rez = true;
            decimal out_val;

            if ( string.IsNullOrWhiteSpace(param) ) { return false; }

            rez = decimal.TryParse(param, NumberStyles.Currency, new CultureInfo("en_US"), out out_val);

            return rez;
        }

        /// <summary>
        /// Метод проверяет, является ли строка допустимым
        /// адресом e-mail. Алгоритм очень упрощён, рассчитан
        /// на случайную опечатку. Пустая строка допустима.
        /// </summary>
        /// <param name="param">Проверяемая строка</param>
        /// <returns>Возвращает True в случае допустимости</returns>
        public static bool CheckEmail(String param)
        {
            bool rez = true;

            if (string.IsNullOrWhiteSpace(param)) { return true; }

            rez = Regex.IsMatch(param, "^(.+)(@)(.+)(\\.)(.+)");

            return rez;
        }

        /// <summary>
        /// Метод проверяет, является ли строка допустимым
        /// телефонным номером в местном или международном формате.
        /// Пустая строка допустима.
        /// </summary>
        /// <param name="param">Проверяемая строка</param>
        /// <returns>Возвращает True в случае допустимости</returns>
        public static bool CheckTelephone(String param)
        {
            bool rez = true;

            if (string.IsNullOrWhiteSpace(param)) { return true; }

            string tparam = param.Trim().ToLower();

            tparam = Regex.Replace(tparam, "^\\+|[0-9]+|[\\(\\)]|-", String.Empty);

            rez = tparam == "";

            return rez;
        }

        /// <summary>
        /// Метод проверяет, является ли строка допустимым
        /// номером ИНН.
        /// Пустая строка допустима.
        /// </summary>
        /// <param name="param">Проверяемая строка</param>
        /// <returns>Возвращает True в случае допустимости</returns>
        public static bool CheckINN(String param)
        {
            bool rez = true;

            if (string.IsNullOrWhiteSpace(param)) { return true; }

            string tparam = param.Trim();

            if (tparam.Length != 10) { return false; }

            tparam = Regex.Replace(tparam, "[0-9]+", String.Empty);

            rez = tparam == "";

            return rez;
        }
    }

}