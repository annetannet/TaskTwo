﻿namespace TaskTwo
{
    internal class LetterAfterPoint : ICapitalizationRule
    {
        public bool IsFullfilled(string text, int position)
        {
            return position >= 2 && char.IsWhiteSpace(text[position - 1]) && text[position - 2] is '.';
        }
    }
}