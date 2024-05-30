using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Windows;

namespace WebClientDownloader
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient httpClient;

        public MainWindow()
        {
            InitializeComponent();
            httpClient = new HttpClient();
        }

        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            var url = UrlBox.Text;

            if (string.IsNullOrWhiteSpace(url))
            {
                StatusListBox.Items.Add("URL cannot be empty.");
                return;
            }

            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult) || (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
            {
                StatusListBox.Items.Add("Invalid URL.");
                return;
            }

            var fileName = Path.GetFileName(url);
            if (string.IsNullOrWhiteSpace(fileName))
            {
                StatusListBox.Items.Add("URL must point to a file.");
                return;
            }

            var destinationPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);

            // Показує, куди файл буде завантажений
            StatusListBox.Items.Add($"Started downloading {fileName} to {destinationPath}");

            try
            {
                await DownloadFileAsync(url, destinationPath);
                StatusListBox.Items.Add("Download completed");
            }
            catch (Exception ex)
            {
                StatusListBox.Items.Add($"Error: {ex.Message}");
            }
        }

        private async Task DownloadFileAsync(string url, string destinationPath)
        {
            var fileBytes = await httpClient.GetByteArrayAsync(url);
            await File.WriteAllBytesAsync(destinationPath, fileBytes);
        }
    }
}