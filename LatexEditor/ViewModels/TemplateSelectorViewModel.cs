using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LatexEditor.ViewModels
{
    internal partial class TemplateSelectorViewModel : ViewModelBase
    {
        [ObservableProperty]
        private List<string> templates;

        public TemplateSelectorViewModel()
        {
            var templateDirs = Directory.GetDirectories("Templates", "*", SearchOption.AllDirectories);
            var templateFiles = templateDirs.Select(d => Directory.GetFiles(d));
            templates = templateDirs
                        .Where(d => Directory.GetFiles(d, "*.tex").Length > 0).ToList();
        }
    }
}
