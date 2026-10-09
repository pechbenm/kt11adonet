using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace KT11.Services
{

    public sealed class BouncyCastleAesService : ITextCryptoService
    {
        private const int KeySize = 32;     
        private const int NonceSize = 12;    
        private const int MacSizeBits = 128; 
        private const int MacSize = MacSizeBits / 8;

        public EncryptionResult Encrypt(string plainText, string? keyBase64)
        {
            var random = new SecureRandom();

            byte[] key;
            if (string.IsNullOrWhiteSpace(keyBase64))
            {
                key = new byte[KeySize];
                random.NextBytes(key);
            }
            else
            {
                key = CryptoInput.ParseKey(keyBase64, KeySize);
            }

            byte[] nonce = new byte[NonceSize];
            random.NextBytes(nonce);

            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

            try
            {
                var cipher = new GcmBlockCipher(new AesEngine());
                cipher.Init(true, new AeadParameters(new KeyParameter(key), MacSizeBits, nonce));

                byte[] encrypted = new byte[cipher.GetOutputSize(plainBytes.Length)];
                int written = cipher.ProcessBytes(plainBytes, 0, plainBytes.Length, encrypted, 0);
                cipher.DoFinal(encrypted, written);

                byte[] packed = new byte[NonceSize + encrypted.Length];
                Buffer.BlockCopy(nonce, 0, packed, 0, NonceSize);
                Buffer.BlockCopy(encrypted, 0, packed, NonceSize, encrypted.Length);

                return new EncryptionResult(Convert.ToBase64String(packed), Convert.ToBase64String(key));
            }
            catch (InvalidCipherTextException ex)
            {
                throw new CryptoServiceException("Ошибка при шифровании.", ex);
            }
        }

        public string Decrypt(string cipherTextBase64, string keyBase64)
        {
            byte[] key = CryptoInput.ParseKey(keyBase64, KeySize);
            byte[] packed = CryptoInput.FromBase64(cipherTextBase64, "Шифртекст");

            if (packed.Length < NonceSize + MacSize)
            {
                throw new CryptoServiceException("Шифртекст слишком короткий: данные повреждены или это не результат шифрования AES-GCM.");
            }

            byte[] nonce = new byte[NonceSize];
            Buffer.BlockCopy(packed, 0, nonce, 0, NonceSize);
            int cipherLength = packed.Length - NonceSize;

            try
            {
                var cipher = new GcmBlockCipher(new AesEngine());
                cipher.Init(false, new AeadParameters(new KeyParameter(key), MacSizeBits, nonce));

                byte[] output = new byte[cipher.GetOutputSize(cipherLength)];
                int written = cipher.ProcessBytes(packed, NonceSize, cipherLength, output, 0);
                written += cipher.DoFinal(output, written);

                return Encoding.UTF8.GetString(output, 0, written);
            }
            catch (InvalidCipherTextException ex)
            {
                throw new CryptoServiceException("Не удалось расшифровать: неверный ключ или данные повреждены.", ex);
            }
        }
    }
}
