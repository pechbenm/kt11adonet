using System.Security.Cryptography;
using System.Text;
using Sodium;

namespace KT11.Services
{

    public sealed class LibsodiumService : ITextCryptoService
    {
        private const int KeySize = 32;
        private const int NonceSize = 24;
        private const int MacSize = 16;

        public EncryptionResult Encrypt(string plainText, string? keyBase64)
        {
            byte[] key = string.IsNullOrWhiteSpace(keyBase64)
                ? SecretBox.GenerateKey()
                : CryptoInput.ParseKey(keyBase64, KeySize);

            byte[] nonce = SecretBox.GenerateNonce();
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] box = SecretBox.Create(plainBytes, nonce, key);

            byte[] packed = new byte[NonceSize + box.Length];
            Buffer.BlockCopy(nonce, 0, packed, 0, NonceSize);
            Buffer.BlockCopy(box, 0, packed, NonceSize, box.Length);

            return new EncryptionResult(Convert.ToBase64String(packed), Convert.ToBase64String(key));
        }

        public string Decrypt(string cipherTextBase64, string keyBase64)
        {
            byte[] key = CryptoInput.ParseKey(keyBase64, KeySize);
            byte[] packed = CryptoInput.FromBase64(cipherTextBase64, "Шифртекст");

            if (packed.Length < NonceSize + MacSize)
            {
                throw new CryptoServiceException("Шифртекст слишком короткий: данные повреждены или это не результат шифрования libsodium.");
            }

            byte[] nonce = new byte[NonceSize];
            byte[] box = new byte[packed.Length - NonceSize];
            Buffer.BlockCopy(packed, 0, nonce, 0, NonceSize);
            Buffer.BlockCopy(packed, NonceSize, box, 0, box.Length);

            try
            {
                byte[] plainBytes = SecretBox.Open(box, nonce, key);
                return Encoding.UTF8.GetString(plainBytes);
            }
            catch (CryptographicException ex)
            {
                throw new CryptoServiceException("Не удалось расшифровать: неверный ключ или данные повреждены.", ex);
            }
        }
    }
}
