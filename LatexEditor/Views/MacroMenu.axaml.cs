using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using LatexEditor.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;

namespace LatexEditor;

public partial class MacroMenu : Window
{
    MacroMenuViewModel vm;
    public MacroMenu()
    {
        InitializeComponent();
        vm = new MacroMenuViewModel();
        DataContext = vm;

        MacroGrid.CellEditEnded += (s, e) => 
        {
            if (s is DataGrid dg)
            {
                var temp = vm.Macros;
                vm.Macros = null;
                vm.Macros = temp;
            }
        };

        CancelButton.Click += (s, e) =>
        {
            Close();
        };

        OkButton.Click += (s, e) =>
        {
            var json = JsonConvert.SerializeObject(vm.Macros);
            File.WriteAllText("Macros.json", json);
            Close();
        };
    }

    
}