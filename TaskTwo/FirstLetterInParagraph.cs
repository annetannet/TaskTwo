namespace TaskTwo
{
    internal class FirstLetterInParagraph : ICapitalizationRule
    {
        public bool IsFullfilled(string text, int position)
        {
            return position == 0 || NoLettersBetween(text, position);
        }

        private static bool NoLettersBetween(string text, int pos)
        {
            for (var i = 1; i <= pos; i++)
            {
                var currentChar = text[pos - i];
                if (currentChar == '\n')
                    return true;
                if (char.IsLetter(currentChar))
                    return false;
            }

            return true;
        }
    }
}