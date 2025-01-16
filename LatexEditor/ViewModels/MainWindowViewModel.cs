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
using TextMateSharp.Grammars;
using System.Linq;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Models;
using System.Collections.Generic;

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

    private FileSystemWatcher? watcher;
    private static MainWindow? window => ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).MainWindow as MainWindow;
    private SymbolPickerViewModel symbolPickerViewModel = new SymbolPickerViewModel();

    internal async Task OpenFile(IStorageFile file, CancellationToken token)
    {
        await using var readStream = await file.OpenReadAsync();
        using var reader = new StreamReader(readStream);
        Text = await reader.ReadToEndAsync(token);
        OriginalText = Text;
        OpenFileName = file.Name;
        openFilePath = file.TryGetLocalPath();
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

    private void UnloadFolder()
    {
        FileTree.Clear();
        if (watcher != null)
        {
            watcher.Dispose();
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
    private async Task NewFile()
    {
        if (window.ChangesMade == true)
        {
            var confirm = MessageBoxManager.GetMessageBoxStandard(
                "Confirm",
                "You have unsaved changes in the editor. Are you sure you want to create a new document? Unsaved changes will be lost.",
                ButtonEnum.YesNo
            );
            var result = await confirm.ShowAsync();
            if (result == ButtonResult.No) return;
        }
        OpenFileName = null;
        openFilePath = null;
        window.Title = Constants.ApplicationName;
        window.ChangesMade = false;
        Text = "";
        OriginalText = Text;
        PdfPath = null;
        Document.Text = Text;
    }

    [RelayCommand]
    private async Task SelectFile(CancellationToken token)
    {
        try
        {
            if (window.ChangesMade)
            {
                var confirm = MessageBoxManager.GetMessageBoxStandard(
                    "Confirm",
                    "You have unsaved changes in the editor. Are you sure you want to open a different file? Unsaved changes will be lost.",
                    ButtonEnum.YesNo
                );
                var result = await confirm.ShowAsync();
                if (result == ButtonResult.No) return;
            }
            var file = await DoOpenFilePickerAsync();
            if (file is null) return;

            await OpenFile(file, token);
            UnloadFolder();
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
            var folderNode = new DirectoryNode("📁 " + folder.Name, new ObservableCollection<DirectoryNode>(), folder.Path);
            await foreach (var item in items)
            {
                if (item is IStorageFile fileItem)
                {
                    folderNode.SubNodes.Add(new DirectoryNode("📄 " + fileItem.Name, fileItem.Path, folderNode));
                }
                else if (item is IStorageFolder folderItem)
                {
                    var subDirNode = new DirectoryNode("📁 " + folderItem.Name, new ObservableCollection<DirectoryNode>(), folderItem.Path, folderNode);

                    var items2 = folderItem.GetItemsAsync();
                    await foreach (var item2 in items2)
                    {
                        if (item2 is IStorageFile fileItem2)
                        {
                            subDirNode.SubNodes.Add(new DirectoryNode("📄 " + fileItem2.Name, fileItem2.Path, subDirNode));
                        }
                        else if (item2 is IStorageFolder folderItem2)
                        {
                            var subSubDirNode = new DirectoryNode("📁 " + folderItem2.Name, new ObservableCollection<DirectoryNode>(), folderItem2.Path, subDirNode);
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

    private async Task FileTreeLoad(IStorageFolder folder)
    {
        var folderNode = await LoadFolder(folder);
        FileTree = null;
        FileTree = new ObservableCollection<DirectoryNode> { folderNode };  // using Clear() and Add() caused duplicates for some reason
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            var descendants = window.fileTreeView.GetLogicalDescendants().OfType<TreeViewItem>();
            var treeViewItem = descendants.First();
            treeViewItem.IsExpanded = true;

            var treeViewContextMenu = new TreeViewContextMenuBuilder(this);
            var contextMenu = treeViewContextMenu.ContextMenu;
            foreach (var descendant in descendants)
            {
                descendant.ContextMenu = contextMenu;
            }
        });
    }

    [RelayCommand]
    private async Task OpenFolder(CancellationToken token)
    {
        try
        {
            if (window.ChangesMade)
            {
                var confirm = MessageBoxManager.GetMessageBoxStandard(
                    "Confirm",
                    "You have unsaved changes in the editor. Are you sure you want to open a folder? Unsaved changes will be lost.",
                    ButtonEnum.YesNo
                );
                var result = await confirm.ShowAsync();
                if (result == ButtonResult.No) return;
            }

            var folder = await DoOpenFolderPickerAsync();
            if (folder is null) return;

            if (watcher != null)
            {
                watcher.Dispose();
            }

            async void fileSystemEvent(object? sender, FileSystemEventArgs e)
            {
                await FileTreeLoad(folder);
            }

            watcher = new FileSystemWatcher(folder.Path.LocalPath);

            watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;

            watcher.Changed += fileSystemEvent;
            watcher.Created += fileSystemEvent;
            watcher.Deleted += fileSystemEvent;
            watcher.Renamed += fileSystemEvent;

            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;

            await FileTreeLoad(folder);
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
            window.Title = Constants.ApplicationName + " - " + openFilePath;
            window.ChangesMade = false;
            OriginalText = Text;
        }
        catch
        {
            throw;
        }
    }

    [RelayCommand]
    private async Task NewFileDialog()
    {
        var dialogViewModel = new EnterTextDialogViewModel()
        {
            TextBoxWatermark = "File name"
        };
        var dialog = new EnterTextDialog()
        {
            Width=300,
            Height=64,
            Title="Create new file",
            DataContext=dialogViewModel
        };
        var filename = await dialog.ShowDialog<string>(window);

        if (!string.IsNullOrEmpty(filename))
        {
            var selected = window.fileTreeView.SelectedItem as DirectoryNode;
            if (selected != null)
            {
                DirectoryNode newNode = null;
                if (selected.SubNodes is null)
                {
                    var path = Path.Join(selected.Parent.Path.LocalPath, filename);
                    File.Create(path).Close();
                    newNode = new DirectoryNode(filename, new Uri(path), selected.Parent);
                    selected.Parent.SubNodes.Add(newNode);
                }

                else
                {
                    var path = Path.Join(selected.Path.LocalPath, filename);
                    File.Create(path).Close();
                    newNode = new DirectoryNode(filename, new Uri(path), selected);
                    selected.SubNodes.Add(newNode);
                }
            }
        }
    }

    [RelayCommand]
    private async Task NewFolderDialog()
    {
        var dialogViewModel = new EnterTextDialogViewModel()
        {
            TextBoxWatermark = "Folder name"
        };
        var dialog = new EnterTextDialog()
        {
            Width = 300,
            Height = 64,
            Title = "Create new folder",
            DataContext = dialogViewModel
        };
        var folderName = await dialog.ShowDialog<string>(window);

        if (!string.IsNullOrEmpty(folderName))
        {
            var selected = window.fileTreeView.SelectedItem as DirectoryNode;
            if (selected != null)
            {
                DirectoryNode newNode = null;
                if (selected.SubNodes is null)
                {
                    var path = Path.Join(selected.Parent.Path.LocalPath, folderName);
                    Directory.CreateDirectory(path);
                    newNode = new DirectoryNode(folderName, new Uri(path), selected.Parent);
                    selected.Parent.SubNodes.Add(newNode);
                }

                else
                {
                    var path = Path.Join(selected.Path.LocalPath, folderName);
                    Directory.CreateDirectory(path);
                    newNode = new DirectoryNode(folderName, new Uri(path), selected);
                    selected.SubNodes.Add(newNode);
                }
            }
        }
    }

    [RelayCommand]
    private async void FileTreeDelete()
    {
        var selected = window.fileTreeView.SelectedItem as DirectoryNode;

        if (selected != null)
        {
            var box = MessageBoxManager.GetMessageBoxStandard(
                    "Confirm",
                    $"Are you sure you want to delete {selected.Title}? This action cannot be undone.",
                    ButtonEnum.YesNo);

            var path = selected.Path.LocalPath;
            if (selected.SubNodes is null)
            {
                var result = await box.ShowAsync();
                if (result == ButtonResult.Yes) File.Delete(path);
            }

            else
            {
                var dir = new DirectoryInfo(path);

                void removeReadonly(DirectoryInfo dir)
                {
                    dir.Attributes &= ~FileAttributes.ReadOnly;
                    foreach (var subDir in dir.GetDirectories())
                    {
                        removeReadonly(subDir);
                    }
                    foreach (var file in dir.GetFiles())
                    {
                        file.Attributes &= ~FileAttributes.ReadOnly;
                    }
                }

                if (dir.Exists)
                {
                    var result = await box.ShowAsync();
                    if (result == ButtonResult.Yes)
                    {
                        removeReadonly(dir);
                        dir.Delete(true);
                    }
                }
            }
        }

        selected.Parent.SubNodes.Remove(selected);
    }

    [RelayCommand]
    private async Task PickSymbol()
    {
        var symbolPicker = new SymbolPicker(symbolPickerViewModel);
        var symbol = await symbolPicker.ShowDialog<string>(window);
        if (symbol != null)
        {
            var doc = window.textEditor.Document;
            var offset = window.textEditor.CaretOffset;
            doc.Insert(offset, symbol);
            window.SetChangeMarker();
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
            Title = "Open Folder",
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
