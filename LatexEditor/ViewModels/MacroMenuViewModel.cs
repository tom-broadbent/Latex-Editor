using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LatexEditor.ViewModels
{
    public partial class MacroMenuViewModel: ViewModelBase
    {
        [ObservableProperty]
        private ObservableCollection<LatexMacro>? macros;
        public MacroMenuViewModel()
        {
            var json = File.ReadAllText("Macros.json");
            Macros = JsonConvert.DeserializeObject<ObservableCollection<LatexMacro>>(json);
        }

        [RelayCommand]
        public void NewMacro()
        {
            if (Macros != null)
            {
                Macros.Add(new LatexMacro());
            }
        }
    }
}
