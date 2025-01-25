using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LatexEditor.ViewModels
{
    public partial class NewDocumentFormatterViewModel : ViewModelBase
    {
        [ObservableProperty]
        private List<string> documentClasses = new List<string>()
        {
            "article", "report", "book", "slides", "beamer", "letter"
        };

        [ObservableProperty]
        private List<string> pageSizes = new List<string>()
        {
            "a0paper", "a1paper", "a2paper", "a3paper", "a4paper", "a5paper", "a6paper",
            "b0paper", "b1paper", "b2paper", "b3paper", "b4paper", "b5paper", "b6paper",
            "c0paper", "c1paper", "c2paper", "c3paper", "c4paper", "c5paper", "c6paper",
            "b0j", "b1j", "b2j", "b3j", "b4j", "b5j", "b6j",
            "ansiapaper", "ansibpaper", "ansicpaper", "ansidpaper", "ansiepaper",
            "letterpaper", "executivepaper", "legalpaper"
        };

        [ObservableProperty]
        private List<string> pageOrientations = new List<string>()
        {
            "portrait", "landscape"
        };

        [ObservableProperty]
        private float topMargin = 0;

        [ObservableProperty]
        private float bottomMargin = 0;

        [ObservableProperty]
        private float leftMargin = 0;

        [ObservableProperty]
        private float rightMargin = 0;

        [ObservableProperty]
        private int numberOfColumns = 1;

        [ObservableProperty]
        private float columnSeparation = 1;

        [ObservableProperty]
        private bool usePageNumbers = false;

        [RelayCommand]
        private void ToggleUsePageNumbers()
        {
            UsePageNumbers = !UsePageNumbers;
        }
    }
}
