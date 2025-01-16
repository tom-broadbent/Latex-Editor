using Avalonia.Controls;
using Avalonia.Layout;
using LatexEditor.ViewModels;
using AvaloniaMath.Controls;
using System;
using Avalonia.LogicalTree;
using System.Linq;

namespace LatexEditor.Views;

internal partial class SymbolPicker : Window
{
    internal SymbolPicker(SymbolPickerViewModel vm)
    {
        DataContext = vm;
        Width = 537;
        Height = 600;
        InitializeComponent();

        foreach (var expander in vm.Expanders)
        {
            RootGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            foreach (var button in expander.GetLogicalDescendants().OfType<Button>())
            {
                button.Click += SymbolClick;
            }
            RootGrid.Children.Add(expander);
        }
    }

    private void SymbolClick(Object sender, EventArgs e)
    {
        if (sender is Button button && button.Content is FormulaBlock fb)
        {
            RootGrid.Children.Clear();
            Close(fb.Formula);
        }
    }
}