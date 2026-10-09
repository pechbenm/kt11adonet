namespace KT11.Services
{
    internal static class CryptoInput
    {
        public static byte[] FromBase64(string value, string fieldName)
        {
            try
            {
                return Convert.FromBase64String(value.Trim());
            }
            catch (FormatException ex)
            {
                throw new CryptoServiceException($"{fieldName}: некорректная строка Base64.", ex);
            }
        }

        public static byte[] ParseKey(string base64Key, int expectedLength)
        {
            byte[] key = FromBase64(base64Key, "Ключ");

            if (key.Length != expectedLength)
            {
                throw new CryptoServiceException(
                    $"Ключ должен содержать ровно {expectedLength} байта ({expectedLength * 8} бит) в формате Base64, " +
                    $"а получено байт: {key.Length}.");
            }

            return key;
        }
    }
}
