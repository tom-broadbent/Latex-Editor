using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace LatexEditor.ViewModels
{
    public partial class EnterTextDialogViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string textBoxWatermark = "";

        [ObservableProperty]
        private string textBoxDefaultText = "";
    }
}
