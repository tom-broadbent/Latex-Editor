using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Input;
using LatexEditor.ViewModels;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System;
using System.Collections.Generic;
using System.IO;
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
        DocumentClassComboBox.SelectedIndex = 0;

        foreach (var item in vm.PageSizes)
        {
            PageSizeComboBox.Items.Add(item);
        }
        PageSizeComboBox.SelectedItem = vm.PageSizes.Contains("a4paper") ? "a4paper" : vm.PageSizes.First();

        foreach (var item in vm.PageOrientations)
        {
            PageOrientationComboBox.Items.Add(item);
        }
        PageOrientationComboBox.SelectedIndex = 0;

        foreach (var item in vm.PageNumbering)
        {
            PageNumberingComboBox.Items.Add(item);
        }
        PageNumberingComboBox.SelectedIndex = 0;

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

        CancelButton.Click += CancelButtonClick;
        OkButton.Click += OkButtonClick;

        var maxRow = (int?) RootGrid.Children.Select(x => x[Grid.RowProperty]).Max();
        if (maxRow != null)
        {
            for (var i = 0; i <= maxRow; i++)
            {
                RootGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            }
        }
    }

    private void CancelButtonClick(object? sender, EventArgs e)
    {
        Close();
    }

    private void OkButtonClick(object? sender, EventArgs e)
    {
        var vm = DataContext as NewDocumentFormatterViewModel;
        if (vm != null)
        {

            try
            {
                if (DocumentClassComboBox.SelectedItem == null)
                {
                    throw new Exception("You must select a document class.");
                }
                else if (PageSizeComboBox.SelectedItem == null)
                {
                    throw new Exception("You must select a page size.");
                }
                else if (PageOrientationComboBox.SelectedItem == null)
                {
                    throw new Exception("You must select a page orientation.");
                }
                else if (vm.NumberOfColumns <= 0)
                {
                    throw new Exception("Number of columns cannot be less than 1.");
                }
                else if (vm.ProjectDirectoryPath == "...")
                {
                    throw new Exception("No directory selected. Please select a directory to contain the project.");
                }
                else if (string.IsNullOrEmpty(vm.ProjectName))
                {
                    throw new Exception("No project name entered. Please enter a name for the project.");
                }
                else
                {
                    var project = Path.Join(vm.ProjectDirectoryPath, vm.ProjectName);
                    if (Directory.Exists(project))
                    {
                        throw new Exception($"{project} already exists. Please enter a different project name or select a different directory.");
                    }
                    Directory.CreateDirectory(project);
                    if (Directory.Exists(project))
                    {
                        var latex = GenerateLatex();
                        File.WriteAllText(Path.Join(project, "main.tex"), latex);
                        Close(project);
                    }
                }
            }
            catch (Exception ex)
            {
                var box = MessageBoxManager.GetMessageBoxStandard("Error", ex.Message, ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
                box.ShowAsync();
            }
        }
    }

    private string? GenerateLatex()
    {
        var vm = DataContext as NewDocumentFormatterViewModel;
        if (vm != null)
        {
            var latex = new List<string>();

            // documentclass
            latex.Add($"\\documentclass{{{DocumentClassComboBox.SelectedItem}}}");

            // geometry
            var geometry = $"\\usepackage[{PageSizeComboBox.SelectedItem}, {PageOrientationComboBox.SelectedItem}";
            if (LeftMarginUpDown.Value > 0)
            {
                geometry += $", left={LeftMarginUpDown.Value}cm";
            }
            if (RightMarginUpDown.Value > 0)
            {
                geometry += $", right={RightMarginUpDown.Value}cm";
            }
            if (TopMarginUpDown.Value > 0)
            {
                geometry += $", top={TopMarginUpDown.Value}cm";
            }
            if (BottomMarginUpDown.Value > 0)
            {
                geometry += $", bottom={BottomMarginUpDown.Value}cm";
            }
            geometry += "]{geometry}";
            latex.Add(geometry);

            // page numbering
            if (PageNumberingComboBox.SelectedValue is string pageNumberingStyle && pageNumberingStyle != "none")
            {
                latex.Add($"\\pagenumbering{{{pageNumberingStyle}}}");
            }

            // columns
            if (ColumnsUpDown.Value > 1)
            {
                latex.Add("\\usepackage{multicol}");
                latex.Add($"\\setlength{{\\columnsep}}{{{ColSepUpDown.Value}cm}}");
            }

            // main body
            latex.Add("\n\\begin{document}");

            if (ColumnsUpDown.Value > 1)
            {
                latex.Add($"\\begin{{multicols}}{{{ColumnsUpDown.Value}}}");
                latex.Add("[");
                latex.Add("% Header text (not affected by multicolumns)\n");
                latex.Add("]");
                latex.Add("% Multicolumn body");
            }
            else
            {
                latex.Add("% Main body");
            }

            latex.Add("\n\n");

            if (ColumnsUpDown.Value > 1)
            {
                latex.Add("\\end{multicols}");
            }

            latex.Add("\\end{document}");

            return string.Join("\n", latex);
        }
        return null;
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