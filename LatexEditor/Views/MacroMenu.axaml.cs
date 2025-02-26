using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using LatexEditor.ViewModels;
using LatexEditor.Views;
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

        AddButton.Click += (s, e) =>
        {
            if (Owner is MainWindow window)
            {
                var macro = MacroGrid.SelectedItem as LatexMacro;
                if (macro != null)
                {
                    var paramCount = macro.Latex?.Count(f => f == '#') ?? 0;
                    var insertText = $"\\newcommand{{\\{macro.CommandName}}}";
                    if (paramCount > 0)
                    {
                        insertText += $"[{paramCount}]";
                    }
                    insertText += $"{{{macro.Latex}}}\n";

                    var doc = window.textEditor.Document;
                    doc.Insert(0, insertText);
                    window.SetChangeMarker();
                }
            }
        };

        InsertButton.Click += (s, e) =>
        {
            if (Owner is MainWindow window)
            {
                var macro = MacroGrid.SelectedItem as LatexMacro;
                if (macro != null)
                {
                    var doc = window.textEditor.Document;
                    var offset = window.textEditor.CaretOffset;
                    doc.Insert(offset, macro.Latex);
                    window.SetChangeMarker();
                }
            }
        };

        DuplicateButton.Click += (s, e) =>
        {
            var macro = MacroGrid.SelectedItem as LatexMacro;
            if (macro != null)
            {
                var index = vm.Macros.IndexOf(macro);
                vm.Macros.Insert(index, new LatexMacro() { Name = macro.Name, Latex = macro.Latex, CommandName = macro.CommandName });
            }
        };

        DeleteButton.Click += (s, e) =>
        {
            var macro = MacroGrid.SelectedItem as LatexMacro;
            if (macro != null)
            {
                vm.Macros.Remove(macro);
            }
        };
    }

    
}