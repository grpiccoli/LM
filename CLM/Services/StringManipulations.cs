﻿using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace CLM.Models
{
    public static class StringManipulations
    {
        public static string HtmlToPlainText(string html)
        {
            const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";//matches one or more (white space or line breaks) between '>' and '<'
            const string stripFormatting = @"<[^>]*(>|$)";//match any character between '<' and '>', even when end tag is missing
            const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";//matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />
            var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
            var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
            var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);

            var text = html;
            //Decode html specific characters
            text = System.Net.WebUtility.HtmlDecode(text);
            //Remove tag whitespace/line breaks
            text = tagWhiteSpaceRegex.Replace(text, "><");
            //Replace <br /> with line breaks
            text = lineBreakRegex.Replace(text, Environment.NewLine);
            //Strip formatting
            text = stripFormattingRegex.Replace(text, string.Empty);

            text = Regex.Replace(text, @"<[^>]+>|&nbsp;", "").Trim();

            text = Regex.Replace(text, @"\s{2,}", " ");

            text = Regex.Replace(text, @">", "");

            return text;
        }

        public static string GetDigit(int ID)
        {
            int Digito;
            int Contador;
            int Multiplo;
            int Acumulador;
            string RutDigito;

            Contador = 2;
            Acumulador = 0;

            while (ID != 0)
            {
                Multiplo = (ID % 10) * Contador;
                Acumulador = Acumulador + Multiplo;
                ID = ID / 10;
                Contador = Contador + 1;
                if (Contador == 8)
                {
                    Contador = 2;
                }
            }
            Digito = 11 - (Acumulador % 11);
            RutDigito = Digito.ToString().Trim();
            if (Digito == 10)
            {
                RutDigito = "K";
            }
            if (Digito == 11)
            {
                RutDigito = "0";
            }
            return (RutDigito);
        }

        public static List<int> AllIndexesOf(this string str, string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("El texto a buscar no puede estar vacío", "value");
            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }
        public static List<string> romanNumerals = new List<string>() { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };
        public static List<int> numerals = new List<int>() { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };

        public static string ToRomanNumeral(int number)
        {
            var romanNumeral = string.Empty;
            while (number > 0)
            {
                // find biggest numeral that is less than equal to number
                var index = numerals.FindIndex(x => x <= number);
                // subtract it's value from your number
                number -= numerals[index];
                // tack it onto the end of your roman numeral
                romanNumeral += romanNumerals[index];
            }
            return romanNumeral;
        }

        public static string TranslateText(
            string targetLanguage,
            string input,
            string languagePair)
        {
            //return Task.Run(() =>  
            //{
            //    string url = String.Format("http://www.google.com/translate_t?hl={0}&ie=UTF8&text={1}&langpair={2}", targetLanguage, input, languagePair);
            //    HttpClient hc = new HttpClient();
            //    HttpResponseMessage result = hc.GetAsync(url).Result;
            //    HtmlDocument doc = new HtmlDocument() { OptionReadEncoding = true };
            //    doc.Load(result.Content.ReadAsStreamAsync().Result);
            //    string resultado = "bla";
            //    try
            //    {
            //        resultado = HtmlToPlainText(doc.DocumentNode.SelectSingleNode("//span[@id='result_box']/span").InnerHtml);
            //    }
            //    catch { }
            //    return resultado;
            //}).Result;

            string url = String.Format("http://www.google.com/translate_t?hl={0}&ie=UTF8&text={1}&langpair={2}", targetLanguage, input, languagePair);
            HttpClient hc = new HttpClient();
            HttpResponseMessage result = hc.GetAsync(url).Result;
            HtmlDocument doc = new HtmlDocument() { OptionReadEncoding = true };
            doc.Load(result.Content.ReadAsStreamAsync().Result);
            string resultado = "bla";
            try
            {
                resultado = HtmlToPlainText(doc.DocumentNode.SelectSingleNode("//span[@id='result_box']/span").InnerHtml);
            }
            catch { }
            return resultado;
        }
    }
}
