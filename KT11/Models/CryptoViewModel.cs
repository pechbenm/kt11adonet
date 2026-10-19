namespace KT11.Models
{
    public class CryptoViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Algorithm { get; set; } = string.Empty;
        public string FormatNote { get; set; } = string.Empty;

        public EncryptRequest Encrypt { get; set; } = new();
        public DecryptRequest Decrypt { get; set; } = new();

        public string? EncryptedText { get; set; }
        public string? UsedKey { get; set; }
        public bool KeyWasGenerated { get; set; }

        public string? DecryptedText { get; set; }

        public string? EncryptError { get; set; }
        public string? DecryptError { get; set; }
    }
}
