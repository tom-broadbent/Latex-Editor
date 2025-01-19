using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LatexEditor.ViewModels;

namespace LatexEditor.Views;

public partial class TemplateSelector : Window
{
    public TemplateSelector()
    {
        InitializeComponent();
        DataContext = new TemplateSelectorViewModel();
    }
}