using Microsoft.Maui.Layouts;
using Microsoft.Maui.Controls;

namespace MAUINoDataBinding
{
    public partial class MainPage : ContentPage
    {
        private int playerNumber = 1;

        public MainPage()
        {
            InitializeComponent();
            UpdateUI();
        }

        public void AdvancePlayer()
        {
            playerNumber++;
            if (playerNumber > 4) playerNumber = 1;
        }

        public void UpdateUI()
        {
            /* SANITY NOTE: If you create new x:Name property references in your XAML, 
             * be sure that you do not have your application running when you first try
             * to reference them in your C# code. I lost a full 90 minutes trying to figure out why
             * I couldn't reference PlayerName. Of course, I should have followed my own advice
             * and closed and re-opened the project much sooner, since that tipped me off to the
             * problem... but hopefully you won't make the same mistakes that I do!
             */

            PlayerName.Text = $"Player {playerNumber}:";

            /* CHALLENGE: Using the examples in this script, can you figure out a way to disable
             * the "Remove Robot" button when the current player has 0 robots and to re-enable it
             * when the current player has 1 or more robots?
             */
        }

        void Add_Clicked(System.Object sender, System.EventArgs e)
        {
            /* This next line dynamically finds a XAML item by its x:Name property. This is a very
             * common technique in Unity when looking for GameObjects, so it is a good one to get
             * used to in MAUI or WPF applications. Keyword this refers to the page for which this
             * C# script is the code-behind. The string interpolation yields PlayerN, where N is a 
             * number between 1 and 4 (see method AdvancePlayer() for this limitation). The
             * as HorizontalStackLayout code ensures that C# knows what type of XAML object we 
             * want to talk to. Finally, the Add method creates a new Image object and places it
             * at the end of the named XAML item's children.
            */

            var addToMe = this.FindByName($"Player{playerNumber}") as HorizontalStackLayout;
            /* If you use the MAUI project template, you are provided with a Resources folder that in
             * turn contains an Images folder. You can place your images in this folder and then reference
             * them by filename. Note that MAUI projects convert SVG to PNG before runtime,
             * so even if the file is an SVG (as our example file is -- dotnet_bot.svg),
             * you refer to the PNG version (as shown below)
             */
            addToMe.Add(new Image { Source = "dotnet_bot.png" });
            AdvancePlayer();
            UpdateUI();
        }

        void Remove_Clicked(System.Object sender, System.EventArgs e)
        {
            /* This cose uses the same methodology as Add_Clicked above, with one key difference:
             * Here, we want to remove the last child from the list of items inside of our layout.
             * To do this, we make sure that there are any children left to remove, and if so, then
             * we remove the child at Count - 1. Remember, in programming, indexed items (arrays, lists,
             * collections, etc.) almost always start with a 0 index. This means that the Count of the
             * number of items will be one more than the highest index used in the list. So, to remove
             * the last item, we remove the one at Count - 1. If this is unclear to you, try to remove
             * an item from Count (without -1) and see what happens!
             */

            var takeFromMe = this.FindByName($"Player{playerNumber}") as HorizontalStackLayout;
            if (takeFromMe.Children.Count > 0) takeFromMe.Children.RemoveAt(takeFromMe.Children.Count - 1);
            AdvancePlayer();
            UpdateUI();
        }
    }
}
