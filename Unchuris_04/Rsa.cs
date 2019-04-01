using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Unchuris_04 {
    class Rsa {

        private static BigInteger e, n, d = 0;

        public static void Encrypt(byte[] source) {
            if (!GetPublicKey()) return;

            List<byte[]> resultList = new List<byte[]>();

            int rsaBlockSize = Generate.KEY_LENGT_BYTE / 2 - 1;

            int blockSize = rsaBlockSize - OAEP.MIN_PADDING_SIZE;

            int round = (int)Math.Ceiling((double)source.Length / blockSize);

            for (int i = 0; i < round; i++) {
                byte[] modifiedText = OAEP.ApplyOAEP(source.SubArray(i * blockSize, blockSize), rsaBlockSize);
                resultList.Add(EncryptBlock(modifiedText));
            }

            byte[] encrtypted = resultList.SelectMany(a => a).ToArray();

            CustomFile.WriteAllBytes(encrtypted, "C:\\Users\\vlady\\Desktop\\empty1.txt");
        }

        public static void Decrypt(byte[] source) {
            if (!GetPrivateKey()) return;

            List<byte[]> resultList = new List<byte[]>();

            int rsaBlockSize = Generate.KEY_LENGT_BYTE / 2 - 1;

            int rsaBlockSizeInc = rsaBlockSize + 1;

            int blockSize = rsaBlockSize - OAEP.MIN_PADDING_SIZE;

            int round = (int)Math.Ceiling((double)source.Length / rsaBlockSizeInc);

            for (int i = 0; i < round; i++) {
                byte[] messageBlock = source.SubArray(i * rsaBlockSizeInc, rsaBlockSizeInc);
                byte[] decrypteBlock = DecryptBlock(messageBlock);
                byte[] decrypte = OAEP.RemoveOAEP(decrypteBlock);
                if (decrypte == null) {
                    Console.WriteLine("Блок " + (i + 1) + " был поврежден, ваш файл или ключ повреждены.");
                    resultList.Add(messageBlock);
                } else {
                    resultList.Add(decrypte);
                }
            }

            byte[] decrypted = resultList.SelectMany(i => i).ToArray();

            CustomFile.WriteAllBytes(decrypted);
        }

        private static byte[] EncryptBlock(byte[] block) {
            byte[] encryptBlock = new byte[block.Length + 1];

            BigInteger bigInteger = new BigInteger(block.Concat(new byte[] { 0 }).ToArray());
            byte[] encrypt = BigInteger.ModPow(bigInteger, e, n).ToByteArray();
            Array.Copy(encrypt, 0, encryptBlock, 0, encrypt.Length);

            return encryptBlock;
        }

        private static byte[] DecryptBlock(byte[] block) {
            byte[] decryptBlock = new byte[block.Length - 1];

            BigInteger bigInteger = new BigInteger(block.Concat(new byte[] { 0 }).ToArray());
            byte[] decrypt = BigInteger.ModPow(bigInteger, d, n).ToByteArray();
            var size = decrypt.Length > decryptBlock.Length ? decryptBlock.Length : decrypt.Length;
            Array.Copy(decrypt, 0, decryptBlock, 0, size);

            return decryptBlock;
        }

        private static bool GetPublicKey() {
            byte[] key = CustomFile.OpenFile("открытого ключа");

            if (key.Length != Generate.KEY_LENGT_BYTE) {
                Console.WriteLine("Ключ поврежден!");
                return false;
            }

            e = new BigInteger(key.SubArray(0, Generate.KEY_LENGT_BYTE / 2));
            n = new BigInteger(key.SubArray(Generate.KEY_LENGT_BYTE / 2, Generate.KEY_LENGT_BYTE / 2));

            Console.WriteLine("Открытый ключ (e, n): ({0}\n {1}).", e.ToString("X"), n.ToString("X"));
            Console.WriteLine("Выполнение...");

            return true;
        }

        private static bool GetPrivateKey() {
            byte[] key = CustomFile.OpenFile("закрытого ключа");

            if (key.Length != Generate.KEY_LENGT_BYTE) {
                Console.WriteLine("Ключ поврежден!");
                return false;
            }

            d = new BigInteger(key.SubArray(0, Generate.KEY_LENGT_BYTE / 2));
            n = new BigInteger(key.SubArray(Generate.KEY_LENGT_BYTE / 2, Generate.KEY_LENGT_BYTE / 2));

            Console.WriteLine("Закрытый ключ (d, n): ({0}\n {1}).", d.ToString("X"), n.ToString("X"));
            Console.WriteLine("Выполнение...");

            return true;
        }

        public class Generate {

            public static readonly int KEY_LENGT = 1024;
            public static readonly int KEY_LENGT_BYTE = 512;
            private static BigInteger n, p, q;
            private static readonly Random rand = new Random();

            public static void GenerateKey() {
                BigInteger fi, e, d;
                int size = KEY_LENGT_BYTE / 2;

                do {
                    generateNumber();

                    fi = (p - 1) * (q - 1);

                    for (e = n / 5; e < fi; e++) {
                        if (Euclid.Gcd(fi, e) == 1)
                            break;
                    }

                    d = Euclid.ModInverse(e, fi);

                    if ((e * d) % fi != 1 && d != 0) {
                        GenerateKey();
                        return;
                    }
                    
                } while (e.ToByteArray().Length != size && n.ToByteArray().Length != size && d.ToByteArray().Length != size);

                Console.WriteLine("Открытый ключ записан в файл public_key.txt, закрытый - в private_key.txt");
                File.WriteAllBytes("public_key.txt", e.ToByteArray().Concat(n.ToByteArray()).ToArray());
                File.WriteAllBytes("private_key.txt", d.ToByteArray().Concat(n.ToByteArray()).ToArray());
            }

            public static void generateNumber() {
                q = BigIntegerExtensions.GetBigIntegerRandomPrimeNumber(Generate.KEY_LENGT);
                p = BigIntegerExtensions.GetBigIntegerRandomPrimeNumber(Generate.KEY_LENGT);
                n = p * q;
            }
        }
    }
}
