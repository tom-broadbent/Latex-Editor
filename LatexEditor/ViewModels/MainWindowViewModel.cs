﻿using CommunityToolkit.Mvvm.ComponentModel;
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
using System.Collections.Generic;
using AvaloniaPdfViewer;
using System.Reflection;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

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
	[NotifyCanExecuteChangedFor(nameof(CompileLatexCommand), nameof(SelectFileCommand), nameof(NewProjectCommand))]
	private string? pdfPath;

	public string OriginalText { get; set; } = "";

	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(SelectFileCommand), nameof(NewProjectCommand))]
	private TextDocument document = new TextDocument();

	[ObservableProperty]
	private ObservableCollection<DirectoryNode> fileTree = new ObservableCollection<DirectoryNode>();

	private List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();
	private static MainWindow? window => ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).MainWindow as MainWindow;
	private SymbolPickerViewModel symbolPickerViewModel = new SymbolPickerViewModel();
	private bool multipleOpenFolders = false;

	private string? copiedPath;

	private void UnloadFile()
	{
		Text = "";
		OriginalText = Text;
		OpenFileName = null;
		openFilePath = null;
		window.Title = Constants.ApplicationName;
		window.ChangesMade = false;
		PdfPath = null;
		Document.Text = Text;

	}

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

	private void UnloadAllFolders()
	{
		FileTree.Clear();
		watchers.Clear();
	}

	[RelayCommand]
	private async Task OpenOptionsMenu()
	{
		var optionsMenu = new OptionsMenu()
		{
			Width = 800,
			Height = 300
		};

		await optionsMenu.ShowDialog(window);
	}

    [RelayCommand]
    private async Task OpenMacroMenu()
    {
        var macroMenu = new MacroMenu()
        {
            Width = 900,
            Height = 400
        };

        await macroMenu.ShowDialog(window);
    }

	[RelayCommand]
	private async Task CreateTable()
	{
		var vm = new EnterMultiTextDialogViewModel()
		{
			TextBoxWatermarks = { "Number of columns", "Number of rows" }
		};

        var menu = new EnterMultiTextDialog(vm, 2)
		{
			Title = "Table Dimensions",
			Width = 300,
			Height = 100
		};

		var output = await menu.ShowDialog<List<string>>(window);
		if (output != null)
		{
			int width;
			int height;
			try
			{
				width = int.Parse(output[0]);
			}
			catch
			{
				width = 0;
			}
			try
			{
                height = int.Parse(output[1]);
            }
			catch
			{
				height = 0;
			}

			var latexString = "\\begin{table}[H]\n" +
							  "\\begin{center}\n" +
							  "\\begin{tabular}{|c";
			if (width > 1) {
				for (int i = 0; i < width-1; i++)
				{
					latexString += " | c";
				}
			}
			latexString += "|}\n";
			for (var row = 0; row < height; row++)
			{
				latexString += "	\\hline\n";
				latexString += "	";
				for (var column = 0; column < width; column++)
				{
					latexString += $"cell{column + row * width}";
					if (column < width - 1)
					{
						latexString += " & ";
					}
                }
                latexString += " \\\\";
                latexString += "\n";
            }
			latexString += "	\\hline\n";
            latexString += "\\end{tabular}\n";
            latexString += "\\end{center}\n";
			latexString += "\\caption{Your caption here}\n";
            latexString += "\\end{table}";

            var doc = window.textEditor.Document;
			var offset = window.textEditor.CaretOffset;
			doc.Insert(offset, latexString);
			window.SetChangeMarker();
		}
    }

    [RelayCommand]
	private async Task CompileLatex()
	{
		PdfPath = null;
		var compilingImage = new Bitmap(AssetLoader.Open(new Uri("avares://LatexEditor/Assets/Compiling.png")));
		var imageInfo = typeof(PdfViewer).GetField("MainImage", BindingFlags.NonPublic | BindingFlags.Instance);
		Image? original = imageInfo?.GetValue(window.pdfViewer) as Image;
		if (original != null)
		{
			original.Source = compilingImage;
			imageInfo.SetValue(window.pdfViewer, original);
		}

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

		ProcessStartInfo bibInfo = new ProcessStartInfo
		{
			FileName = window.config?.BibBackend ?? "biber",
			Arguments = $"\"{Path.ChangeExtension(openFilePath, "bcf")}\"",
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			UseShellExecute = false,
			CreateNoWindow = true
		};

		try
		{
			await Task.Run(() => {
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
                    process.StartInfo = bibInfo;
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
            });
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

		var logpath = Path.ChangeExtension(openFilePath, ".log");
        var logtext = File.ReadAllText(logpath);
		if (logtext != null && logtext.Contains("no output PDF file produced!"))
		{
            var box = MessageBoxManager.GetMessageBoxStandard("Error", $"An error occurred and no PDF file was generated. Check {Path.GetFileName(logpath)} for details", ButtonEnum.Ok, Icon.Error);
			box.ShowAsync();
        }
		

		PdfPath = Path.ChangeExtension(openFilePath, ".pdf");
	}

	private void SetNewFileText(string text = "")
	{
		OpenFileName = null;
		openFilePath = null;
		window.Title = Constants.ApplicationName;
		window.ChangesMade = false;
		Text = text;
		OriginalText = Text;
		PdfPath = null;
		Document.Text = Text;
	}

	[RelayCommand]
	private async Task NewProject()
	{
		if (window.ChangesMade == true)
		{
			var confirm = MessageBoxManager.GetMessageBoxStandard(
				"Confirm",
				"You have unsaved changes in the editor. Are you sure you want to create a new project? Unsaved changes will be lost.",
				ButtonEnum.YesNo
			);
			var result = await confirm.ShowWindowDialogAsync(window);
			if (result == ButtonResult.No) return;
		}

		var newProjectMenu = new ProjectCreator()
		{
			Width = 400,
			Height = 225
		};
		//(var newFileText, var newProjectPath) = await newProjectMenu.ShowDialog<(string, string)>(window);
		//if (newFileText != null && newProjectPath != null)
		//{
		//    SetNewFileText(newFileText);
		//}
		var newProjectPath = await newProjectMenu.ShowDialog<string>(window);
		if (newProjectPath != null)
		{
			var dir = await FsUtils.TryGetFolderFromPathAsync(newProjectPath);
			if (dir != null)
			{
				var box = MessageBoxManager.GetMessageBoxStandard("New Project Created", $"New project created at {newProjectPath}", ButtonEnum.Ok);
				await box.ShowAsync();
				await OpenFolder(dir);
				UnloadFile();
			}
		}
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
				var result = await confirm.ShowWindowDialogAsync(window);
				if (result == ButtonResult.No) return;
			}
			var file = await FsUtils.DoOpenFilePickerAsync();
			if (file is null) return;

			await OpenFile(file, token);
			UnloadAllFolders();
		}
		catch (Exception e)
		{
			var box = MessageBoxManager.GetMessageBoxStandard("Error", e.ToString(), ButtonEnum.Ok, Icon.Error);
			box.ShowAsync();
		}
	}

	[RelayCommand]
	private async Task InsertImage(CancellationToken token)
	{
		try
		{
			var file = await FsUtils.DoOpenFilePickerAsync();
			if (file is not null)
			{
				var path = file.Path.LocalPath;

				if (OperatingSystem.IsWindows())
				{
					var split = path.Split(Path.DirectorySeparatorChar);
					path = string.Join('/', split);
				}

				var text = "\\begin{figure}[H]\n" +
							"	\\centering\n" +
							$"	\\includegraphics{{{path}}}\n" +
							"	\\caption{Your caption here}\n" +
							"\\end{figure}";

				var doc = window.textEditor.Document;
				var offset = window.textEditor.CaretOffset;
				doc.Insert(offset, text);
				window.SetChangeMarker();
			}
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

	private async Task FileTreeLoad(IStorageFolder folder, bool clearTree=true)
	{
		var folderNode = await LoadFolder(folder);
		var ftList = new List<DirectoryNode>();
		if (clearTree)
		{
			FileTree.Clear();
		}
		foreach (var item in FileTree)
		{
			if (item.Path != folderNode.Path)
			{
				ftList.Add(item);
			}
			else
			{
				ftList.Add(folderNode);
			}
		}
		if (!ftList.Contains(folderNode))
		{
			ftList.Add(folderNode);
		}
		FileTree = null;
		FileTree = new ObservableCollection<DirectoryNode>(ftList);
		await Dispatcher.UIThread.InvokeAsync(() =>
		{
			var descendants = window.fileTreeView.GetLogicalDescendants().OfType<TreeViewItem>();
			descendants.ToList().ForEach(item => item.IsExpanded = true);

			var treeViewContextMenu = new TreeViewContextMenuBuilder(this);
			var contextMenu = treeViewContextMenu.ContextMenu;
			foreach (var descendant in descendants)
			{
				descendant.ContextMenu = contextMenu;
			}
		});
	}

	private async Task OpenFolder(IStorageFolder folder, bool closeOpenFolders=true)
	{
		if (folder is null) return;

		if (closeOpenFolders)
		{
			UnloadAllFolders();
		}

		multipleOpenFolders = !closeOpenFolders;
		async void fileSystemEvent(object? sender, FileSystemEventArgs e)
		{
			if (Directory.Exists(folder.Path.LocalPath))
			{
				await FileTreeLoad(folder, !multipleOpenFolders);
			}
			else
			{
				UnloadAllFolders();
			}
		}

		var watcher = new FileSystemWatcher(folder.Path.LocalPath);

		watcher.NotifyFilter = NotifyFilters.Attributes
							 | NotifyFilters.CreationTime
							 | NotifyFilters.DirectoryName
							 | NotifyFilters.FileName
							 | NotifyFilters.LastAccess
							 | NotifyFilters.LastWrite
							 | NotifyFilters.Security
							 | NotifyFilters.Size;

		watcher.Created += fileSystemEvent;
		watcher.Deleted += fileSystemEvent;
		watcher.Renamed += fileSystemEvent;

		watcher.IncludeSubdirectories = true;
		watcher.EnableRaisingEvents = true;

		watchers.Add(watcher);

		await FileTreeLoad(folder, closeOpenFolders);
		SetNewFileText();
	}

	private async Task PickFolder(CancellationToken token, bool closeOpenFolders = true)
	{
		try
		{
			if (window.ChangesMade && closeOpenFolders)
			{
				var confirm = MessageBoxManager.GetMessageBoxStandard(
					"Confirm",
					"You have unsaved changes in the editor. Are you sure you want to open a folder? Unsaved changes will be lost.",
					ButtonEnum.YesNo
				);
				var result = await confirm.ShowWindowDialogAsync(window);
				if (result == ButtonResult.No) return;
			}

			var folder = await FsUtils.DoOpenFolderPickerAsync();
			await OpenFolder(folder, closeOpenFolders);
		}
		catch (Exception e)
		{
			var box = MessageBoxManager.GetMessageBoxStandard("Error", e.ToString(), ButtonEnum.Ok, Icon.Error);
			box.ShowAsync();
		}
	}

	[RelayCommand]
	private async Task PickFolderClearTree(CancellationToken token)
	{
		await PickFolder(token, true);
	}

	[RelayCommand]
	private async Task PickFolderAddToTree(CancellationToken token)
	{
		await PickFolder(token, false);
	}
	

	[RelayCommand]
	private async Task SaveAsFile()
	{
		try
		{
			var file = await FsUtils.DoSaveFilePickerAsync();
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
		catch (Exception e)
		{
			var box = MessageBoxManager.GetMessageBoxStandard("Error", e.ToString(), ButtonEnum.Ok, Icon.Error);
			box.ShowAsync();
		}
	}

	[RelayCommand]
	private async Task NewFileDialog()
	{
		try
		{
			var dialogViewModel = new EnterTextDialogViewModel()
			{
				TextBoxWatermark = "File name"
			};
			var dialog = new EnterTextDialog()
			{
				Width = 300,
				Height = 64,
				Title = "Create new file",
				DataContext = dialogViewModel
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
						if (!Path.Exists(path))
						{
							File.Create(path).Close();
							newNode = new DirectoryNode(filename, new Uri(path), selected.Parent);
							selected.Parent.SubNodes.Add(newNode);
						}
						else
						{
							throw new Exception($"{path} already exists");
						}
					}

					else
					{
						var path = Path.Join(selected.Path.LocalPath, filename);
						if (!Path.Exists(path))
						{
							File.Create(path).Close();
							newNode = new DirectoryNode(filename, new Uri(path), selected);
							selected.SubNodes.Add(newNode);
						}
                        else
                        {
                            throw new Exception($"{path} already exists");
                        }
                    }
				}
			}
		}
        catch (Exception e)
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Error", e.ToString(), ButtonEnum.Ok, Icon.Error);
            box.ShowAsync();
        }
    }

	[RelayCommand]
	private async Task NewFolderDialog()
	{
		try
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
		catch (Exception e)
		{
			var box = MessageBoxManager.GetMessageBoxStandard("Error", e.ToString(), ButtonEnum.Ok, Icon.Error);
			box.ShowAsync();
		}
	}

	[RelayCommand]
	private async void FileTreeCopy()
	{
		try
		{
            var selected = window.fileTreeView.SelectedItem as DirectoryNode;
			if (selected != null)
			{
                copiedPath = selected.Path.LocalPath;
            }
        }
        catch (Exception e)
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Error", e.ToString(), ButtonEnum.Ok, Icon.Error);
            box.ShowAsync();
        }
    }

    [RelayCommand]
    private async void FileTreePaste()
    {
        try
        {
            var selected = window.fileTreeView.SelectedItem as DirectoryNode;
            if (selected != null && copiedPath != null)
            {
				var filename = Path.GetFileName(copiedPath);
                DirectoryNode newNode = null;

                if (selected.SubNodes is null)
                {
                    var path = Path.Join(selected.Parent.Path.LocalPath, filename);
					while (Path.Exists(path))
					{
						filename = "Copy of " + filename;
                        path = Path.Join(selected.Parent.Path.LocalPath, filename);
                    }
					var attr = File.GetAttributes(copiedPath);
					if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
					{
						FsUtils.CopyDirectory(copiedPath, path, true);
					}
					else
					{
                        File.Copy(copiedPath, path, true);
                    }
                    newNode = new DirectoryNode(filename, new Uri(path), selected.Parent);
                    selected.Parent.SubNodes.Add(newNode);
                }

                else
                {
                    var path = Path.Join(selected.Path.LocalPath, filename);
                    while (Path.Exists(path))
                    {
                        filename = "Copy of " + filename;
                        path = Path.Join(selected.Path.LocalPath, filename);
                    }
                    var attr = File.GetAttributes(copiedPath);
                    if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        FsUtils.CopyDirectory(copiedPath, path, true);
                    }
                    else
                    {
                        File.Copy(copiedPath, path, true);
                    }
                    newNode = new DirectoryNode(filename, new Uri(path), selected);
                    selected.SubNodes.Add(newNode);
                }
            }
        }
        catch (Exception e)
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Error", e.ToString(), ButtonEnum.Ok, Icon.Error);
            box.ShowAsync();
        }
    }

    [RelayCommand]
	private async void FileTreeDelete()
	{
		try
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
		catch (Exception e)
		{
			var box = MessageBoxManager.GetMessageBoxStandard("Error", e.ToString(), ButtonEnum.Ok, Icon.Error);
			box.ShowAsync();
		}
	}

	[RelayCommand]
	private async void FileTreeRename()
	{
		try
		{
			var selected = window.fileTreeView.SelectedItem as DirectoryNode;

			if (selected != null)
			{
				var dialogViewModel = new EnterTextDialogViewModel()
				{
					TextBoxWatermark = "New name",
					TextBoxDefaultText = Path.GetFileName(selected.Path.LocalPath)
				};
				var renameDialog = new EnterTextDialog()
				{
					Width = 300,
					Height = 64,
					Title = "Rename",
					DataContext = dialogViewModel
				};

				var result = await renameDialog.ShowDialog<string>(window);
				if (result != null)
				{
					var path = selected.Path.LocalPath;

					var confirm = MessageBoxManager.GetMessageBoxStandard(
						"Confirm",
						"You have unsaved changes in the editor. In order to rename this file, the file in the editor must be closed.\nDo you want to continue? Your changes will be lost.",
						ButtonEnum.YesNo
					);

					if (selected.SubNodes == null) // if it's a file
					{
						if (selected.Path.LocalPath == openFilePath) // close open file if it's the one being renamed
						{
							if (window.ChangesMade)
							{
								var confirmResult = await confirm.ShowWindowDialogAsync(window);
								if (confirmResult == ButtonResult.No) return;
							}

							UnloadFile();
						}
						var newPath = Path.Join(Path.GetDirectoryName(path), result);

                        if (!Path.Exists(newPath))
						{
							File.Move(path, newPath);
						}
						else
						{
							throw new Exception($"{newPath} already exists");
						}
					}
					else // if it's a directory
					{
						if (selected.SearchDescendants(x => x.Path.LocalPath == openFilePath) != null) // close open file if it is contained in the folder being renamed
						{
							if (window.ChangesMade)
							{
								var confirmResult = await confirm.ShowWindowDialogAsync(window);
								if (confirmResult == ButtonResult.No) return;
							}

							UnloadFile();
						}
						var newPath = Path.Join(Path.GetDirectoryName(path), result);

                        if (!Path.Exists(newPath))
                        {
                            Directory.Move(path, newPath);

                            if (selected.Parent == null) // re-open the directory if it is the open one
                            {
                                var folder = await FsUtils.TryGetFolderFromPathAsync(newPath);
                                if (folder != null)
                                {
                                    UnloadFile();
                                    FileTree.Remove(selected);
                                    await OpenFolder(folder, false);
                                }
                            }
                        }
                        else
                        {
                            throw new Exception($"{newPath} already exists");
                        }
					}
				}
			}
		}
		catch (Exception e)
		{
			var box = MessageBoxManager.GetMessageBoxStandard("Error", e.ToString(), ButtonEnum.Ok, Icon.Error);
			box.ShowAsync();
		}
	}

	[RelayCommand]
	private async Task PickSymbol()
	{
		try
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
		catch (Exception e)
		{
			var box = MessageBoxManager.GetMessageBoxStandard("Error", e.ToString(), ButtonEnum.Ok, Icon.Error);
			box.ShowAsync();
		}
	}
#pragma warning restore CA1822 // Mark members as static
}
