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
		custom.Click += (s, e) => OpenNewDocumentFormatter();
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

	private async void OpenNewDocumentFormatter()
	{
		var docFormatter = new NewDocumentFormatter()
		{
			Width = 600,
			Height = 450
		};
		var newProjectDir = await docFormatter.ShowDialog<string>(this);
		if (newProjectDir != null)
		{
			Close(newProjectDir);
		}
	}
}