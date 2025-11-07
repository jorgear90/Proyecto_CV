using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace CurriculumVitaeApp.Helpers
{
    public class IdProtector
    {
        private readonly string _key;

        public IdProtector(IConfiguration configuration)
        {
            _key = configuration["EncryptionSettings:Key"]
                ?? throw new Exception("Encryption key not found in appsettings.json");
        }

        public string EncryptId(int id)
        {
            using var aes = Aes.Create();
            var keyBytes = new Rfc2898DeriveBytes(_key, Encoding.UTF8.GetBytes("SaltFijo"), 1000).GetBytes(32);
            aes.Key = keyBytes;
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            var bytes = BitConverter.GetBytes(id);
            var cipher = encryptor.TransformFinalBlock(bytes, 0, bytes.Length);

            var result = Convert.ToBase64String(aes.IV.Concat(cipher).ToArray());
            return result.Replace("/", "_").Replace("+", "-"); // URL safe
        }

        public int DecryptId(string encrypted)
        {
            encrypted = encrypted.Replace("_", "/").Replace("-", "+");
            var data = Convert.FromBase64String(encrypted);

            using var aes = Aes.Create();
            var keyBytes = new Rfc2898DeriveBytes(_key, Encoding.UTF8.GetBytes("SaltFijo"), 1000).GetBytes(32);
            aes.Key = keyBytes;

            var iv = data.Take(16).ToArray();
            var cipher = data.Skip(16).ToArray();

            aes.IV = iv;
            using var decryptor = aes.CreateDecryptor();
            var plain = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);

            return BitConverter.ToInt32(plain, 0);
        }
    }
}
