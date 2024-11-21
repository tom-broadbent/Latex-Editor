using Avalonia.Controls;
using AvaloniaEdit;
using AvaloniaEdit.TextMate;
using LatexEditor.ViewModels;
using TextMateSharp.Grammars;

namespace LatexEditor.Views;

public partial class MainWindow : Window
{
    public bool ChangesMade = false;

    public MainWindow()
    {
        InitializeComponent();

        // set theming and syntax highlighting
        var textEditor = this.FindControl<TextEditor>("textEditor");
        var registryOptions = new RegistryOptions(ThemeName.DarkPlus);
        var textMateInstallation = textEditor.InstallTextMate(registryOptions);
        textMateInstallation.SetGrammar(registryOptions.GetScopeByLanguageId(registryOptions.GetLanguageByExtension(".tex").Id));
    }

    private void Binding(object? sender, Avalonia.Input.KeyEventArgs e)
    {
    }

    private void OnMakeChange(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        var viewModel = DataContext as MainWindowViewModel;
        viewModel.Text = viewModel.Document.Text;
        if(!ChangesMade && viewModel.Text != viewModel.OriginalText)
        {
            ChangesMade = true;
            this.Title += " *";
        }
    }
}