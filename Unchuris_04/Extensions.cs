using System;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace Unchuris_04 {
    static class Extensions {

        public static string PadLeftRandom(this string source, int size) {
            string result = source.PadLeft(16, '0');
            result = GetRandomString(size - result.Length) + result;
            return result;
        }

        public static string ToBitsString(this byte[] source) {
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < source.Length; i++) {
                result.Append(Convert.ToString(source[i], 2).PadLeft(8, '0'));
            }

            return result.ToString();
        }

        public static byte[] ToBytes(this string source) {
            byte[] result = new byte[source.Length / 8];

            for (int i = 0, j = 0; i < result.Length; i++, j += 8) {
                result[i] = Convert.ToByte(source.Substring(j, 8), 2);
            }

            return result;
        }

        public static string ToBinaryString(this BigInteger source) {
            string result = "";
            while (source > 0) {
                result = (source % 2).ToString() + result;
                source = source >> 1;
            }
            return result;
        }
        public static BigInteger ToBigInteger(this string value) {
            BigInteger res = 0;

            foreach (char c in value) {
                res <<= 1;
                res += c == '1' ? 1 : 0;
            }

            return res;
        }

        public static T[] SubArray<T>(this T[] data, int index, int length) {
            int lastIndex = data.Length < (index + length) ? (data.Length - index) : length;
            T[] result = new T[lastIndex];
            Array.Copy(data, index, result, 0, lastIndex);
            return result;
        }

        public static bool IsProbablePrime(this BigInteger source) {
            if (source == 2 || source == 3)
                return true;
            if (source < 2 || source % 2 == 0)
                return false;

            //source − 1 = (2^s)·t

            BigInteger t = source - 1;
            int s = 0;

            while (t % 2 == 0) {
                t /= 2;
                s += 1;
            }

            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] bytes = new byte[source.ToByteArray().LongLength];
            BigInteger randomNumber;

            for (int i = 0; i < source.ToString().Length; i++) {
                do {
                    rng.GetBytes(bytes);
                    randomNumber = new BigInteger(bytes);
                }
                while (randomNumber < 2 || randomNumber >= source - 2);

                BigInteger x = BigInteger.ModPow(randomNumber, t, source);
                if (x == 1 || x == source - 1)
                    continue;

                for (int r = 1; r < s; r++) {
                    x = BigInteger.ModPow(x, 2, source);
                    if (x == 1)
                        return false;
                    if (x == source - 1)
                        break;
                }

                if (x != source - 1)
                    return false;
            }

            return true;
        }

        public static string GetRandomString(int size) {
            Random random = new Random();
            byte[] result = new byte[size];
            random.NextBytes(result);
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < size; i++) {
                stringBuilder.Append((char)i);
            }
            return stringBuilder.ToString();
        }
    }
}
