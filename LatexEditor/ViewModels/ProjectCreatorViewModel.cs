using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LatexEditor.ViewModels
{
    public partial class ProjectCreatorViewModel : ViewModelBase
    {
        [ObservableProperty]
        protected string projectDirectoryPath = "...";
        protected IStorageFolder? projectDirectory;

        [ObservableProperty]
        protected string projectName = "";

        [RelayCommand]
        public async Task SelectProjectDir()
        {
            var dir = await FsUtils.DoOpenFolderPickerAsync();
            if (dir != null)
            {
                projectDirectory = dir;
                ProjectDirectoryPath = dir.Path.LocalPath;
            }
        }
    }
}
