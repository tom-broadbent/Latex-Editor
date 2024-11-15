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
using System.Linq;

namespace LatexEditor.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
#pragma warning disable CA1822 // Mark members as static
    [ObservableProperty]
    private string text = "";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenFileCommand))]
    private string? openFileName;

    [ObservableProperty]
    private string applicationName = Constants.ApplicationName;

    private string? openFilePath;

    public string OriginalText { get; set; } = "";

    [RelayCommand]
    private void NewFile()
    {
        OpenFileName = null;
        openFilePath = null;
        var window = ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).MainWindow as MainWindow;
        window.Title = Constants.ApplicationName;
        window.ChangesMade = false;
        Text = "";
        OriginalText = Text;
    }

    [RelayCommand]
    private async Task OpenFile(CancellationToken token)
    {
        try
        {
            var file = await DoOpenFilePickerAsync();
            if (file is null) return;

            await using var readStream = await file.OpenReadAsync();
            using var reader = new StreamReader(readStream);
            Text = await reader.ReadToEndAsync(token);
            OriginalText = Text;
            OpenFileName = file.Name;
            openFilePath = file.TryGetLocalPath();
            var window = ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).MainWindow as MainWindow;
            window.Title = Constants.ApplicationName + " - " + openFilePath;
            window.ChangesMade = false;
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

            var stream = new MemoryStream(Encoding.Default.GetBytes(Text));
            await using var writeStream = await file.OpenWriteAsync();
            await stream.CopyToAsync(writeStream);
            OpenFileName = file.Name;
            openFilePath = file.TryGetLocalPath();
            var window = ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).MainWindow as MainWindow;
            window.Title = Constants.ApplicationName + " - " + openFilePath;
            window.ChangesMade = false;
            OriginalText = Text;
        }
        catch (Exception e)
        {
            throw;
        }
    }

    [RelayCommand]
    private async Task SaveFile()
    {
        try
        {
            File.WriteAllTextAsync(openFilePath.ToString(), Text);
            var window = ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).MainWindow as MainWindow;
            window.Title = Constants.ApplicationName + " - " + openFilePath;
            window.ChangesMade = false;
            OriginalText = Text;
        }
        catch
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
