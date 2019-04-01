using System;
using System.Numerics;
using System.Security.Cryptography;

namespace Unchuris_04 {
    static class Extensions {
        public static string ToBinary(this BigInteger source) {
            string result = "";

            while (source > 0) {
                result = (source % 2).ToString() + result;
                source = source >> 1;
            }

            return result;
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
    }
}
