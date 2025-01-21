using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PdfConvert = PDFtoImage.Conversion;
using Avalonia;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Controls.Primitives;

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
		[ObservableProperty]
		private string? selectedTemplateName;
		
		private List<string> templates;
		public List<ToggleButton> TemplateButtons = new List<ToggleButton>();
		private double templatePreviewWidth = 300.0;

		public TemplateSelectorViewModel()
		{
			var templateDirs = Directory.GetDirectories("Templates", "*", SearchOption.AllDirectories);
			var templateFiles = templateDirs.Select(d => Directory.GetFiles(d));
			templates = templateDirs
						.Where(d => Directory.GetFiles(d, "*.tex").Length > 0).ToList();
			
			foreach (var template in templates)
			{
				var texFile = Directory.GetFiles(template, "*.tex")[0];
				SKBitmap? image = null;
				if (Path.Exists(texFile))
				{
					using (var fs = File.OpenRead(Path.ChangeExtension(texFile, ".pdf")))
					{
						image = PdfConvert.ToImage(fs, new Index(0));
					}
				}

				var button = new ToggleButton()
				{
					Content = new StackPanel()
					{
						Children = 
						{							
							new TextBlock()
							{
								Text = Path.GetFileNameWithoutExtension(template)
							}
						},
					}
				};
				if (image != null)
				{
					var imageScale = templatePreviewWidth / image.Width;
					var templatePreviewHeight = imageScale * image.Height;
					((StackPanel)button.Content).Children.Insert(0, new Image()
					{
						Source = image.ToAvaloniaImage(),
						Width = templatePreviewWidth,
						Height = templatePreviewHeight,
						RenderTransform = new ScaleTransform(imageScale, imageScale),
						VerticalAlignment = VerticalAlignment.Top,
						HorizontalAlignment = HorizontalAlignment.Left,
						RenderTransformOrigin = new RelativePoint(0, 0, RelativeUnit.Relative)
					});
				}
				TemplateButtons.Add(button);
				button.Click += (sender, e) => TemplateButtonClick(sender, e, template);
			}
		}
		
		private void TemplateButtonClick(object? sender, EventArgs e, string template)
		{
			if (sender is ToggleButton button)
			{
				TemplateButtons.ForEach(b => b.IsChecked = false);
				button.IsChecked = true;
				SelectedTemplate = Path.GetFullPath(template);
				SelectedTemplateName = Path.GetFileNameWithoutExtension(template);
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
