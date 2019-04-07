using System;
using System.Numerics;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace Unchuris_04 {
    class Rsa {

        private static BigInteger e = 65537;
        private static BigInteger n, d = 0;
        private static int blockSize = 0;
        private static readonly int HASH_SIZE = 16;
        private static readonly int TRASH_BLOCK = HASH_SIZE + 4;

        public static void Encrypt(byte[] source) {
            if (!GetPublicKey()) return;

            StringBuilder bits = new StringBuilder(source.ToBitsString());

            StringBuilder result = new StringBuilder();

            int rsaBlockSize = blockSize;

            byte[] startBytes = GetBytesToCheckMessage(bits.ToString());

            int addBitsCount = rsaBlockSize - (bits.Length % rsaBlockSize);

            for (int i = 0; i < addBitsCount; i++) {
                bits.Append('0');
            }

            for (int i = 0; i < bits.Length; i += rsaBlockSize) {
                string blockMessage = bits.ToString(i, rsaBlockSize);
                result.Append(EncryptBlock(blockMessage));
            }

            byte[] encrypted = startBytes.Concat(result.ToString().ToBytes()).ToArray();

            CustomFile.WriteAllBytes(encrypted, "C:\\Users\\vlady\\Desktop\\result.txt");
        }

        public static void Decrypt(byte[] source) {
            if (source.Length <= TRASH_BLOCK) {
                ShowErrorMessage();
                return;
            }

            if (!GetPrivateKey()) return;

            StringBuilder result = new StringBuilder();

            int rsaBlockSize = Generate.KEY_LENGT_BITS;

            byte[] data = source.SubArray(TRASH_BLOCK, source.Length - TRASH_BLOCK);

            StringBuilder bits = new StringBuilder(data.ToBitsString());

            if (bits.Length % rsaBlockSize != 0) {
                ShowErrorMessage();
                return;
            }

            BigInteger messageRealLength = new BigInteger(source.SubArray(HASH_SIZE, 4).Concat(new byte[] { 0 }).ToArray());

            for (int i = 0; i < bits.Length; i += rsaBlockSize) {
                string blockMessage = bits.ToString(i, rsaBlockSize);
                result.Append(DecryptBlock(blockMessage));
            }

            string dataBlock = result.ToString(0, (int)messageRealLength);

            byte[] currentHashMessage = GetHash(dataBlock);

            byte[] hashMessage = source.SubArray(0, HASH_SIZE);

            if (currentHashMessage.SequenceEqual(hashMessage)) {
                CustomFile.WriteAllBytes(dataBlock.ToBytes());
            } else {
                ShowErrorMessage();
            }
        }

        private static string EncryptBlock(string block) {
            BigInteger bigInt = block.ToBigInteger();
            string encrypt = BigInteger.ModPow(block.ToBigInteger(), e, n).ToBinaryString();
            return encrypt.PadLeft(Generate.KEY_LENGT_BITS, '0');
        }

        private static string DecryptBlock(string block) {
            BigInteger bigInt = BigInteger.ModPow(block.ToBigInteger(), d, n);
            string decrypt = bigInt.ToBinaryString();
            return decrypt.PadLeft(blockSize - 1, '0'); 
        }

        private static byte[] GetBytesToCheckMessage(string bits) {
            byte[] messageSize = new byte[4];

            messageSize[0] = (byte)(bits.Length % 256);
            messageSize[1] = (byte)(bits.Length % 65536 / 256);
            messageSize[2] = (byte)(bits.Length % 16777216 / 65536);
            messageSize[3] = (byte)(bits.Length / 16777216);

            byte[] result = new byte[HASH_SIZE + messageSize.Length];

            Array.Copy(GetHash(bits), result, HASH_SIZE);

            Array.Copy(messageSize, 0, result, HASH_SIZE, messageSize.Length);

            return result;
        }

        private static byte[] GetHash(string bits) {
            return MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(bits));
        }

        private static void ShowErrorMessage() {
            Console.WriteLine("Ошибка, зашифрованный файл был модифицирован.");
        }

        private static bool GetPublicKey() {
            byte[] key = CustomFile.OpenFile("открытого ключа");

            if (key.Length != Generate.KEY_LENGT_BYTE / 2) {
                Console.WriteLine("Ключ поврежден!");
                return false;
            }

            n = new BigInteger(key.SubArray(0, Generate.KEY_LENGT_BYTE / 2));

            blockSize = (int)Math.Floor(BigInteger.Log(n, 2));
            Console.WriteLine("Выполнение...");
            return true;
        }

        private static bool GetPrivateKey() {
            byte[] key = CustomFile.OpenFile("закрытого ключа");

            if (key.Length < Generate.KEY_LENGT_BYTE / 2) {
                Console.WriteLine("Ключ поврежден!");
                return false;
            }

            n = new BigInteger(key.SubArray(0, Generate.KEY_LENGT_BYTE / 2));
            d = new BigInteger(key.SubArray(Generate.KEY_LENGT_BYTE / 2, Generate.KEY_LENGT_BYTE));

            blockSize = (int)Math.Ceiling(BigInteger.Log(n, 2));
            Console.WriteLine("Выполнение...");
            return true;
        }

        public class Generate {

            public static readonly int KEY_LENGT = 1024 * 2;
            public static readonly int KEY_LENGT_BYTE = 512 * 2;
            public static readonly int KEY_LENGT_BITS = 4096;
            private static BigInteger n, p, q;
            private static readonly Random rand = new Random();

            public static void GenerateKey() {
                BigInteger fi, d;
                int size = KEY_LENGT_BYTE / 2;

                do {
                    generateNumber();

                    fi = (p - 1) * (q - 1);

                    d = Euclid.ModInverse(e, fi);

                    if ((e * d) % fi != 1 && d != 0) {
                        GenerateKey();
                        return;
                    }

                } while (n.ToByteArray().Length != size);

                Console.WriteLine("Открытый ключ записан в файл public_key.txt, закрытый - в private_key.txt");
                File.WriteAllBytes("public_key.txt", n.ToByteArray().ToArray());
                File.WriteAllBytes("private_key.txt", n.ToByteArray().Concat(d.ToByteArray()).ToArray());
            }

            public static void generateNumber() {
                q = BigIntegerExtensions.GetBigIntegerRandomPrimeNumber(Generate.KEY_LENGT);
                p = BigIntegerExtensions.GetBigIntegerRandomPrimeNumber(Generate.KEY_LENGT - 1);
                n = p * q;
            }
        }
    }
}
