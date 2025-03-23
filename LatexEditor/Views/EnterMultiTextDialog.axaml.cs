using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LatexEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LatexEditor.Views;

public partial class EnterMultiTextDialog : Window
{
    private List<TextBox> textBoxes = new();
    public EnterMultiTextDialog(EnterMultiTextDialogViewModel vm, int textBoxCount = 1)
    {
        if( textBoxCount < 1)
        {
            throw new ArgumentOutOfRangeException("textBoxCount must be greater than or equal to 1");
        }

        InitializeComponent();
        DataContext = vm;

        CancelButton.Click += CancelButton_Click;
        OkButton.Click += OkButton_Click;



        for (int i = 0; i < textBoxCount; i++)
        {
            var textBox = new TextBox()
            {
                Watermark = vm.TextBoxWatermarks.Count > i ? vm.TextBoxWatermarks[i] : null,
                Text = vm.TextBoxDefaultTexts.Count > i ? vm.TextBoxDefaultTexts[i] : null,
                [Grid.RowProperty] = i,
                [Grid.ColumnSpanProperty] = 2,
            };
            textBoxes.Add(textBox);
            MainGrid.Children.Add(textBox);

            MainGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        }

        CancelButton[Grid.RowProperty] = MainGrid.RowDefinitions.Count;
        OkButton[Grid.RowProperty] = MainGrid.RowDefinitions.Count;
        MainGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
    }

    private void CancelButton_Click(object? sender, EventArgs e)
    {
        Close();
    }

    private void OkButton_Click(object? sender, EventArgs e)
    {
        Close(textBoxes.Select(box => box.Text).ToList());
    }
}