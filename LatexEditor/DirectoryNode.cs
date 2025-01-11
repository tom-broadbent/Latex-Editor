using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using LatexEditor.ViewModels;
using LatexEditor.Views;
using System.Collections.ObjectModel;

namespace LatexEditor
{
    public class DirectoryNode
    {
        public ObservableCollection<DirectoryNode>? SubNodes { get; }
        public string Title { get; }
        public IStorageFile File { get; }

        public DirectoryNode(string title)
        {
            Title = title;
        }
        public DirectoryNode(string title, IStorageFile file)
        {
            Title = title;
            File = file;
        }

        public DirectoryNode(string title, ObservableCollection<DirectoryNode> subNodes)
        {
            Title = title;
            SubNodes = subNodes;
        }
    }
}