using System;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LatexEditor.ViewModels;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace LatexEditor.Views;

public partial class TemplateSelector : Window
{
	private string? template;
	public TemplateSelector()
	{
		InitializeComponent();
		var vm = new TemplateSelectorViewModel();
		DataContext = vm;
		CancelButton.Click += CancelButtonClick;
		OkButton.Click += OkButtonClick;
		
		var i = 0;
		foreach (var button in vm.TemplateButtons)
		{
			TemplateSelection.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
			button[Grid.RowProperty] = i++;
			TemplateSelection.Children.Add(button);
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
			if (vm.SelectedTemplate != null && vm.ProjectDirectoryPath != "..." && !string.IsNullOrEmpty(vm.ProjectName))
			{
				try
				{
					var project = Path.Join(vm.ProjectDirectoryPath, vm.ProjectName);
					FsUtils.CopyDirectory(vm.SelectedTemplate, project, true);
					if (Directory.Exists(project))
					{
						Close(project);
					}
				}
				catch (Exception ex)
				{
					var box = MessageBoxManager.GetMessageBoxStandard("Error", ex.ToString(), ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
					box.ShowAsync();
				}
			}
		}
	}
}