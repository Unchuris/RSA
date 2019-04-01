using System;
using System.Numerics;
using System.Security.Cryptography;

namespace Unchuris_04 {
    public static class BigIntegerExtensions {

        public static BigInteger GetBigIntegerRandomPrimeNumber(int bitLength) {
            BigInteger n;

            do {
                n = NextBigInteger(bitLength);
            } while (!n.IsProbablePrime());

            return n;
        }

        private static BigInteger NextBigInteger(int bitLength) {
            if (bitLength < 1) return BigInteger.Zero;

            Random random = new Random();

            int byteSizeInBits = 8;

            int bytes = bitLength / byteSizeInBits;
            int bits = bitLength % byteSizeInBits;

            byte[] result = new byte[bytes + 1];
            random.NextBytes(result);

            byte mask = (byte)(0xFF >> (byteSizeInBits - bits));
            result[result.Length - 1] &= mask;

            return new BigInteger(result);
        }
    }
}
