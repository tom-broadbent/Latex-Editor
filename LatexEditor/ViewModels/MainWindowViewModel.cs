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
using System.Diagnostics;
using AvaloniaEdit.Document;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;

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

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CompileLatexCommand), nameof(OpenFileCommand), nameof(NewFileCommand))]
    private string? pdfPath;

    public string OriginalText { get; set; } = "";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenFileCommand), nameof(NewFileCommand))]
    private TextDocument document = new TextDocument();

    [RelayCommand]
    private async Task CompileLatex()
    {
        PdfPath = null;

        if (openFilePath == null)
        {
           await SaveAsFile();
        }

        var window = ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).MainWindow as MainWindow;
        if (window.ChangesMade)
        {
            await SaveFile();
        }

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "pdflatex",
            Arguments = $"-interaction=nonstopmode -output-directory=\"{Path.GetDirectoryName(openFilePath)}\" \"{openFilePath}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        try
        {
            using (Process process = new Process())
            {
                process.StartInfo = startInfo;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                string errors = process.StandardError.ReadToEnd();

                process.WaitForExit();
            }
        }
        catch (Exception e)
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Error", e.ToString(), ButtonEnum.Ok, Icon.Error);
            if (e.Message.Contains("pdflatex"))
            {
                box = MessageBoxManager.GetMessageBoxStandard("Error", "Could not find any TeX distribution installed. Make sure that a TeX distribution such as MiKTeX or TeX Live is installed, and that it has been added to your PATH", ButtonEnum.Ok, Icon.Error);
            }
            box.ShowAsync();
        }
        

        PdfPath = Path.ChangeExtension(openFilePath, ".pdf");
    }

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
        PdfPath = null;
        Document.Text = Text;
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
            PdfPath = Path.ChangeExtension(openFilePath, ".pdf");
            Document.Text = Text;
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
