using KT11.Models;
using KT11.Services;
using Microsoft.AspNetCore.Mvc;

namespace KT11.Controllers
{
    /// <summary>
    /// Общая логика страниц шифрования: форма «Зашифровать» и форма «Расшифровать».
    /// Конкретная библиотека определяется переданным сервисом.
    /// </summary>
    [AutoValidateAntiforgeryToken]
    public abstract class CryptoControllerBase : Controller
    {
        private readonly ITextCryptoService _service;
        private readonly string _title;
        private readonly string _algorithm;
        private readonly string _formatNote;

        protected CryptoControllerBase(ITextCryptoService service, string title, string algorithm, string formatNote)
        {
            _service = service;
            _title = title;
            _algorithm = algorithm;
            _formatNote = formatNote;
        }

        [HttpGet]
        public IActionResult Index() => View(CreateModel());

        [HttpPost]
        public IActionResult Encrypt([Bind(Prefix = nameof(CryptoViewModel.Encrypt))] EncryptRequest request)
        {
            request ??= new EncryptRequest();
            var model = CreateModel();
            model.Encrypt = request;

            if (!ModelState.IsValid)
            {
                model.EncryptError = CollectErrors();
            }
            else
            {
                try
                {
                    EncryptionResult result = _service.Encrypt(request.PlainText, request.Key);

                    model.EncryptedText = result.CipherText;
                    model.UsedKey = result.Key;
                    model.KeyWasGenerated = string.IsNullOrWhiteSpace(request.Key);

                    // Подставляем результат в форму расшифровки, чтобы можно было сразу проверить.
                    model.Decrypt = new DecryptRequest { CipherText = result.CipherText, Key = result.Key };
                }
                catch (CryptoServiceException ex)
                {
                    model.EncryptError = ex.Message;
                }
            }

            return Render(model);
        }

        [HttpPost]
        public IActionResult Decrypt([Bind(Prefix = nameof(CryptoViewModel.Decrypt))] DecryptRequest request)
        {
            request ??= new DecryptRequest();
            var model = CreateModel();
            model.Decrypt = request;

            if (!ModelState.IsValid)
            {
                model.DecryptError = CollectErrors();
            }
            else
            {
                try
                {
                    model.DecryptedText = _service.Decrypt(request.CipherText, request.Key);
                }
                catch (CryptoServiceException ex)
                {
                    model.DecryptError = ex.Message;
                }
            }

            return Render(model);
        }

        private CryptoViewModel CreateModel() => new()
        {
            Title = _title,
            Algorithm = _algorithm,
            FormatNote = _formatNote
        };

        private string CollectErrors() =>
            string.Join(" ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .Where(m => !string.IsNullOrWhiteSpace(m)));

        private IActionResult Render(CryptoViewModel model)
        {
            // Поля формы берём из модели, а не из ModelState (иначе подставленные значения будут перезаписаны).
            ModelState.Clear();
            return View("Index", model);
        }
    }
}
