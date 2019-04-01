using System.Numerics;
namespace Unchuris_04 {
    class Euclid {

        public static BigInteger Gcd(BigInteger firstNumber, BigInteger secondNumber) {
            return secondNumber == 0 ? firstNumber : Gcd(secondNumber, firstNumber % secondNumber);
        }

        public static BigInteger ModInverse(BigInteger a1, BigInteger mod) {
            if (Gcd(a1, mod) != 1) return 0;

            BigInteger a = a1;
            BigInteger m = mod;
            BigInteger y = 0;
            BigInteger x = 1;

            while (a > 1) {

                BigInteger q = a / m;
                BigInteger t = m;
                m = a % m;
                a = t;
                t = y;

                y = x - q * y;
                x = t;
            }

            if (x < 0) {
                x += mod;
            }

            return x;
        }
    }
}
