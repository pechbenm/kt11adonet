using KT11.Services;

namespace KT11.Controllers
{
    public class BouncyCastleController : CryptoControllerBase
    {
        public BouncyCastleController(BouncyCastleAesService service)
            : base(
                service,
                title: "Bouncy Castle — шифрование AES",
                algorithm: "AES-256-GCM, библиотека BouncyCastle.Cryptography",
                formatNote: "Результат — строка Base64 вида: nonce (12 байт) | шифртекст | тег аутентификации (16 байт). " +
                            "Ключ — 32 случайных байта в Base64.")
        {
        }
    }
}
