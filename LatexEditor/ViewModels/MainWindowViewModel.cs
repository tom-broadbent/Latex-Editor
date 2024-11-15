using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Avalonia;
using LatexEditor.Views;
using System.Text;

namespace LatexEditor.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
#pragma warning disable CA1822 // Mark members as static
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(GetTextCommand))]
    private string text = "";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenFileCommand))]
    private string? openFileName;

    [ObservableProperty]
    private string applicationName = Constants.ApplicationName;

    private Uri? openFilePath;

    public string LatexText { get; set; } = "";

    [RelayCommand]
    private void GetText()
    {
        LatexText = Text;
    }

    [RelayCommand]
    private void SetText()
    {
        Text = LatexText;
    }

    [RelayCommand]
    private async Task OpenFile(CancellationToken token)
    {
        try
        {
            var file = await DoOpenFilePickerAsync();
            if (file is null) return;

            // Limit the text file to 1MB so that the demo won't lag.
            if ((await file.GetBasicPropertiesAsync()).Size <= 1024 * 1024 * 1)
            {
                await using var readStream = await file.OpenReadAsync();
                using var reader = new StreamReader(readStream);
                Text = await reader.ReadToEndAsync(token);
                OpenFileName = file.Name;
                openFilePath = file.Path;
                var window = ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).MainWindow;
                window.Title = Constants.ApplicationName + " - " + OpenFileName;
            }
            else
            {
                throw new Exception("File exceeded 1MB limit.");
            }
        }
        catch (Exception e)
        {
            throw;
        }
    }

    [RelayCommand]
    private async Task SaveAsFile()
    {
        try
        {
            var file = await DoSaveFilePickerAsync();
            if (file is null) return;

            // Limit the text file to 1MB so that the demo won't lag.
            if (Text?.Length <= 1024 * 1024 * 1)
            {
                var stream = new MemoryStream(Encoding.Default.GetBytes(Text));
                await using var writeStream = await file.OpenWriteAsync();
                await stream.CopyToAsync(writeStream);
                OpenFileName = file.Name;
                openFilePath = file.Path;
                var window = ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).MainWindow;
                window.Title = Constants.ApplicationName + " - " + OpenFileName;
            }
            else
            {
                throw new Exception("File exceeded 1MB limit.");
            }
        }
        catch (Exception e)
        {
            throw;
        }
    }

    private async Task<IStorageFile?> DoOpenFilePickerAsync()
    {
        // For learning purposes, we opted to directly get the reference
        // for StorageProvider APIs here inside the ViewModel. 

        // For your real-world apps, you should follow the MVVM principles
        // by making service classes and locating them with DI/IoC.

        // See IoCFileOps project for an example of how to accomplish this.
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
            desktop.MainWindow?.StorageProvider is not { } provider)
            throw new NullReferenceException("Missing StorageProvider instance.");

        var files = await provider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = "Open Text File",
            AllowMultiple = false
        });

        return files?.Count >= 1 ? files[0] : null;
    }

    private async Task<IStorageFile?> DoSaveFilePickerAsync()
    {
        // For learning purposes, we opted to directly get the reference
        // for StorageProvider APIs here inside the ViewModel. 

        // For your real-world apps, you should follow the MVVM principles
        // by making service classes and locating them with DI/IoC.

        // See DepInject project for a sample of how to accomplish this.
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
            desktop.MainWindow?.StorageProvider is not { } provider)
            throw new NullReferenceException("Missing StorageProvider instance.");

        return await provider.SaveFilePickerAsync(new FilePickerSaveOptions()
        {
            Title = "Save As"
        });
    }
#pragma warning restore CA1822 // Mark members as static
}
