using System;
using System.Security.Cryptography;
using System.Text;

namespace Unchuris_04 {
    class OAEP {

        public static readonly int MIN_PADDING_SIZE = 65;

        private static readonly string MAGIC_STRING = "SHA-256 MGF1";

        private static readonly int HASH_SIZE = 32;

        public static byte[] ApplyOAEP(byte[] message, int length) {
            Random random = new Random();

            int messageSize = message.Length;
            int hashSize = HASH_SIZE;

            if (messageSize > length - (hashSize << 1) - 1) {
                return null;
            }

            int zeroPad = length - messageSize - (hashSize << 1) - 1;
            byte[] dataBlock = new byte[length - hashSize];
            Array.Copy(HashAlgorithm(Encoding.UTF8.GetBytes(MAGIC_STRING)), 0, dataBlock, 0, hashSize);
            Array.Copy(message, 0, dataBlock, hashSize + zeroPad + 1, messageSize);
            dataBlock[hashSize + zeroPad] = 1;
            byte[] seed = new byte[hashSize];

            random.NextBytes(seed);

            byte[] dataBlockMask = MGF1(seed, 0, hashSize, length - hashSize);
            for (int i = 0; i < length - hashSize; i++) {
                dataBlock[i] ^= dataBlockMask[i];
            }

            byte[] seedMask = MGF1(dataBlock, 0, length - hashSize, hashSize);
            for (int i = 0; i < hashSize; i++) {
                seed[i] ^= seedMask[i];
            }

            byte[] padded = new byte[length];
            Array.Copy(seed, 0, padded, 0, hashSize);
            Array.Copy(dataBlock, 0, padded, hashSize, length - hashSize);

            return padded;
        }

        public static byte[] RemoveOAEP(byte[] message) {
            int messageSize = message.Length;
            int hashSize = HASH_SIZE;

            if (messageSize < (hashSize << 1) + 1) {
                return null;
            }

            byte[] copy = new byte[messageSize];
            Array.Copy(message, 0, copy, 0, messageSize);
            byte[] seedMask = MGF1(copy, hashSize, messageSize - hashSize, hashSize);
            for (int i = 0; i < hashSize; i++) {
                copy[i] ^= seedMask[i];
            }
            byte[] paramsHash = HashAlgorithm(Encoding.UTF8.GetBytes(MAGIC_STRING));
            byte[] dataBlockMask = MGF1(copy, 0, hashSize, messageSize - hashSize);
            int index = -1;
            for (int i = hashSize; i < messageSize; i++) {
                copy[i] ^= dataBlockMask[i - hashSize];
                if (i < (hashSize << 1)) {
                    if (copy[i] != paramsHash[i - hashSize]) {
                        return null;
                    }
                } else if (index == -1) {
                    if (copy[i] == 1) {
                        index = i + 1;
                    }
                }
            }

            if (index == -1 || index == messageSize) {
                return null;
            }

            byte[] unpadded = new byte[messageSize - index];
            Array.Copy(copy, index, unpadded, 0, messageSize - index);

            return unpadded;
        }

        static byte[] MGF1(byte[] seed, int seedOffset, int seedLength, int desiredLength) {
            int hashSize = HASH_SIZE;
            int offset = 0;
            int i = 0;
            byte[] mask = new byte[desiredLength];
            byte[] temp = new byte[seedLength + 4];

            Array.Copy(seed, seedOffset, temp, 4, seedLength);

            while (offset < desiredLength) {
                temp[0] = (byte)(i >> 24);
                temp[1] = (byte)(i >> 16);
                temp[2] = (byte)(i >> 8);
                temp[3] = (byte)i;
                int remaining = desiredLength - offset;
                Array.Copy(HashAlgorithm(temp), 0, mask, offset, remaining < hashSize ? remaining : hashSize);
                offset = offset + hashSize;
                i = i + 1;
            }

            return mask;
        }

        static byte[] HashAlgorithm(byte[] input) {
            return SHA256Cng.Create().ComputeHash(input);
        }
    }
}
