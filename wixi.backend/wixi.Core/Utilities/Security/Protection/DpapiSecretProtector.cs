using System;
using System.Security.Cryptography;
using System.Text;

namespace wixi.Core.Utilities.Security.Protection
{
    /// <summary>
    /// Windows DPAPI-based protector (User scope). Works only on Windows hosts.
    /// </summary>
    public sealed class DpapiSecretProtector : ISecretProtector
    {
        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return plainText ?? string.Empty;
            var bytes = Encoding.UTF8.GetBytes(plainText);
            var protectedBytes = ProtectedData.Protect(bytes, optionalEntropy: null, scope: DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(protectedBytes);
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return cipherText ?? string.Empty;
            var protectedBytes = Convert.FromBase64String(cipherText);
            var bytes = ProtectedData.Unprotect(protectedBytes, optionalEntropy: null, scope: DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}

