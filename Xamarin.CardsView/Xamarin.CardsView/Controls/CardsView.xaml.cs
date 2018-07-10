using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin.CardsView.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CardsView : ContentView
	{
		public CardsView ()
		{
			InitializeComponent ();
            PropertyChanged += (s,e) => 
            {
                if (e.PropertyName == nameof(Content))
                {
                    Content = mainGrid;
                }
            };
		}

        #region Bindable Properties
        public static readonly BindableProperty DisplayedContentProperty = BindableProperty.Create("DisplayedContent", typeof(View), typeof(CardsView), default(View),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                var mcv = bindable as CardsView;
                mcv.mainContent.Content = newValue as View;
                mcv.fadingContent.Content = newValue as View;
            });

        public View DisplayedContent
        {
            get { return (View)GetValue(DisplayedContentProperty); }
            set { SetValue(DisplayedContentProperty, value); }
        }

        public static readonly BindableProperty FrameColorProperty = BindableProperty.Create("FrameColor", typeof(Color), typeof(CardsView), default(Color),
            propertyChanged:
            (bindable, oldValue, newValue) => 
            {
                var cv = bindable as CardsView;
                foreach (var fr in cv.mainGrid.Children)
                    fr.BackgroundColor = (Color)newValue;
            });

        public Color FrameColor
        {
            get { return (Color)GetValue(FrameColorProperty); }
            set { SetValue(FrameColorProperty, value); }
        }
        #endregion

        #region Events
        /// <summary>
        /// Event to change content in the view
        /// </summary>
        public event EventHandler MovementAnimationFinished;
        private void OnMovementAnimationFinished()
        {
            MovementAnimationFinished?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Evenet to say that all animations are completed
        /// </summary>
        public event EventHandler TurnCardsAnimationFinished;
        private void OnTurnCardsAnimationFinished()
        {
            TurnCardsAnimationFinished?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        bool AnimationInProgress = false;

        /// <summary>
        /// Complete animation function
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
        private async Task TurnAnimationBase(Func<Task> act)
        {
            if (AnimationInProgress) return;

            AnimationInProgress = true;

            await MovingAnimation();
            OnMovementAnimationFinished();

            await act();

            fadingContent.Content = mainContent.Content;

            fadingContent.TranslationX = 0;
            fadingContent.Rotation = 0;

            await fadingContent.FadeTo(1, 250);

            OnTurnCardsAnimationFinished();
            AnimationInProgress = false;
        }

        public async Task TurnCardLeft()
        {
            await TurnAnimationBase(TurnCardsLeftAnimation);
        }

        public async Task TurnCardRight()
        {
            await TurnAnimationBase(TurnCardsRightAnimation);
        }

        public async Task TurnCardRandom()
        {
            await TurnAnimationBase(TurnCardRandomAnimation);
        }

        private async Task TurnCardsRightAnimation()
        {
            await Task.WhenAll
                (
                    fadingContent.TranslateTo(Width + fadingContent.Width, 0, 450),
                    fadingContent.RotateTo(180, 450),
                    fadingContent.FadeTo(0, 450)
                );
        }

        private async Task TurnCardsLeftAnimation()
        {
            await Task.WhenAll
                (
                    fadingContent.TranslateTo(-Width - fadingContent.Width, 0, 450),
                    fadingContent.RotateTo(-180, 450),
                    fadingContent.FadeTo(0, 450)
                );
        }

        private async Task TurnCardRandomAnimation()
        {
            Random rnd = new Random();
            int val = rnd.Next(10);
            if (val % 2 == 0)
                await TurnCardsLeftAnimation();
            else
                await TurnCardsRightAnimation();
        }

        private async Task MovingAnimation()
        {
            await Task.WhenAll(
            tempFrame.ScaleTo(0.9),
            tempFrame.TranslateTo(0, -10)
            );

            tempFrame.Scale = 0.8;
            tempFrame.TranslationY = -20;

            await Task.WhenAll(
            tempFrame2.ScaleTo(1),
            tempFrame2.TranslateTo(0, 0)
            );

            tempFrame2.Scale = 0.9;
            tempFrame2.TranslationY = -10;
        }
    }
}