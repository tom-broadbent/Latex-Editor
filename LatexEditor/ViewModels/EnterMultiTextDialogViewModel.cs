using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LatexEditor.ViewModels
{
    public partial class EnterMultiTextDialogViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ObservableCollection<string> textBoxWatermarks = new();

        [ObservableProperty]
        private ObservableCollection<string> textBoxDefaultTexts = new();
    }
}
