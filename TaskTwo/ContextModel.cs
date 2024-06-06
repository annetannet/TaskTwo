using System;
using System.Collections.Generic;

namespace TaskTwo
{
    [Serializable]
    internal class ContextModel
    {

        public ContextModel(string str)
        {
            Str = str;
            OccurrencesWithCharCount = new Dictionary<char, int>();
        }

        public string Str { get; } // подстрока
        public int AllOccurrencesCount { get; private set; } // кол-во всех вхождений подстроки в текст

        public Dictionary<char, int> OccurrencesWithCharCount; // словарь, где key - символ, встречающийся в тексте с левым контекстом Str,
        // value - кол-во вхождений этого символа в текст с левым контекстом Str
        public void AddOccurence(char nextCharacter)
        {
            AllOccurrencesCount++;
            OccurrencesWithCharCount.TryAdd(nextCharacter, 0);
            OccurrencesWithCharCount[nextCharacter]++;
        }
    }
}