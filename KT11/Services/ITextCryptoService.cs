namespace KT11.Services
{

    public record EncryptionResult(string CipherText, string Key);

    public interface ITextCryptoService
    {
        EncryptionResult Encrypt(string plainText, string? keyBase64);

        string Decrypt(string cipherTextBase64, string keyBase64);
    }
}
