﻿using System.Text;

namespace PolyChessTGBot
{
    public static partial class Utils
    {
        public static string RemoveBadSymbols(this string message) =>
            message.Replace(".", @"\.")
                   .Replace("!", @"\!")
                   .Replace("-", @"\-");

        public static string Beautify(this string str) =>
            str[0].ToString().ToUpper() + str[1..];

        public static List<string> ParseParameters(string message)
        {
            List<string> result = [];
            StringBuilder sb = new();
            bool instr = false;
            for (int i = 0; i < message.Length; i++)
            {
                char c = message[i];

                if (c == '\\' && ++i < message.Length)
                {
                    if (message[i] != '"' && message[i] != ' ' && message[i] != '\\')
                        sb.Append('\\');
                    sb.Append(message[i]);
                }
                else if (c == '"')
                {
                    instr = !instr;
                    if (!instr)
                    {
                        result.Add(sb.ToString());
                        sb.Clear();
                    }
                    else if (sb.Length > 0)
                    {
                        result.Add(sb.ToString());
                        sb.Clear();
                    }
                }
                else if (c == ' ' && !instr)
                {
                    if (sb.Length > 0)
                    {
                        result.Add(sb.ToString());
                        sb.Clear();
                    }
                }
                else
                    sb.Append(c);
            }
            if (sb.Length > 0)
                result.Add(sb.ToString());

            return result;
        }

        public static string CreateSimpleBar(double now, double max, char empty = '□', char solid = '■', int bars = 10)
        {
            string result = "";
            double solidBarsCount = now / max * bars;
            for (int i = 0; i < bars; i++)
                if (i <= solidBarsCount - 1)
                    result += solid;
                else
                    result += empty;
            return result;
        }
    }
}