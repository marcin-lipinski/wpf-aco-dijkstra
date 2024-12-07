using SPA.Core;
using SPA.Core.Configuration;
using SPA.Core.Interfaces;
using System.ComponentModel;
using System.Windows;

namespace SPA.App;

public partial class ResultWindow : Window, IResultLogger
{
    private readonly MainWindow _mainWindow;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public ResultWindow(MainWindow mainWindow, AlgorithmsConfig config)
    {
        InitializeComponent();
        _mainWindow = mainWindow;
        _cancellationTokenSource = new CancellationTokenSource();
        RunAlgorithmsAsync(config);
    }

    private async void RunAlgorithmsAsync(AlgorithmsConfig config)
    {
        var program = new Program(this, config);
        await program.RunAsync(_cancellationTokenSource.Token);
        Log("Logging completed. Click 'Return' to go back.");
        ReturnButton.Visibility = Visibility.Visible;
    }

    private void ReturnButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
        _mainWindow.Show();
    }

    public void Log(string message)
    {
        void AppendMessage()
        {
            LogTextBox.AppendText($"{message}\n");
            LogTextBox.ScrollToEnd();
        }

        if (Dispatcher.CheckAccess()) AppendMessage();
        else Dispatcher.Invoke(AppendMessage);
    }

    public void Throw(string message)
    {
        Dispatcher.Invoke(() =>
        {
            MessageBox.Show(this, message);
            _cancellationTokenSource?.Cancel();
            _mainWindow.Show();
            Close();
        });
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        _cancellationTokenSource.Cancel();
        _mainWindow.Show();
        base.OnClosing(e);
    }
}
