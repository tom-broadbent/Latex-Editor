using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaEdit;
using AvaloniaEdit.CodeCompletion;
using AvaloniaEdit.TextMate;
using LatexEditor.ViewModels;
using TextMateSharp.Grammars;

namespace LatexEditor.Views;

public partial class MainWindow : Window
{
    public bool ChangesMade = false;
    private CompletionWindow completionWindow;
    private OverloadInsightWindow insightWindow;

    public MainWindow()
    {
        InitializeComponent();

        // set theming and syntax highlighting
        var registryOptions = new RegistryOptions(ThemeName.DarkPlus);
        var textMateInstallation = textEditor.InstallTextMate(registryOptions);
        textMateInstallation.SetGrammar(registryOptions.GetScopeByLanguageId(registryOptions.GetLanguageByExtension(".tex").Id));
        textEditor.TextArea.TextEntered += OnTextEntered;
    }

    private void Binding(object? sender, Avalonia.Input.KeyEventArgs e)
    {
    }

    private void OnTextEntered(object? sender, Avalonia.Input.TextInputEventArgs e)
    {
        var viewModel = DataContext as MainWindowViewModel;
        viewModel.Text = viewModel.Document.Text;
        if(!ChangesMade && viewModel.Text != viewModel.OriginalText)
        {
            ChangesMade = true;
            this.Title += " *";
        }
        /*ShowCompletion();
        ShowInsight();*/
    }

    private void ShowCompletion()
    {
        completionWindow = new CompletionWindow(textEditor.TextArea);
        completionWindow.Closed += (_, _) => completionWindow = null;

        var data = completionWindow.CompletionList.CompletionData;
        data.Add(new LatexCompletionData("Item1"));
        data.Add(new LatexCompletionData("Item2"));
        data.Add(new LatexCompletionData("Item3"));

        completionWindow.Show();
    }

    private void ShowInsight()
    {
        insightWindow = new OverloadInsightWindow(textEditor.TextArea);
        insightWindow.Closed += (o, args) => insightWindow = null;

        insightWindow.Provider = new LatexOverloadProvider(new[]
        {
            ("Method1(int, string)", "Method1 description"),
            ("Method2(int)", "Method2 description"),
            ("Method3(string)", "Method3 description"),
        });

        insightWindow.Show();
    }
}