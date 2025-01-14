using Avalonia.Controls;
using LatexEditor.ViewModels;

namespace LatexEditor
{
    internal class TreeViewContextMenuBuilder
    {
        private MainWindowViewModel _viewModel;
        internal ContextMenu ContextMenu { get; set; }
        internal TreeViewContextMenuBuilder(MainWindowViewModel viewModel)
        {
            _viewModel = viewModel;
            ContextMenu = new ContextMenu()
            {
                Items =
                {
                    new MenuItem()
                    {
                        Header = "New File",
                        Command = _viewModel.NewFileDialogCommand
                    },
                    new MenuItem()
                    {
                        Header = "New Folder",
                        Command = _viewModel.NewFolderDialogCommand
                    },
                    new MenuItem()
                    {
                        Header = "Delete",
                        Command = _viewModel.FileTreeDeleteCommand
                    }
                }
            };
    }
        
    }
}
