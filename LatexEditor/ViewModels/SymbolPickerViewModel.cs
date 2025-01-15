using Avalonia.Controls;
using Avalonia.Layout;
using AvaloniaMath.Controls;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace LatexEditor.ViewModels
{
    internal class SymbolPickerViewModel : ViewModelBase
    {
        public ObservableCollection<Symbol> Symbols { get; set; }
        public int MaxColumns = 10;
        public List<Button> Buttons { get; } = new List<Button>();

        public SymbolPickerViewModel()
        {
            Symbols = new ObservableCollection<Symbol>();

            var files = Directory.EnumerateFiles("Symbols").Where(x => x.EndsWith(".txt"));
            foreach (var filename in files)
            {
                var symbolList = File.ReadLines(filename);
                foreach (var symbol in symbolList)
                {
                    Symbols.Add(new Symbol(symbol));
                }
            }

            var i = 0;
            foreach (var symbol in Symbols)
            {
                var col = i % MaxColumns;
                var row = i / MaxColumns;
                var button = new Button()
                {
                    Content = new FormulaBlock()
                    {
                        Formula = symbol.Latex,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        ClipToBounds = false
                    },
                    Width = 50,
                    Height = 50,
                    [Grid.RowProperty] = row,
                    [Grid.ColumnProperty] = col
                };
                Buttons.Add(button);
                i++;
            }

        }
    }

    class Symbol
    {
        public string Latex { get; set; }
        public Symbol(string latex)
        {
            Latex = latex;
        }
    }
}
