using Avalonia.Controls;
using Avalonia.Layout;
using LatexEditor.ViewModels;
using AvaloniaMath.Controls;
using System;

namespace LatexEditor.Views;

internal partial class SymbolPicker : Window
{
    internal SymbolPicker(SymbolPickerViewModel vm)
    {
        DataContext = vm;
        Width = 510;
        Height = 600;
        InitializeComponent();

        for (var c = 0; c < vm.MaxColumns; c++)
        {
            RootGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        }


        var rowCount = (vm.Symbols.Count + vm.MaxColumns - 1) / vm.MaxColumns;
        for (var r = 0; r < rowCount; r++)
        {
            RootGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        }

        foreach (var button in vm.Buttons)
        {
            button.Click += SymbolClick;
            RootGrid.Children.Add(button);
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