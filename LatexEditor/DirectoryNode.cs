using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using DynamicData;
using LatexEditor.ViewModels;
using LatexEditor.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace LatexEditor
{
	public class DirectoryNode
	{
		public ObservableCollection<DirectoryNode>? SubNodes { get; }
		public string Title { get; set; }
		public Uri? Path { get; }
		public DirectoryNode? Parent { get; }

		public DirectoryNode(string title, Uri? path = null, DirectoryNode? parent = null)
		{
			Title = title;
			Path = path;
			Parent = parent;
		}

		public DirectoryNode(string title, ObservableCollection<DirectoryNode> subNodes, Uri? path = null, DirectoryNode? parent = null)
		{
			Title = title;
			SubNodes = subNodes;
			Path = path;
			Parent = parent;
		}

		public async void OnExpandRequested()
		{
			var window = ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).MainWindow as MainWindow;
			var viewModel = window.DataContext as MainWindowViewModel;
			if (Path != null)
			{				
				var folder = await FsUtils.TryGetFolderFromPathAsync(Path);

				if (folder != null)
				{
					var token = new CancellationToken();
					var loadedFolder = await viewModel.LoadFolder(folder);
					SubNodes.Clear();
					SubNodes.Add(loadedFolder.SubNodes);
				}
			}
		}
	}
}