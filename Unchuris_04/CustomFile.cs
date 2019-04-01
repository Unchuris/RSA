using System;
using System.IO;

namespace Unchuris_04 {
    class CustomFile {

        public static byte[] OpenFile(string message, string defaultFileName = null) {
            byte[] file;

            do {
                string fileName = "";
                Console.WriteLine("Введите имя файла " + message + ":");
                while (fileName == "") {
                    fileName = Console.ReadLine();
                }
                while (!File.Exists(fileName)) {
                    Console.WriteLine("Ошибка! Не удалось открыть файл, введите другой путь к файлу:");
                    fileName = Console.ReadLine();
                }
                file = File.ReadAllBytes(fileName);
                if (file.Length == 0) {
                    Console.WriteLine("Файл пуст!");
                }
            } while (file.Length < 1);
            return file;
        }

        public static void WriteAllBytes(byte[] bytes, String defaultName = null) {
            if (defaultName == null) {
                string fileName = "";
                Console.WriteLine("Введите имя файла для записи результата:");
                while (fileName == "") {
                    fileName = Console.ReadLine();
                }
                File.WriteAllBytes(fileName, bytes);
            } else {
                File.WriteAllBytes(defaultName, bytes);
            }
        }
    }
}
