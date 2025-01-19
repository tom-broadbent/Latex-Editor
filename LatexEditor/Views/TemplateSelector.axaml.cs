using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LatexEditor.ViewModels;

namespace LatexEditor.Views;

public partial class TemplateSelector : Window
{
	public TemplateSelector()
	{
		InitializeComponent();
		var vm = new TemplateSelectorViewModel();
		DataContext = vm;
		CancelButton.Click += CancelButtonClick;
		
		var i = 0;
		foreach (var button in vm.TemplateButtons)
		{
			TemplateSelection.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
			button[Grid.RowProperty] = i++;
			TemplateSelection.Children.Add(button);
			button.Click += TemplateButtonClick;
		}
	}
	
	private void TemplateButtonClick(object? sender, EventArgs e)
	{
		if (sender is Button button)
		{
			((TemplateSelectorViewModel) DataContext).TemplateButtons.ForEach(b => b.IsEnabled = true);
			button.IsEnabled = false;
		}
	}
	
	private void CancelButtonClick(object? sender, EventArgs e)
	{
		Close();
	}
}