using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using LatexEditor.ViewModels;
using LatexEditor.Views;

namespace LatexEditor.Views;

public partial class ProjectCreator : Window
{
	public ProjectCreator()
	{
		InitializeComponent();
		DataContext = new ProjectCreatorViewModel();
		fromTemplate.Click += (s, e) => OpenTemplateSelector();
	}

	private async void OpenTemplateSelector()
	{
		var templateSelector = new TemplateSelector();
		var newProjectDir = await templateSelector.ShowDialog<string>(this);
		if (newProjectDir != null)
		{
			Close(newProjectDir);
		}
	}
}