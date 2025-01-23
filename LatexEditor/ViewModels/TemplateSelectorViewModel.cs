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
using Avalonia.Media.Imaging;
using Avalonia.Platform;

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
		
		public List<Expander> Expanders => _expanders.Values.ToList();
		private Dictionary<string, Expander> _expanders = new Dictionary<string, Expander>();
		
		private List<string> templates;
		private double templatePreviewWidth = 200.0;
		private int templateGridColumns = 4;

		public TemplateSelectorViewModel()
		{
			var templateDirs = Directory.GetDirectories("Templates", "*", SearchOption.AllDirectories);
			var templateFiles = templateDirs.Select(d => Directory.GetFiles(d));
			templates = templateDirs
						.Where(d => Directory.GetFiles(d, "*.tex").Length > 0).ToList();
			
			foreach (var template in templates)
			{
				var templateDir = Path.GetDirectoryName(template);
				if (!_expanders.ContainsKey(templateDir))
				{
					var grid = new Grid();
					for (int c = 0; c < templateGridColumns; c++)
					{
						grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
					}
					_expanders.Add(templateDir, new Expander()
					{
						Header = Path.GetFileNameWithoutExtension(templateDir),
						Content = grid
					});
				}
				var texFile = Directory.GetFiles(template, "*.tex")[0];
				SKBitmap? image = null;
				if (Path.Exists(texFile))
				{
					var pdf = Path.ChangeExtension(texFile, ".pdf");
					if (Path.Exists(pdf))
					{
						using (var fs = File.OpenRead(pdf))
						{
							image = PdfConvert.ToImage(fs, new Index(0));
						}
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
								Text = Path.GetFileNameWithoutExtension(template),
								HorizontalAlignment = HorizontalAlignment.Center
							}
						},
					}
				};
				
				IImage avaloniaImage;

				if (image != null)
				{
					avaloniaImage = image.ToAvaloniaImage();
                }
				else
				{
					avaloniaImage = new Bitmap(AssetLoader.Open(new Uri("avares://LatexEditor/Assets/NoPreview.png")));
                }

                var imageScale = templatePreviewWidth / avaloniaImage.Size.Width;
                var templatePreviewHeight = imageScale * avaloniaImage.Size.Height;
                var templatePreview = new Image()
                {
                    Source = avaloniaImage,
                    Width = templatePreviewWidth,
                    Height = templatePreviewHeight,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    RenderTransformOrigin = new RelativePoint(0, 0, RelativeUnit.Relative),
					RenderTransform = image != null ? new ScaleTransform(imageScale, imageScale) : default
                };


                ((StackPanel)button.Content).Children.Insert(0, templatePreview);
				
				var buttonGrid = _expanders[templateDir].Content as Grid;
				var gridColumn = buttonGrid.Children.Count % templateGridColumns;
				var gridRow = buttonGrid.Children.Count / templateGridColumns;
				if (gridRow >= buttonGrid.RowDefinitions.Count)
				{
					buttonGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
				}
				button[Grid.ColumnProperty] = gridColumn;
				button[Grid.RowProperty] = gridRow;
				buttonGrid.Children.Add(button);
				button.Click += (sender, e) => TemplateButtonClick(sender, e, template);
			}
		}
		
		private void TemplateButtonClick(object? sender, EventArgs e, string template)
		{
			if (sender is ToggleButton button)
			{
				_expanders.Values.ToList().ForEach(e => ((Grid)e.Content).Children.ToList().ForEach(b => 
				{
					if (b is ToggleButton button)
					{
						button.IsChecked = false;
					}
				}));
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
