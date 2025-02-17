using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using LatexEditor.ViewModels;
using Newtonsoft.Json;
using System;
using System.IO;

namespace LatexEditor.Views;

public partial class OptionsMenu : Window
{
    private MainWindow mainWindow => ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).MainWindow as MainWindow;
    public OptionsMenu()
    {
        InitializeComponent();

        var vm = new OptionsMenuViewModel();
        DataContext = vm;
        if (vm != null)
        {
            foreach(var item in vm.BibliographyBackends)
            {
                BibBackendCombo.Items.Add(item);
            }
        }

        if (mainWindow != null)
        {
            BibBackendCombo.SelectedItem = mainWindow.config?.BibBackend ?? "biber";
        }

        CancelButton.Click += CancelButtonClick;
        OkButton.Click += OkButtonClick;
    }

    private void CancelButtonClick(object? sender, EventArgs e)
    {
        Close();
    }

    private void OkButtonClick(object? sender, EventArgs e)
    {
        if (mainWindow != null)
        {
            mainWindow.config.BibBackend = (string?) BibBackendCombo.SelectedItem ?? "biber";

            var json = JsonConvert.SerializeObject(mainWindow.config);
            File.WriteAllText("appsettings.json", json);
            Close();
        }
    }
}