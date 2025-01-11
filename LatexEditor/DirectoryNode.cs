using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using LatexEditor.ViewModels;
using LatexEditor.Views;
using System;
using System.Collections.ObjectModel;

namespace LatexEditor
{
    public class DirectoryNode
    {
        public ObservableCollection<DirectoryNode>? SubNodes { get; }
        public string Title { get; }
        public Uri Path { get; }

        public DirectoryNode(string title)
        {
            Title = title;
        }
        public DirectoryNode(string title, Uri path)
        {
            Title = title;
            Path = path;
        }

        public DirectoryNode(string title, ObservableCollection<DirectoryNode> subNodes)
        {
            Title = title;
            SubNodes = subNodes;
        }
    }
}