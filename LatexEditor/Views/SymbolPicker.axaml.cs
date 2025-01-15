using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LatexEditor.ViewModels;
using AvaloniaMath.Controls;

namespace LatexEditor.Views;

internal partial class SymbolPicker : Window
{
    internal SymbolPicker(SymbolPickerViewModel vm)
    {
        DataContext = vm;
        InitializeComponent();

        if (vm != null)
        {
            var i = 0;
            foreach (var symbol in vm.Symbols)
            {
                var button = new Button()
                {
                    Content = new FormulaBlock()
                    {
                        Formula = symbol.Latex
                    },
                    [Grid.RowProperty] = i++
                };
                RootGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
                RootGrid.Children.Add(button);
            }
        }
    }
}