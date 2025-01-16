using Avalonia.Controls;
using System;

namespace LatexEditor.Views;

public partial class EnterTextDialog : Window
{
    public EnterTextDialog()
    {
        InitializeComponent();

        CancelButton.Click += CancelButton_Click;
        OkButton.Click += OkButton_Click;
    }

    private void CancelButton_Click(object? sender, EventArgs e)
    {
        Close();
    }

    private void OkButton_Click(object? sender, EventArgs e)
    {
        Close(TextEntry.Text);
    }
}