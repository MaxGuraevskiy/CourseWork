using System;
using System.Collections.Generic;
using System.Linq;
using AlcoLib;
using Xamarin.Forms;

namespace AlcoTest
{
    public partial class ProfilePage : ContentPage
    {
        public static Profile profile = null;

        private List<double> regions = new List<double>(){-12, -11, -10, -9.5, -9, -8, -7, -6, -5, -4, -3.5, -3, -2, -1, 0, 1, 2, 3, 3.5, 4, 4.5, 5, 5.5, 5.75, 6, 6.5, 7, 8, 8.75, 9, 9.5, 10, 10.5, 11, 12, 12.75, 13, 14 };

        public double Region { get; private set; }

        public ProfilePage()
        {
            InitializeComponent();

            profile = App.ProfileDatabase.GetProfilesAsync().Result.First();
            Region = profile.Region;

            UpdateProfile(profile);
        }

        void UpdateProfile(Profile profile)
        {
            AgeStepper.Value = profile.Age;
            HeightStepper.Value = profile.Height;
            WeightSlider.Value = profile.Weight;
            SexPicker.SelectedItem = profile.Gender;
            RegionPicker.SelectedItem = RegionPicker.Items[regions.FindIndex(x=> x == Region)];
        }

        void ConfirmProfileCheckedChanged(object sender, EventArgs e)
        {
            var checkBox = (CheckBox)sender;
            if (!checkBox.IsChecked)
            {
                
                foreach (var shotDrinks in App.Database.GetShotDrinksAsync().Result)
                {
                    App.Database.DeleteShotDrinkAsync(shotDrinks);
                }
                ProfileStackLayout.IsEnabled = true;
            }
            else
            {
                App.ProfileDatabase.DeleteProfilesAsync();
                profile = new Profile((int)AgeStepper.Value, (int)HeightStepper.Value, (int)WeightSlider.Value, (string)SexPicker.SelectedItem, Region);
                App.ProfileDatabase.SaveProfileAsync(profile);
                ProfileStackLayout.IsEnabled = false;
            }
        }

        void RegionPickerSelectedIndexChanged(object sender, EventArgs e)
        {
            Region = regions[RegionPicker.SelectedIndex];
            App.ProfileDatabase.DeleteProfilesAsync();
            profile = new Profile((int)AgeStepper.Value, (int)HeightStepper.Value, (int)WeightSlider.Value, (string)SexPicker.SelectedItem, Region);
            App.ProfileDatabase.SaveProfileAsync(profile);
        }
    }
}
