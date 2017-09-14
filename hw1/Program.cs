using System;
using System.Collections.Generic;

namespace hw1
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var trie = new Trie();
            trie.Add("abacaba");
            trie.Add("dabacaba");
            Console.WriteLine($"aba: {trie.HowManyStartsWithPrefix("aba")}");
            Console.WriteLine($"b: {trie.HowManyStartsWithPrefix("d")}");
            Console.WriteLine($"<empty>: {trie.HowManyStartsWithPrefix("")}");
        }
    }
}