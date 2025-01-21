using System;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using LatexEditor.ViewModels;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace LatexEditor.Views;

public partial class TemplateSelector : Window
{
	public TemplateSelector()
	{
		InitializeComponent();
		var vm = new TemplateSelectorViewModel();
		DataContext = vm;
		CancelButton.Click += CancelButtonClick;
		OkButton.Click += OkButtonClick;
		
		foreach (var expander in vm.Expanders)
		{
			TemplateSelection.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
			expander[Grid.RowProperty] = TemplateSelection.RowDefinitions.Count - 1;
			TemplateSelection.Children.Add(expander);
		}
	}
	
	private void CancelButtonClick(object? sender, EventArgs e)
	{
		Close();
	}
	
	private void OkButtonClick(object? sender, EventArgs e)
	{
		var vm = DataContext as TemplateSelectorViewModel;
		if (vm != null)
		{

			try
			{
				if (vm.SelectedTemplate == null)
				{
					throw new Exception("No template selected. Please select a template.");
				}
				else if (vm.ProjectDirectoryPath == "...")
				{
					throw new Exception("No directory selected. Please select a directory to contain the project.");
				}
				else if (string.IsNullOrEmpty(vm.ProjectName))
				{
					throw new Exception("No project name entered. Please enter a name for the project.");
				}
				else
				{
					var project = Path.Join(vm.ProjectDirectoryPath, vm.ProjectName);
					if (Directory.Exists(project))
					{
						throw new Exception($"{project} already exists. Please enter a different project name or select a different directory.");
					}
					FsUtils.CopyDirectory(vm.SelectedTemplate, project, true);
					if (Directory.Exists(project))
					{
						Close(project);
					}
				}
			}
			catch (Exception ex)
			{
				var box = MessageBoxManager.GetMessageBoxStandard("Error", ex.Message, ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
				box.ShowAsync();
			}
		}
	}
}