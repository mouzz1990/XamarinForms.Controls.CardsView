using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Xamarin.CardsView
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();

            //Changing content
            cardView.MovementAnimationFinished += (s, e) => 
            {
                count++;
                cardView.DisplayedContent = new Label()
                {
                    Text = $"Counter: {count}",
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };
            };
		}

        int count = 0;

        private async void btnLeft_Clicked(object sender, EventArgs e)
        {
            await cardView.TurnCardLeft();
        }

        private async void btnRight_Clicked(object sender, EventArgs e)
        {
            await cardView.TurnCardRight();
        }

        private async void btnRandom_Clicked(object sender, EventArgs e)
        {
            await cardView.TurnCardRandom();
        }
    }
}
