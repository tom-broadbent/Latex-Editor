using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
		private string projectDirectoryPath = "...";
		private IStorageFolder? projectDirectory;
		
		[ObservableProperty]
		private string projectName = "";
		
		[ObservableProperty]
		private string? selectedTemplate;
		
		private List<string> templates;
		public List<Button> TemplateButtons = new List<Button>();

		public TemplateSelectorViewModel()
		{
			var templateDirs = Directory.GetDirectories("Templates", "*", SearchOption.AllDirectories);
			var templateFiles = templateDirs.Select(d => Directory.GetFiles(d));
			templates = templateDirs
						.Where(d => Directory.GetFiles(d, "*.tex").Length > 0).ToList();
			
			foreach (var template in templates)
			{
				var button = new Button()
				{
					Content = new StackPanel()
					{
						Children = 
						{
							new TextBlock()
							{
								Text = Path.GetFileNameWithoutExtension(template)
							}
						}
					}
				};
				TemplateButtons.Add(button);
				button.Click += (sender, e) => TemplateButtonClick(sender, e, template);
			}
		}
		
		private void TemplateButtonClick(object? sender, EventArgs e, string template)
		{
			if (sender is Button button)
			{
				TemplateButtons.ForEach(b => b.IsEnabled = true);
				button.IsEnabled = false;
				SelectedTemplate = Path.GetFullPath(template);
			}
		}
		
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
