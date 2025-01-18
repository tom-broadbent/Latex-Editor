using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using LatexEditor.ViewModels;
using LatexEditor.Views;

namespace LatexEditor.Views;

public partial class ProjectCreator : Window
{
    public ProjectCreator()
    {
        InitializeComponent();
        DataContext = new ProjectCreatorViewModel();
    }
}