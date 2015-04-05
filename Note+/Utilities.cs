using System;

namespace Note_
{
    public class Utilities
    {
        /// <summary>
        /// Trim data, remove trash, standardized
        /// </summary>
        /// <param name="s">raw string</param>
        /// <returns>perfect string</returns>
        internal static string RemoveRedundancy(string s)
        {
            s = s.Replace(Environment.NewLine, " ");
            s = s.Replace("\t", " ");
            s = s.Replace("’", "'");
            s = s.Trim();
            do
            {
                s = s.Replace("  ", " ");
            }
            while (s.Contains("  "));
            do
            {
                s = s.Replace("..", ".");
            }
            while (s.Contains(".."));
            const string str = "!@#$%^&*()_+{}:\"<>?[]',./\\;-=";
            foreach (var ch in str.ToCharArray())
            {
                s = s.Replace(ch + " ", ch.ToString());
                s = s.Replace(" " + ch, ch.ToString());
            }
            return s;
        }
    }
}