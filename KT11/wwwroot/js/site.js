
document.addEventListener('click', async (event) => {
  const button = event.target.closest('[data-copy]');
  if (!button) return;

  const source = document.getElementById(button.dataset.copy);
  if (!source) return;

  const originalText = button.textContent;
  try {
    await navigator.clipboard.writeText(source.textContent.trim());
    button.textContent = 'Скопировано';
  } catch {
    const range = document.createRange();
    range.selectNodeContents(source);
    const selection = window.getSelection();
    selection.removeAllRanges();
    selection.addRange(range);
    button.textContent = 'Выделено — Ctrl+C';
  }
  setTimeout(() => (button.textContent = originalText), 1500);
});
