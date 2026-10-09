using KT11.Services;

namespace KT11.Controllers
{
    public class LibsodiumController : CryptoControllerBase
    {
        public LibsodiumController(LibsodiumService service)
            : base(
                service,
                title: "Libsodium — шифрование SecretBox",
                algorithm: "XSalsa20-Poly1305 (crypto_secretbox), библиотека Sodium.Core",
                formatNote: "Результат — строка Base64 вида: nonce (24 байта) ‖ MAC (16 байт) ‖ шифртекст. " +
                            "Ключ — 32 случайных байта в Base64.")
        {
        }
    }
}
