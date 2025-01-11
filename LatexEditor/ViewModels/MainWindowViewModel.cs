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
using System.Collections.ObjectModel;
using Avalonia.Controls;
using TextMateSharp.Grammars;

namespace LatexEditor.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
#pragma warning disable CA1822 // Mark members as static
    [ObservableProperty]
    private string text = "";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SelectFileCommand))]
    private string? openFileName;

    [ObservableProperty]
    private string applicationName = Constants.ApplicationName;

    private string? openFilePath;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CompileLatexCommand), nameof(SelectFileCommand), nameof(NewFileCommand))]
    private string? pdfPath;

    public string OriginalText { get; set; } = "";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SelectFileCommand), nameof(NewFileCommand))]
    private TextDocument document = new TextDocument();

    [ObservableProperty]
    private ObservableCollection<DirectoryNode> fileTree = new ObservableCollection<DirectoryNode>();

    internal async Task OpenFile(IStorageFile file, CancellationToken token)
    {
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
        var regOpt = window.TextMate.RegistryOptions as RegistryOptions;
        var language = regOpt.GetLanguageByExtension(Path.GetExtension(openFilePath));
        if (language != null)
        {
            window.TextMate.SetGrammar(regOpt.GetScopeByLanguageId(language.Id));
        }
        else
        {
            window.TextMate.SetGrammar("text.txt");
        }
    }

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

        ProcessStartInfo pdflatexInfo = new ProcessStartInfo
        {
            FileName = "pdflatex",
            Arguments = $"-interaction=nonstopmode -output-directory=\"{Path.GetDirectoryName(openFilePath)}\" \"{openFilePath}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        ProcessStartInfo biberInfo = new ProcessStartInfo
        {
            FileName = "biber",
            Arguments = $"\"{Path.ChangeExtension(openFilePath, "bcf")}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        try
        {
            using (Process process = new Process())
            {
                process.StartInfo = pdflatexInfo;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                string errors = process.StandardError.ReadToEnd();

                process.WaitForExit();
            }

            using (Process process = new Process())
            {
                process.StartInfo = biberInfo;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                string errors = process.StandardError.ReadToEnd();

                process.WaitForExit();
            }

            using (Process process = new Process())
            {
                process.StartInfo = pdflatexInfo;
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
        FileTree.Clear();
    }

    [RelayCommand]
    private async Task SelectFile(CancellationToken token)
    {
        try
        {
            var file = await DoOpenFilePickerAsync();
            if (file is null) return;

            await OpenFile(file, token);
            FileTree.Clear();
        }
        catch (Exception e)
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Error", e.ToString(), ButtonEnum.Ok, Icon.Error);
            box.ShowAsync();
        }
    }

    internal async Task<DirectoryNode> LoadFolder(IStorageFolder folder)
    {
        try
        {
            var items = folder.GetItemsAsync();
            var folderNode = new DirectoryNode(folder.Name, new ObservableCollection<DirectoryNode>(), folder.Path);
            await foreach (var item in items)
            {
                if (item is IStorageFile fileItem)
                {
                    folderNode.SubNodes.Add(new DirectoryNode(fileItem.Name, fileItem.Path));
                }
                else if (item is IStorageFolder folderItem)
                {
                    var subDirNode = new DirectoryNode(folderItem.Name, new ObservableCollection<DirectoryNode>(), folderItem.Path);

                    var items2 = folderItem.GetItemsAsync();
                    await foreach (var item2 in items2)
                    {
                        if (item2 is IStorageFile fileItem2)
                        {
                            subDirNode.SubNodes.Add(new DirectoryNode(fileItem2.Name, fileItem2.Path));
                        }
                        else if (item2 is IStorageFolder folderItem2)
                        {
                            var subSubDirNode = new DirectoryNode(folderItem2.Name, new ObservableCollection<DirectoryNode>(), folderItem2.Path);
                            subDirNode.SubNodes.Add(subSubDirNode);
                        }
                    }

                    folderNode.SubNodes.Add(subDirNode);
                }
            }
            return folderNode;
        }
        catch(UnauthorizedAccessException e)
        {
            return new DirectoryNode(folder.Name);
        }
    }

    [RelayCommand]
    private async Task OpenFolder(CancellationToken token)
    {
        try
        {
            var folder = await DoOpenFolderPickerAsync();
            if (folder is null) return;

            var folderNode = await LoadFolder(folder);
            FileTree.Clear();
            FileTree.Add(folderNode);
        }
        catch(Exception e)
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Error", e.ToString(), ButtonEnum.Ok, Icon.Error);
            box.ShowAsync();
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
            var box = MessageBoxManager.GetMessageBoxStandard("Error", e.ToString(), ButtonEnum.Ok, Icon.Error);
            box.ShowAsync();
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

    private async Task<IStorageFolder?> DoOpenFolderPickerAsync()
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
            desktop.MainWindow?.StorageProvider is not { } provider)
            throw new NullReferenceException("Missing StorageProvider instance.");

        var folder = await provider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
        {
            Title = "Open Directory",
            AllowMultiple = false
        });
        return folder?.Count >= 1 ? folder[0] : null;
    }

    private async Task<IStorageFile?> DoSaveFilePickerAsync()
    {
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
