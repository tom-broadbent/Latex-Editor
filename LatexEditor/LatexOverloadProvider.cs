using AvaloniaEdit.CodeCompletion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LatexEditor
{
    public class LatexOverloadProvider : IOverloadProvider
    {
        private readonly IList<(string header, string content)> items;
        private int selectedIndex;

        public LatexOverloadProvider(IList<(string header, string content)> items)
        {
            this.items = items;
            SelectedIndex = 0;
        }

        public int SelectedIndex
        {
            get => selectedIndex;
            set
            {
                selectedIndex = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentHeader));
                OnPropertyChanged(nameof(CurrentContent));
            }
        }

        public int Count => items.Count;
        public string CurrentIndexText => $"{SelectedIndex + 1} of {Count}";
        public object CurrentHeader => items[SelectedIndex].header;
        public object CurrentContent => items[SelectedIndex].content;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
