using System;

namespace wixi.Core.Utilities.Security.Protection
{
    /// <summary>
    /// Chooses AES if MAIL_ENC_KEY (Base64 32-byte) is provided; otherwise falls back to DPAPI (Windows).
    /// </summary>
    public sealed class CompositeSecretProtector : ISecretProtector
    {
        private readonly ISecretProtector _inner;

        public CompositeSecretProtector()
        {
            var keyB64 = Environment.GetEnvironmentVariable("MAIL_ENC_KEY");
            if (!string.IsNullOrWhiteSpace(keyB64))
            {
                var key = Convert.FromBase64String(keyB64);
                _inner = new AesSecretProtector(key);
            }
            else
            {
                _inner = new DpapiSecretProtector();
            }
        }

        public string Encrypt(string plainText) => _inner.Encrypt(plainText);
        public string Decrypt(string cipherText) => _inner.Decrypt(cipherText);
    }
}

