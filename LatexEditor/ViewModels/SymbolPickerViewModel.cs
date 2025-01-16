using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
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
        public ObservableCollection<List<Symbol>> SymbolGroups { get; set; }
        public int MaxColumns = 10;
        public List<Expander> Expanders { get; } = new List<Expander>();

        /*private Dictionary<string, string> replacements = new Dictionary<string, string>()
        {
            "\\notin": "\\not\\in"
        };*/

        public SymbolPickerViewModel()
        {
            SymbolGroups = new ObservableCollection<List<Symbol>>();

            var files = Directory.EnumerateFiles("Symbols").Where(x => x.EndsWith(".txt"));
            var expanderRow = 0;
            foreach (var filename in files)
            {
                var symbolList = File.ReadLines(filename);
                var symbolGroup = new List<Symbol>();
                foreach (var symbol in symbolList)
                {
                    symbolGroup.Add(new Symbol(symbol));
                }
                SymbolGroups.Add(symbolGroup);


                var buttonGrid = new Grid();

                for (var c = 0; c < MaxColumns; c++)
                {
                    buttonGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
                }

                var rowCount = (symbolGroup.Count + MaxColumns - 1) / MaxColumns;
                for (var r = 0; r < rowCount; r++)
                {
                    buttonGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
                }

                var expander = new Expander()
                {
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Content = buttonGrid,
                    Header = Path.GetFileNameWithoutExtension(filename),
                    [Grid.RowProperty] = expanderRow
                };

                Expanders.Add(expander);
                expanderRow++;
            }

            for (var i = 0; i < Expanders.Count; i++)
            {
                var j = 0;
                var symbolGroup = SymbolGroups[i];
                var buttonGrid = Expanders[i].Content as Grid;

                if (buttonGrid != null)
                {
                    foreach (var symbol in symbolGroup)
                    {
                        var col = j % MaxColumns;
                        var row = j / MaxColumns;
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
                        buttonGrid.Children.Add(button);
                        j++;
                    }
                }
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
