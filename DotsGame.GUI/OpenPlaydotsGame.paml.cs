using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Threading;

namespace DotsGame.GUI
{
    public class OpenPlaydotsGame : Window
    {
        TextBox _textBox;
        Timer _clipboardTimer;

        public OpenPlaydotsGame()
        {
            this.InitializeComponent();
            App.AttachDevTools(this);
            DataContext = this;

            _textBox = this.Find<TextBox>("InputTextBox");
            var okButton = this.Find<Button>("OkButton");
            var cancelButton = this.Find<Button>("CancelButton");

            _clipboardTimer = new Timer(ClipboardUpdateEvent, null, 0, Timeout.Infinite);

            okButton.Click += (sender, e) =>
            {
                _clipboardTimer.Dispose();
                Close(_textBox.Text);
            };
            cancelButton.Click += (sender, e) =>
            {
                _clipboardTimer.Dispose();
                Close(null);
            };
        }

        private async void ClipboardUpdateEvent(object state)
        {
            string clipboardText = await Avalonia.Application.Current.Clipboard.GetTextAsync();
            if (clipboardText != null && clipboardText.Contains("game.playdots.ru") && clipboardText != _textBox.Text)
            {
                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => _textBox.Text = clipboardText);
            }
            _clipboardTimer.Change(250, Timeout.Infinite);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
