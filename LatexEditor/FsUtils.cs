using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

namespace LatexEditor;

public static class FsUtils
{
	public static async Task<IStorageFile?> DoOpenFilePickerAsync()
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

	public static async Task<IStorageFolder?> DoOpenFolderPickerAsync()
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
	
	public static async Task<IStorageFolder?> TryGetFolderFromPathAsync(string path)
	{
		if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
			desktop.MainWindow?.StorageProvider is not { } provider)
			throw new NullReferenceException("Missing StorageProvider instance.");
			
		var folder = await provider.TryGetFolderFromPathAsync(path);
		return folder;
	}
	
	public static async Task<IStorageFolder?> TryGetFolderFromPathAsync(Uri uriPath)
	{
		if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
			desktop.MainWindow?.StorageProvider is not { } provider)
			throw new NullReferenceException("Missing StorageProvider instance.");
			
		var folder = await provider.TryGetFolderFromPathAsync(uriPath);
		return folder;
	}
	
	public static async Task<IStorageFile?> TryGetFileFromPathAsync(string path)
	{
		if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
			desktop.MainWindow?.StorageProvider is not { } provider)
			throw new NullReferenceException("Missing StorageProvider instance.");
			
		var file = await provider.TryGetFileFromPathAsync(path);
		return file;
	}
	
	public static async Task<IStorageFile?> TryGetFileFromPathAsync(Uri uriPath)
	{
		if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
			desktop.MainWindow?.StorageProvider is not { } provider)
			throw new NullReferenceException("Missing StorageProvider instance.");
			
		var file = await provider.TryGetFileFromPathAsync(uriPath);
		return file;
	}

	public static async Task<IStorageFile?> DoSaveFilePickerAsync()
	{
		if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
			desktop.MainWindow?.StorageProvider is not { } provider)
			throw new NullReferenceException("Missing StorageProvider instance.");

		return await provider.SaveFilePickerAsync(new FilePickerSaveOptions()
		{
			Title = "Save As"
		});
	}
	
	public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
	{
		// Get information about the source directory
		var dir = new DirectoryInfo(sourceDir);

		// Check if the source directory exists
		if (!dir.Exists)
			throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

		// Cache directories before we start copying
		DirectoryInfo[] dirs = dir.GetDirectories();

		// Create the destination directory
		Directory.CreateDirectory(destinationDir);

		// Get the files in the source directory and copy to the destination directory
		foreach (FileInfo file in dir.GetFiles())
		{
			string targetFilePath = Path.Combine(destinationDir, file.Name);
			file.CopyTo(targetFilePath);
		}

		// If recursive and copying subdirectories, recursively call this method
		if (recursive)
		{
			foreach (DirectoryInfo subDir in dirs)
			{
				string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
				CopyDirectory(subDir.FullName, newDestinationDir, true);
			}
		}
	}

}