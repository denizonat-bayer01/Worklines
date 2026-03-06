using System;

namespace wixi.Core.Utilities.Security.Protection
{
    public interface ISecretProtector
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
    }
}

