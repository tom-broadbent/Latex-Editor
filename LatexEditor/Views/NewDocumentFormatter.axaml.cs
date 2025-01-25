using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Input;
using LatexEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LatexEditor.Views;

public partial class NewDocumentFormatter : Window
{
    public NewDocumentFormatter()
    {
        InitializeComponent();
        var vm = new NewDocumentFormatterViewModel();
        DataContext = vm;
        foreach (var item in vm.DocumentClasses)
        {
            DocumentClassComboBox.Items.Add(item);
        }

        foreach (var item in vm.PageSizes)
        {
            PageSizeComboBox.Items.Add(item);
        }

        foreach (var item in vm.PageOrientations)
        {
            PageOrientationComboBox.Items.Add(item);
        }

        var float_updowns = new List<NumericUpDown>()
        {
            TopMarginUpDown, BottomMarginUpDown, LeftMarginUpDown, RightMarginUpDown, ColSepUpDown
        };
        float_updowns.ForEach(updown => {
            updown.AddHandler(KeyDownEvent, UpDown_Format, RoutingStrategies.Tunnel);
        });

        var int_updowns = new List<NumericUpDown>()
        {
            ColumnsUpDown
        };
        int_updowns.ForEach(updown =>
        {
            updown.AddHandler(KeyDownEvent, UpDown_IntFormat, RoutingStrategies.Tunnel);
        });
    }

    private void UpDown_IntFormat(object? sender, KeyEventArgs e)
    {
        if (sender is NumericUpDown updown)
        {
            // Don't allow a decimal point
            if (e.Key == Key.OemPeriod)
            {
                e.Handled = true;
            }
            else
            {
                UpDown_Format(sender, e);
            }
        }
    }

    private void UpDown_Format(object? sender, KeyEventArgs e)
    {
        if (sender is NumericUpDown updown)
        {
            // Set to 0 on deletion if there is only one character
            if ((e.Key == Key.Back || e.Key == Key.Delete) && updown.Text.Length <= 1)
            {
                e.Handled = true;
                updown.Value = 0;
            }
            //Don't allow cut
            else if (e.Key == Key.X && (e.KeyModifiers == KeyModifiers.Control))
            {
                e.Handled = true;
            }
            // Don't allow a second decimal point
            else if (e.Key == Key.OemPeriod && updown.Text.Contains('.'))
            {
                e.Handled = true;
            }
            // Don't allow non-number characters
            else if (e.KeySymbol != null && (!e.KeySymbol.All(c => char.IsDigit(c) || c == '.' || c == '\b')))
            {
                e.Handled = true;
            }
            // Remove leading 0s
            else if (e.KeySymbol != null && e.KeySymbol.All(char.IsDigit) && updown.Text.First() == '0' && !updown.Text.Contains('.'))
            {
                updown.Text = updown.Text.Remove(0, 1);
            }
        }
    }
}