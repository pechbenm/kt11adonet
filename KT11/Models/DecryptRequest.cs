using System.ComponentModel.DataAnnotations;

namespace KT11.Models
{
    public class DecryptRequest
    {
        [Required(ErrorMessage = "Введите зашифрованный текст")]
        [StringLength(200_000, ErrorMessage = "Шифртекст слишком длинный.")]
        public string CipherText { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите ключ , которым был зашифрован текст.")]
        public string Key { get; set; } = string.Empty;
    }
}
