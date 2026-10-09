using System.ComponentModel.DataAnnotations;

namespace KT11.Models
{
    public class EncryptRequest
    {
        [Required(ErrorMessage = "Введите текст для шифрования.")]
        [StringLength(100_000, ErrorMessage = "Текст не должен превышать 100 000 символов.")]
        public string PlainText { get; set; } = string.Empty;

        public string? Key { get; set; }
    }
}
