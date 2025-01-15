using Avalonia.Controls;
using LatexEditor.ViewModels;
using AvaloniaMath.Controls;
using System;

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
                button.Click += SymbolClick;
                RootGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
                RootGrid.Children.Add(button);
            }
        }
    }

    private void SymbolClick(Object sender, EventArgs e)
    {
        if (sender is Button button && button.Content is FormulaBlock fb)
        {
            Close(fb.Formula);
        }
    }
}