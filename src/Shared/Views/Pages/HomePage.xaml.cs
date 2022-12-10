using MixMatch2.Shared.ViewModels;

namespace MixMatch2.Shared.Views.Pages;

public partial class HomePage : ContentPage
{
	public HomePage()
	{
		InitializeComponent();
        Shell.SetNavBarIsVisible(this, false);
    }
}