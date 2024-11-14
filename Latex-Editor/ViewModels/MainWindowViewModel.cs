using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Latex_Editor.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
#pragma warning disable CA1822 // Mark members as static
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(GetTextCommand))]
    private string text = "";

    public string LatexText { get; set; } = "";

    [RelayCommand]
    private void GetText()
    {
        LatexText = Text;
    }

    [RelayCommand]
    private void SetText()
    {
        Text = LatexText;
    }
#pragma warning restore CA1822 // Mark members as static
}
