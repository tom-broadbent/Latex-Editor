using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LatexEditor.ViewModels
{
    internal class SymbolPickerViewModel : ViewModelBase
    {
        public ObservableCollection<Symbol> Symbols { get; set; }

        public SymbolPickerViewModel()
        {
            Symbols = new ObservableCollection<Symbol>(new List<Symbol>
            {
                new Symbol(@"\oplus"),
                new Symbol(@"\rightarrow"),
                new Symbol(@"\pi")
            });
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
