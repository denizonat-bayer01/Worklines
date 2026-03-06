using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace wixi.Core.Utilities.Security.Protection
{
    /// <summary>
    /// AES-256-GCM based protector. Requires a 32-byte key provided via constructor (prefer Base64 from env).
    /// </summary>
    public sealed class AesSecretProtector : ISecretProtector
    {
        private readonly byte[] _key; // 32 bytes

        public AesSecretProtector(byte[] key)
        {
            if (key == null || key.Length != 32)
                throw new ArgumentException("AES key must be 32 bytes (256-bit).", nameof(key));
            _key = key;
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return plainText ?? string.Empty;

            using var aes = new AesGcm(_key);
            var nonce = RandomNumberGenerator.GetBytes(12);
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var cipher = new byte[plainBytes.Length];
            var tag = new byte[16];
            aes.Encrypt(nonce, plainBytes, cipher, tag);

            // format: base64(nonce)||base64(tag)||base64(cipher)
            return Convert.ToBase64String(nonce) + ":" + Convert.ToBase64String(tag) + ":" + Convert.ToBase64String(cipher);
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return cipherText ?? string.Empty;
            
            // Check if it's in the expected format (nonce:tag:cipher)
            var parts = cipherText.Split(':');
            if (parts.Length != 3) 
                throw new FormatException("Invalid cipher format: expected 'nonce:tag:cipher' format");

            try
            {
                var nonce = Convert.FromBase64String(parts[0]);
                var tag = Convert.FromBase64String(parts[1]);
                var cipher = Convert.FromBase64String(parts[2]);

                using var aes = new AesGcm(_key);
                var plain = new byte[cipher.Length];
                aes.Decrypt(nonce, cipher, tag, plain);
                return Encoding.UTF8.GetString(plain);
            }
            catch (FormatException ex)
            {
                throw new FormatException($"Invalid Base64 encoding in cipher parts: {ex.Message}", ex);
            }
        }
    }
}

