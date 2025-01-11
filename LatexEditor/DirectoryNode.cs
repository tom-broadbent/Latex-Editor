using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using DynamicData;
using LatexEditor.ViewModels;
using LatexEditor.Views;
using System;
using System.Collections.ObjectModel;
using System.Threading;

namespace LatexEditor
{
    public class DirectoryNode
    {
        public ObservableCollection<DirectoryNode>? SubNodes { get; }
        public string Title { get; }
        public Uri? Path { get; }

        public DirectoryNode(string title, Uri? path = null)
        {
            Title = title;
            Path = path;
        }

        public DirectoryNode(string title, ObservableCollection<DirectoryNode> subNodes, Uri? path = null)
        {
            Title = title;
            SubNodes = subNodes;
            Path = path;
        }

        public async void OnExpandRequested()
        {
            var window = ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).MainWindow as MainWindow;
            var viewModel = window.DataContext as MainWindowViewModel;
            if (Path != null)
            {
                if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
                desktop.MainWindow?.StorageProvider is not { } provider)
                    throw new NullReferenceException("Missing StorageProvider instance.");

                var folder = await provider.TryGetFolderFromPathAsync(Path);

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