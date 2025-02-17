using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LatexEditor.ViewModels
{
    public partial class OptionsMenuViewModel : ViewModelBase
    {
        [ObservableProperty]
        private List<string> bibliographyBackends = new List<string>()
        {
            "bibtex", "biber"
        };
    }
}
