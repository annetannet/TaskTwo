using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskTwo
{
    public class ArithmeticEncoder
    {
        private readonly int n;
        private readonly ulong intervalExpansion; // 2 ^ (N - 1)
        private readonly ulong maxValue; // 2 ^ N

        public ArithmeticEncoder(int window)
        {
            n = window;
            intervalExpansion = (ulong)Math.Pow(2, n - 1);
            maxValue = (ulong)Math.Pow(2, n);
        }

        public byte[] Encode(byte[] source)
        {
            var totalBits = 0;
            var result = new List<byte> { 0 };
            var offset = 0;
            var byteOffset = 7;
            var counter = Enumerable.Repeat(1, 256).ToArray();
            var l = 0ul;
            var h = maxValue - 1;
            var bits = 0;

            void DecrementByteOffset()
            {
                byteOffset = (byteOffset + 7) % 8;
                totalBits++;
                if (byteOffset != 7) return;
                offset++;
                result.Add(0);
            }

            foreach (var b in source)
            {
                if (offset >= result.Count)
                    result.Add(0);
                (l, h) = Project(counter, b, l, h);
                var off = (ulong)1 << (n - 1);
                if ((l & off) == 0 && (h & off) == off)
                {
                    bits = GetBits(l, h);
                    for (var i = 0; i < bits; i++)
                    {
                        l = (l << 1) - intervalExpansion;
                        h = (h << 1) - intervalExpansion + 1;
                    }
                }
                else
                {
                    var firstBit = (byte)((l & off) > 0 ? 1 << byteOffset : 0);
                    result[offset] |= firstBit;
                    DecrementByteOffset();

                    l <<= 1;
                    h = (h << 1) | 1;

                    for (var i = 0; i < bits; i++)
                    {
                        result[offset] |= (byte)(firstBit == 0 ? 1 << byteOffset : 0);
                        DecrementByteOffset();
                    }

                    bits = 0;

                    for (var i = n - 2; i >= 0; i--)
                    {
                        if ((l & off) != (h & off))
                            break;
                        result[offset] |= (byte)((l & off) > 0 ? 1 << byteOffset : 0);
                        DecrementByteOffset();

                        l <<= 1;
                        h = (h << 1) | 1;
                    }
                }

                counter[b]++;
            }

            Console.WriteLine($"The length of the arithmetic encoder sequence: {totalBits}");
            return result.ToArray();
        }

        private (ulong, ulong) Project(int[] counter, byte element, ulong l, ulong h)
        {
            var c = 0;
            for (var i = 0; i < element; i++)
                c += counter[i];
            var summaryCount = counter.Sum();
            var alpha = c / (double)summaryCount;
            var beta = (c + counter[element]) / (double)summaryCount;
            var a = Math.Ceiling(alpha * maxValue);
            var b = Math.Ceiling(beta * maxValue) - 1;
            var newL = l + Math.Ceiling(a * (h - l + 1) / maxValue);
            var newH = l + Math.Ceiling((b + 1) * (h - l + 1) / maxValue) - 1;
            return ((ulong)newL, (ulong)newH);
        }


        private int GetBits(ulong l, ulong h)
        {
            var offset = n - 2;
            var counter = 0;
            while (offset >= 0)
            {
                var o = (ulong)1 << offset;
                if ((l & o) == o && (h & o) == 0)
                    counter++;
                else
                    break;
                offset--;
            }

            return counter;
        }
    }
}