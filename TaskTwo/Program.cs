using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TaskTwo
{
    public static class Program
    {
        private const byte ByteSize = 8;

        private static readonly IReadOnlyCollection<ICapitalizationRule> Rules = new ICapitalizationRule[]
        {
            new FirstLetterInParagraph(),
            new PersonalPronoun(),
            new LetterAfterPoint(),
            new LetterAfterExclamationMark(),
            new LetterAfterQuestionMark()
        };

        public static async Task Main()
        {
            using var streamReader = new StreamReader(@"text.txt");
            var text = await streamReader.ReadToEndAsync();

            var positions = new byte[(int)Math.Ceiling(text.Length / (double)ByteSize)];
            for (var i = 0; i < text.Length; i++)
            {
                var index = i / ByteSize;
                if (char.IsUpper(text[i]) && AllRulesBroken(text, i))
                    positions[index] += (byte)(1 << (ByteSize - 1 - i % ByteSize));
            }

            var entropy = GetEntropy(positions);
            Console.WriteLine($"Entropy: {entropy}.");

            new ArithmeticEncoder(12).Encode(positions);

            var contextModels = GetContextModels(text);
            Console.WriteLine($"ContextModels size: {GetSize(contextModels.Values)} bytes.");
        }

        private static double GetEntropy(byte[] bytes)
        {
            var bytesCount = new int[256];
            foreach (var b in bytes)
                bytesCount[b]++;

            return bytesCount
                .Where(i => i != 0)
                .Select(i => i / (double)bytes.Length)
                .Select(probability => probability * -Math.Log2(probability))
                .Sum();
        }

        private static long GetSize(IEnumerable<ContextModel> models)
        {
            var size = 0L;
            foreach (var model in models)
            {
                size += model.Str.Length * 2; // char - 2 bytes
                size += 4; // int - 4 bytes
                size += model.OccurrencesWithCharCount.Count *
                        (2 + 4); // количество пар (char, int) умноженное на размер пары в байтах 
            }

            return size;
        }


        private static Dictionary<string, ContextModel> GetContextModels(string text)
        {
            var contextModels = new Dictionary<string, ContextModel>();

            void UpdateContextModel(string substring, char nextCharacter)
            {
                if (!contextModels.TryGetValue(substring, out var model))
                {
                    model = new ContextModel(substring);
                    contextModels[substring] = model;
                }

                model.AddOccurence(nextCharacter);
            }

            for (var i = 0; i < text.Length; i++)
            {
                var substring = string.Empty;
                UpdateContextModel(substring, char.ToLower(text[i]));

                for (var j = i; j < Math.Min(i + 3, text.Length); j++)
                {
                    substring += char.ToLower(text[j]);
                    if (j < text.Length - 1)
                        UpdateContextModel(substring, char.ToLower(text[j + 1]));
                }
            }

            return contextModels;
        }

        private static bool AllRulesBroken(string text, int position)
        {
            return Rules.All(rule => !rule.IsFullfilled(text, position));
        }
    }
}