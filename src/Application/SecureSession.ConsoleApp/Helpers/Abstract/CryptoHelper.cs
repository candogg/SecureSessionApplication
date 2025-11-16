using SecureSession.ConsoleApp.Extensions;
using SecureSession.ConsoleApp.Helpers.Base;
using SecureSession.ConsoleApp.Items;
using System.Security.Cryptography;
using System.Text;

namespace SecureSession.ConsoleApp.Helpers.Abstract
{
    /// <summary>
    /// Author: Can DOĞU
    /// </summary>
    public sealed class CryptoHelper : HelperBase<CryptoHelper>
    {
        public KeyItem GetKeyPair(int keySize = 4096)
        {
            using var rsa = RSA.Create(keySize);

            var pkcs8PrivateKey = rsa.ExportPkcs8PrivateKey();
            var publicKey = rsa.ExportSubjectPublicKeyInfo();

            var privatePem = ToPem("PRIVATE KEY", pkcs8PrivateKey);
            var publicPem = ToPem("PUBLIC KEY", publicKey);

            return new KeyItem
            {
                PublicKey = publicPem,
                PrivateKey = privatePem
            };
        }

        private static string ToPem(string label, byte[] data)
        {
            const int lineLength = 64;

            var b64 = Convert.ToBase64String(data);

            StringBuilder sb = new();

            sb.AppendLine($"-----BEGIN {label}-----");

            for (var i = 0; i < b64.Length; i += lineLength)
            {
                var len = Math.Min(lineLength, b64.Length - i);
                sb.AppendLine(b64.Substring(i, len));
            }

            sb.AppendLine($"-----END {label}-----");

            return sb.ToString();
        }

        public string DecryptDataWithPrivate(string? data, string? privatePem)
        {
            if (privatePem.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(privatePem));
            }

            if (data.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(data));
            }

            using var rsa = RSA.Create();
            rsa.ImportFromPem(privatePem!.ToCharArray());

            if (rsa is null)
                ArgumentNullException.ThrowIfNull(rsa);

            var encryptedBytes = Convert.FromBase64String(data!);

            Span<byte> decrypted = stackalloc byte[rsa.KeySize / 8];

            if (!rsa.TryDecrypt(encryptedBytes, decrypted, RSAEncryptionPadding.OaepSHA256, out var bytesWritten))
            {
                throw new CryptographicException("Hatalı veri");
            }

            return Encoding.UTF8.GetString(decrypted[..bytesWritten]);
        }

        public string EncryptDataWithPublic(string? data, string? publicPem)
        {
            if (publicPem.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(publicPem));
            }

            if (data.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(data));
            }

            using var rsa = RSA.Create();
            rsa.ImportFromPem(publicPem!.ToCharArray());

            var plainBytes = Encoding.UTF8.GetBytes(data!);

            Span<byte> encrypted = stackalloc byte[rsa.KeySize / 8];

            if (!rsa.TryEncrypt(plainBytes, encrypted, RSAEncryptionPadding.OaepSHA256, out var written))
            {
                throw new CryptographicException("Hatalı veri");
            }

            return Convert.ToBase64String(encrypted[..written]);
        }

        public string DecryptDataWithPrivateInChunks(string? data, string? privatePem)
        {
            if (privatePem.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(privatePem));
            }

            if (data.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(data));
            }

            using var rsa = RSA.Create();
            rsa.ImportFromPem(privatePem!.ToCharArray());

            var chunks = data!.Split("»", StringSplitOptions.RemoveEmptyEntries);
            var decryptedBytes = new List<byte>(chunks.Length * (rsa.KeySize / 8));

            foreach (var chunk in chunks)
            {
                var encryptedChunk = Convert.FromBase64String(chunk);
                var decryptedChunk = rsa.Decrypt(encryptedChunk, RSAEncryptionPadding.OaepSHA256);

                decryptedBytes.AddRange(decryptedChunk);
            }

            return Encoding.UTF8.GetString([.. decryptedBytes]);
        }

        public string EncryptDataWithPublicInChunks(string? data, string? publicPem)
        {
            if (publicPem.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(publicPem));
            }

            if (data.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(data));
            }

            using var rsa = RSA.Create();
            rsa.ImportFromPem(publicPem!.ToCharArray());

            var plainBytes = Encoding.UTF8.GetBytes(data!);
            var keySizeBytes = rsa.KeySize / 8;
            var hashSize = 32;
            var maxChunkSize = keySizeBytes - 2 * hashSize - 2;

            var encryptedChunks = new List<string>();

            for (var offset = 0; offset < plainBytes.Length; offset += maxChunkSize)
            {
                var chunkSize = Math.Min(maxChunkSize, plainBytes.Length - offset);
                var chunk = plainBytes[offset..(offset + chunkSize)];

                var encryptedChunk = rsa.Encrypt(chunk, RSAEncryptionPadding.OaepSHA256);

                encryptedChunks.Add(Convert.ToBase64String(encryptedChunk));
            }

            return string.Join("»", encryptedChunks);
        }

        public string? EncryptString(string? input, string? externalKey, string? externalIv)
        {
            if (input.IsNullOrEmpty() || externalKey.IsNullOrEmpty() || externalIv.IsNullOrEmpty())
            {
                return null;
            }

            using var aesAlg = Aes.Create();

            aesAlg.Key = Encoding.UTF8.GetBytes(externalKey!);
            aesAlg.IV = Encoding.UTF8.GetBytes(externalIv!);

            var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using var msEncrypt = new MemoryStream();
            using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);

            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(input);
            }

            return Convert.ToBase64String(msEncrypt.ToArray());
        }

        public string? DecryptString(string input, string? externalKey, string? externalIv)
        {
            if (input.IsNullOrEmpty() || externalKey.IsNullOrEmpty() || externalIv.IsNullOrEmpty())
            {
                return null;
            }

            using var aesAlg = Aes.Create();

            aesAlg.Key = Encoding.UTF8.GetBytes(externalKey!);
            aesAlg.IV = Encoding.UTF8.GetBytes(externalIv!);

            var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using var msDecrypt = new MemoryStream(Convert.FromBase64String(input));
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);

            return srDecrypt.ReadToEnd();
        }
    }
}
