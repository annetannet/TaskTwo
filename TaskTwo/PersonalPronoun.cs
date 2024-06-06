namespace TaskTwo
{
    internal class PersonalPronoun : ICapitalizationRule
    {
        public bool IsFullfilled(string text, int position)
        {
            return position != 0 && position < text.Length - 1 && !char.IsLetter(text[position - 1]) &&
                   !char.IsLetter(text[position + 1]);
        }
    }
}