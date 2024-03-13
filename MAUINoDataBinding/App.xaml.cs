namespace MAUINoDataBinding;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		// By changing this to instantiate MainPage instead of AppShell,
		// we remove one layer of complexity.Now whatever we put on MainPage
		// is what we control, with no extra containers around it.
		//
		// For this demo, I have deleted AppShell, but you can also simply
		// ignore it.
		MainPage = new MainPage();
	}
}

