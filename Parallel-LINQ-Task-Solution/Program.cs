using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Parallel_LINQ_Task_Solution
{
    internal class Program
    {
        static void Main()
        {
            string wordLookupFile = Path.Combine(Path.GetTempPath(), "WordLookup.txt");

            if (!File.Exists(wordLookupFile)) // Contains about 150000 words
            {
                new WebClient().DownloadFile("https://csharpcooking.github.io/data/allwords.txt", wordLookupFile);
            }

            var words = File.ReadAllLines(wordLookupFile);
            string pattern = @".*z$"; // For example, all words ending in 'z'
            Regex regex = new Regex(pattern);

            using (Aes aesAlg = Aes.Create())
            {
                byte[] iv = { 15, 122, 132, 5, 93, 198, 44, 31, 9, 39, 241, 49, 250, 188, 80, 7 };
                string password = "Password for key generation";
                byte[] key;
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                    key = sha256.ComputeHash(passwordBytes);
                }

                var encryptor = new ThreadLocal<ICryptoTransform>(() => aesAlg.CreateEncryptor(key, iv), trackAllValues: true);
                var decryptor = new ThreadLocal<ICryptoTransform>(() => aesAlg.CreateDecryptor(key, iv), trackAllValues: true);

                var encryptedWords = words.AsParallel()
                                          .Where(word => regex.IsMatch(word))
                                          .Select(word => Encrypt(word, encryptor))
                                          .ToList();

                foreach (var encryptedWord in encryptedWords)
                {
                    string decryptedWord = Decrypt(encryptedWord, decryptor);
                    Console.WriteLine($"Encrypted: {Convert.ToBase64String(encryptedWord)}, Decrypted: {decryptedWord}");
                }

                // Release ICryptoTransform resources in each thread
                foreach (var enc in encryptor.Values)
                {
                    enc.Dispose();
                }

                foreach (var dec in decryptor.Values)
                {
                    dec.Dispose();
                }

                // Release ThreadLocal<ICryptoTransform> resources 
                encryptor.Dispose();
                decryptor.Dispose();
            }
        }

        static byte[] Encrypt(string plainText, ThreadLocal<ICryptoTransform> encryptor)
        {
            byte[] data = Encoding.UTF8.GetBytes(plainText);
            byte[] encryptedData;
            using (MemoryStream ms = new MemoryStream())
            {
                using (Stream c = new CryptoStream(ms, encryptor.Value, CryptoStreamMode.Write))
                {
                    c.Write(data, 0, data.Length);
                }
                encryptedData = ms.ToArray();
            }
            return encryptedData;
        }

        static string Decrypt(byte[] cipherText, ThreadLocal<ICryptoTransform> decryptor)
        {
            using (MemoryStream msInput = new MemoryStream(cipherText))
            using (CryptoStream cryptoStream = new CryptoStream(msInput, decryptor.Value, CryptoStreamMode.Read))
            using (MemoryStream msOutput = new MemoryStream())
            {
                cryptoStream.CopyTo(msOutput); // Copy decrypted data to msOutput
                return Encoding.UTF8.GetString(msOutput.ToArray()); // Convert to string
            }
        }
    }
}