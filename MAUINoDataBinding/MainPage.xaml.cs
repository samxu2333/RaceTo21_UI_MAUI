using Microsoft.Maui.Layouts;
using Microsoft.Maui.Controls;

namespace MAUINoDataBinding
{
    public partial class MainPage : ContentPage
    {
        //private int playerNumber = 1;

        public MainPage()
        {
            InitializeComponent();
        }

        private void Start_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new GamePage());
        }

    }
}