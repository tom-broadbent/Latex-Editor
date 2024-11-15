using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using LatexEditor.ViewModels;

namespace LatexEditor.Views;

public partial class MainWindow : Window
{
    public bool ChangesMade = false;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Binding(object? sender, Avalonia.Input.KeyEventArgs e)
    {
    }

    private void OnMakeChange(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        var viewModel = DataContext as MainWindowViewModel;
        if(!ChangesMade && viewModel.Text != viewModel.OriginalText)
        {
            ChangesMade = true;
            this.Title += " *";
        }
    }
}