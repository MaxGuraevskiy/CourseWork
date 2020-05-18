using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AlcoLib;

using Xamarin.Forms;

namespace AlcoTest
{
    public partial class ShottedDrinks : ContentPage
    {
        //public static ObservableCollection<ShotDrink> drinks { get; set; }

        public ShottedDrinks()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var yourDrinks = await App.Database.GetShotDrinksAsync();
            yourDrinks.Reverse();
            CollectionView.ItemsSource = yourDrinks;
        }

        //async void OnDeleteImageButtonClicked(object sender, EventArgs e)
     

        void SwipeItemViewInvoked(object sender, EventArgs e)
        {
            var shotDrink = (sender as SwipeItemView).CommandParameter as ShotDrink;
            App.Database.DeleteShotDrinkAsync(shotDrink);

            var yourDrinks = App.Database.GetShotDrinksAsync().Result;
            yourDrinks.Reverse();
            CollectionView.ItemsSource = yourDrinks;
        }
    }
}