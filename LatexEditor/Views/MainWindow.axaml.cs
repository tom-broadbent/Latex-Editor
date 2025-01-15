using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using AvaloniaEdit.CodeCompletion;
using AvaloniaEdit.TextMate;
using LatexEditor.ViewModels;
using System.Linq;
using System.Threading;
using TextMateSharp.Grammars;
using System;
using Avalonia;

namespace LatexEditor.Views;

public partial class MainWindow : Window
{
    public bool ChangesMade = false;
    private CompletionWindow completionWindow;
    private OverloadInsightWindow insightWindow;
    public TextMate.Installation TextMate;

    public MainWindow()
    {
        InitializeComponent();

        // set theming and syntax highlighting
        var registryOptions = new RegistryOptions(ThemeName.DarkPlus);
        TextMate = textEditor.InstallTextMate(registryOptions);
        TextMate.SetGrammar(registryOptions.GetScopeByLanguageId(registryOptions.GetLanguageByExtension(".tex").Id));
        textEditor.KeyUp += OnTextEntered;
        fileTreeView.DoubleTapped += OnSelectTreeNode;

        LatexCompletionDataLoader.LoadFromDirectory("Completion");
    }

    private void Binding(object? sender, Avalonia.Input.KeyEventArgs e)
    {
    }

    public void SetChangeMarker()
    {
        var viewModel = DataContext as MainWindowViewModel;
        viewModel.Text = viewModel.Document.Text;
        if (!ChangesMade && viewModel.Text != viewModel.OriginalText)
        {
            ChangesMade = true;
            this.Title += " *";
        }
    }

    private void OnTextEntered(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        SetChangeMarker();

        var viewModel = DataContext as MainWindowViewModel;
        if (!string.IsNullOrWhiteSpace(e.KeySymbol) && viewModel.Text.Length > 0)
        {
            ShowCompletion();
            //ShowInsight();
        }
        else
        {
            if (completionWindow is not null && !string.IsNullOrEmpty(e.KeySymbol))
            {
                completionWindow.Close();
            }
        }
    }

    private async void OnSelectTreeNode(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        var viewModel = DataContext as MainWindowViewModel;
        var item = fileTreeView.SelectedItem as DirectoryNode;
        if (item != null && item.Path != null)
        {
            if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
            desktop.MainWindow?.StorageProvider is not { } provider)
                throw new NullReferenceException("Missing StorageProvider instance.");

            var file = await provider.TryGetFileFromPathAsync(item.Path);

            if (file != null)
            {
                var token = new CancellationToken();
                await viewModel.OpenFile(file, token);
            }
        }
    }

    private void ShowCompletion()
    {
        completionWindow = new CompletionWindow(textEditor.TextArea);
        completionWindow.Closed += (_, _) => completionWindow = null;

        var offset = textEditor.TextArea.Caret.Offset;
        var document = textEditor.TextArea.Document;

        int startOffset = offset;
        while (startOffset > 0 && !char.IsWhiteSpace(document.GetCharAt(startOffset - 1)))
        {
            startOffset--;
        }

        int length = offset - startOffset;
        string word = document.GetText(startOffset, length);

        var data = completionWindow.CompletionList.CompletionData;
        foreach (var item in LatexCompletionDataLoader.Data.Where(s => s.StartsWith(word)))
        {
            data.Add(new LatexCompletionData(item));
        }
        

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