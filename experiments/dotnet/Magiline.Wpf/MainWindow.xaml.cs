using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using Magiline.Protocol;

namespace Magiline.Wpf;

public partial class MainWindow : Window
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public MainWindow()
    {
        InitializeComponent();
    }

    private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        => await ExecuteAsync("Lecture de l'état", async client =>
        {
            using var state = await client.GetPoolStateAsync();
            JsonTextBox.Text = JsonSerializer.Serialize(state.RootElement, JsonOptions);
        });

    private async void SpotlightOn_Click(object sender, RoutedEventArgs e)
        => await SendCommandAndRefreshAsync("Projecteur ON", client => client.SendSpotlightAsync(true));

    private async void SpotlightOff_Click(object sender, RoutedEventArgs e)
        => await SendCommandAndRefreshAsync("Projecteur OFF", client => client.SendSpotlightAsync(false));

    private async void FiltrationButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button || !int.TryParse(button.Tag?.ToString(), out var wanted))
            return;

        if (wanted >= 4)
        {
            var answer = MessageBox.Show(
                $"La fonction de wanted={wanted} est inconnue. Envoyer tout de même la commande ?",
                "Commande expérimentale",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (answer != MessageBoxResult.Yes)
                return;
        }

        await SendCommandAndRefreshAsync(
            $"Filtration wanted={wanted}",
            client => client.SendFiltrationModeAsync(wanted));
    }

    private async Task SendCommandAndRefreshAsync(string action, Func<MagilineClient, Task> command)
        => await ExecuteAsync(action, async client =>
        {
            await command(client);
            await Task.Delay(1200);
            using var state = await client.GetPoolStateAsync();
            JsonTextBox.Text = JsonSerializer.Serialize(state.RootElement, JsonOptions);
        });

    private async Task ExecuteAsync(string action, Func<MagilineClient, Task> operation)
    {
        try
        {
            SetBusy(true);
            using var client = CreateClient();
            await WriteLogAsync($"START {action}");
            await operation(client);
            ConnectionStatusText.Text = "Connecté";
            LastActionText.Text = $"{DateTime.Now:HH:mm:ss} — {action} terminé.";
            await WriteLogAsync($"OK {action}");
        }
        catch (Exception ex)
        {
            ConnectionStatusText.Text = "Erreur";
            LastActionText.Text = $"{DateTime.Now:HH:mm:ss} — {action} en échec : {ex.Message}";
            await WriteLogAsync($"ERROR {action}: {ex}");
            MessageBox.Show(ex.Message, "Erreur Magiline", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private MagilineClient CreateClient()
    {
        if (!int.TryParse(PortTextBox.Text, out var port))
            throw new InvalidOperationException("Le port doit être un nombre valide.");

        return new MagilineClient(HostTextBox.Text.Trim(), port);
    }

    private void SetBusy(bool busy)
    {
        IsEnabled = !busy;
        if (busy)
            ConnectionStatusText.Text = "Traitement…";
    }

    private static async Task WriteLogAsync(string message)
    {
        var logDirectory = Path.Combine(AppContext.BaseDirectory, "logs");
        Directory.CreateDirectory(logDirectory);
        var path = Path.Combine(logDirectory, $"magiline-{DateTime.Now:yyyy-MM-dd}.log");
        await File.AppendAllTextAsync(path, $"{DateTime.Now:O} {message}{Environment.NewLine}");
    }
}
