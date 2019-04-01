using System;
using System.Diagnostics;

namespace Unchuris_04 {
    class Programm {
        public static void Main() {
            Stopwatch watch;
            while (true) {
                Console.Clear();
                Console.WriteLine("Алгоритм RSA\n" +
                    "Выберите режим работы:\n" +
                    "1 - Шифрование\n" +
                    "2 - Дешифрование\n" +
                    "3 - Генерация ключей\n" +
                    "4 - Выход");
                switch (Console.ReadKey(true).KeyChar) {
                    case '1':
                        var fileNameEncrypt = CustomFile.OpenFile("для шифрования");
                        watch = Stopwatch.StartNew();
                        Rsa.Encrypt(fileNameEncrypt);
                        ShowRunTime(watch.Elapsed);
                        Exit();
                        return;
                    case '2':
                        var fileNameDecrypt = CustomFile.OpenFile("для расшифрования");
                        watch = Stopwatch.StartNew();
                        Rsa.Decrypt(fileNameDecrypt);
                        ShowRunTime(watch.Elapsed);
                        Exit();
                        return;
                    case '3':
                        Console.Clear();
                        watch = Stopwatch.StartNew();
                        Rsa.Generate.GenerateKey();
                        ShowRunTime(watch.Elapsed);
                        Exit();
                        break;
                    case '4':
                        return;
                    default:
                        break;
                }
            }
        }

        private static void ShowRunTime(TimeSpan ts) {
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}",
                       ts.Hours, 
                       ts.Minutes, 
                       ts.Seconds);
            Console.WriteLine("Время выполнения: " + elapsedTime);
        }
        private static void Exit() {
            Console.WriteLine("Для завершения нажмите любую клавишу...");
            Console.ReadKey();
        }
    }
}
