using MixMatch2.Shared.Views.Tests;

namespace MixMatch2.Shared.Views.Pages;

public partial class TestsPage : ContentPage
{
	public TestsPage()
	{
		InitializeComponent();
        Shell.SetNavBarIsVisible(this, false);
        Shell.SetTabBarBackgroundColor(this, Colors.Transparent);
    }
}