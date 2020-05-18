using System;
using System.Collections.Generic;
using System.Reflection;
using AlcoLib;
using System.Linq;
using Xamarin.Forms;
using System.Windows.Input;
using Xamarin.Forms.PlatformConfiguration.GTKSpecific;
using System.Collections;
using System.ComponentModel;
using Xamarin.Essentials;
using System.Threading.Tasks;
using Plugin.MaterialDesignControls;
using System.Threading;

namespace AlcoTest
{
    public partial class Lobby : ContentPage
    {
        private static List<ShotDrink> baseDrinks = new List<ShotDrink>() {
            // Alcohol
            new ShotDrink("Whiskey", 0.2, 40, DateTime.MinValue, "Whiskey.png", 0),
            new ShotDrink("Beer", 0.6, 4, DateTime.MinValue, "Beer.png", 0),
            new ShotDrink("Cocktail", 0.5, 7, DateTime.MinValue, "Cocktail.png", 0),
            new ShotDrink("Martini", 0.4, 16, DateTime.MinValue, "Martini.png", 0),
            new ShotDrink("Tequila", 0.1, 40, DateTime.MinValue, "Tequila.png", 0),
            new ShotDrink("Vodka", 0.1, 40, DateTime.MinValue, "Vodka.png", 0),
            new ShotDrink("Wine", 0.7, 12, DateTime.MinValue, "Wine.png", 0),
            new ShotDrink("Champagne", 0.3, 15, DateTime.MinValue, "Champagne.png", 0),

            // Non-Alcohol (for fun)
            new ShotDrink("Juice", 0.4, 0, DateTime.MinValue, "Juice.png", 0),
            new ShotDrink("Coffee", 0.4, 0, DateTime.MinValue, "Coffee.png", 0),
            new ShotDrink("Energy Drink", 0.5, 0, DateTime.MinValue, "Energy Drink.png", 0)
        };

        // That's all about volume of your new shotted drink
        public List<string> VolumeItemsSource { get => (List<string>)VolumeDoublePicker.ItemsSource; set { VolumeDoublePicker.ItemsSource = value; } }
        public List<string> VolumeSecondaryItemsSource { get => (List<string>)VolumeDoublePicker.SecondaryItemsSource; set { VolumeDoublePicker.SecondaryItemsSource = value; } }
        public string VolumeSelectedItem { get => VolumeDoublePicker.SelectedItem; set { VolumeDoublePicker.SelectedItem = value; } } 
        public string VolumeSecondarySelectedItem { get => VolumeDoublePicker.SecondarySelectedItem; set { VolumeDoublePicker.SecondarySelectedItem = value; } }
        private double NewVolume { get => double.Parse(VolumeSelectedItem) + double.Parse(VolumeSecondarySelectedItem); }
        public string NewVolumeString { get => NewVolume.ToString() + "l"; }

        // That's all about degree of your new shotted drink
        public List<string> DegreeItemsSource { get => (List<string>)DegreeDoublePicker.ItemsSource; set { DegreeDoublePicker.ItemsSource = value; } }
        public List<string> DegreeSecondaryItemsSource { get => (List<string>)DegreeDoublePicker.SecondaryItemsSource; set { DegreeDoublePicker.SecondaryItemsSource = value; } }
        public string DegreeSelectedItem { get => DegreeDoublePicker.SelectedItem; set { DegreeDoublePicker.SelectedItem = value; } }
        public string DegreeSecondarySelectedItem { get => DegreeDoublePicker.SecondarySelectedItem; set { DegreeDoublePicker.SecondarySelectedItem = value; } }
        private double NewDegree { get => double.Parse(DegreeSelectedItem) + double.Parse(DegreeSecondarySelectedItem); }
        public string NewDegreeString { get => NewDegree.ToString() + "°"; }                                                                    

        // Name of your new shotted drink
        public string NewName { get => DrinkTypePicker.SelectedItem.ToString(); set { DrinkTypePicker.SelectedItem = value; } }
        // Image's url of your new shotted drink
        public string NewImageURL { get => baseDrinks.Find(x => x.Name == NewName).ImageURL; set { TypeImage.Source = value; } }

        public double Region { get; private set; }
         
        public static Profile profile = null;

        List<string> FillFirstDegreeSource()
        {
            var degreeItemsSource = new List<string>();
            for (int i = 0; i < 100; ++i)
            {
                degreeItemsSource.Add(i.ToString());
            }
            return degreeItemsSource;
        }

        double previousPromille = 0.0;

        public Lobby()
        {
            InitializeComponent();

            // init volume picker
            VolumeDoublePicker.ItemsSource = new List<string>(){
            "0.0","0.1","0.2","0.3","0.4","0.5","0.6","0.7","0.8","0.9","1.0","1.1","1.2","1.3","1.4","1.5"};
            VolumeDoublePicker.SecondaryItemsSource = new List<string>(){
                "0.00","0.01","0.02","0.03","0.04","0.05","0.06","0.07","0.08","0.09"};
            VolumeSelectedItem = "0.2";
            VolumeSecondarySelectedItem  = "0.00";

            // init degree picker
            DegreeDoublePicker.ItemsSource = FillFirstDegreeSource();
            DegreeDoublePicker.SecondaryItemsSource = new List<string>(){
                "0.0","0.1","0.2","0.3","0.4","0.5","0.6","0.7","0.8","0.9"};
            DegreeSelectedItem = "40";
            DegreeSecondarySelectedItem = "0.0";

            // init list of names of drinks
            DrinkTypePicker.ItemsSource = baseDrinks.Select(x => x.Name).ToList() as IList;


            NewName = "Whiskey";
            TypeImage.Source = NewImageURL;
            VolumeButton.Text = NewVolumeString;
            DegreeButton.Text = NewDegreeString;

            if(App.ProfileDatabase.GetProfilesAsync().Result.Count != 1)
            {
                App.ProfileDatabase.DeleteProfilesAsync();
                App.ProfileDatabase.SaveProfileAsync(new Profile(25, 170, 70, "Male", 3.0));
            }
            profile = App.ProfileDatabase.GetProfilesAsync().Result.First();

            Region = profile.Region;

            // async method for updating list of shotted drinks and update your promille in real time
            DeleteOldDrinks();
            UpdatePromille();
        }

        void ShowAlertMessageOfHighBAC(double newPromille)
        {
            if(previousPromille - newPromille > 1)
                previousPromille = Math.Truncate(newPromille);

            if (newPromille - previousPromille > 1)
            {
                previousPromille = Math.Truncate(newPromille);
                if (newPromille > 1.0 && newPromille < 2.0)
                    NewDisplayAlertOK("Your BAC is more than .1",
                        // Behavior
                        $"Behavior:{Environment.NewLine}" +
                        $"1. Analgesia{Environment.NewLine}" +
                        $"2. Ataxia{Environment.NewLine}" +
                        $"3. Boisterousness{Environment.NewLine}" +
                        $"4. Over expressed emotions{Environment.NewLine}" +
                        $"5. Possibility of nausea and vomiting{Environment.NewLine}" +
                        $"6. Spins{Environment.NewLine}" +
                        // Impairment
                        $"Impairment:{Environment.NewLine}" +
                        $"1. Gross motor skill{Environment.NewLine}" +
                        $"2. Motor planning{Environment.NewLine}" +
                        $"3. Reflexes{Environment.NewLine}" +
                        $"4. Relaxed pronunciation{Environment.NewLine}" +
                        $"5. Staggering{Environment.NewLine}" +
                        $"6. Temporary erectile dysfunction{Environment.NewLine}");
                if (newPromille > 2.0 && newPromille < 3.0)
                    NewDisplayAlertOK("Your BAC is more than .2",
                        // Behavior
                        $"Behavior:{Environment.NewLine}" +
                        $"1. Anger or sadness{Environment.NewLine}" +
                        $"2. Anterograde amnesia{Environment.NewLine}" +
                        $"3. Impaired sensations{Environment.NewLine}" +
                        $"4. Inhibited sexual desire (ISD){Environment.NewLine}" +
                        $"5. Mood swings{Environment.NewLine}" +
                        $"6. Nausea{Environment.NewLine}" +
                        $"7. Partial loss of understanding{Environment.NewLine}" +
                        $"8. Possibility of stupor{Environment.NewLine}" +
                        $"9. Vomiting{Environment.NewLine}" +
                        // Impairment
                        $"Impairment:{Environment.NewLine}" +
                        $"1. Amnesia (memory blackout){Environment.NewLine}" +
                        $"2. Unconsciousness{Environment.NewLine}" +
                        $"3. Severe physical disability{Environment.NewLine}");
                if (newPromille > 3.0 && newPromille < 4.0)
                    NewDisplayAlertOK("Your BAC is more than .2",
                        // Behavior
                        $"Behavior:{Environment.NewLine}" +
                        $"1. Central nervous system depression{Environment.NewLine}" +
                        $"2. Lapses in and out of consciousness{Environment.NewLine}" +
                        $"3. Loss of understanding{Environment.NewLine}" +
                        $"4. Low possibility of death{Environment.NewLine}" +
                        $"5. Pulmonary aspiration{Environment.NewLine}" +
                        $"6. Stupor{Environment.NewLine}" +
                        // Impairment
                        $"Impairment:{Environment.NewLine}" +
                        $"1. Dysequilibrium{Environment.NewLine}" +
                        $"2. Breathing{Environment.NewLine}" +
                        $"3. Resting heart rate{Environment.NewLine}" +
                        $"4. Urinary incontinence{Environment.NewLine}");
                if (newPromille > 4.0 && newPromille < 5.0)
                    NewDisplayAlertOK("Your BAC is more than .2",
                        // Behavior
                        $"Behavior:{Environment.NewLine}" +
                        $"1. Coma{Environment.NewLine}" +
                        $"2. Possibility of death{Environment.NewLine}" +
                        $"3. Severe central nervous system depression{Environment.NewLine}" +
                        // Impairment
                        $"Impairment:{Environment.NewLine}" +
                        $"1. Respiratory failure{Environment.NewLine}" +
                        $"2. Heart rate{Environment.NewLine}" +
                        $"3. Positional alcohol nystagmus{Environment.NewLine}");
            }
            
        }

        private async void UpdatePromille()
        {
            profile = App.ProfileDatabase.GetProfilesAsync().Result.First();
            Region = profile.Region;

            var newPromille = 0.0;
            newPromille += Formula(App.Database.GetShotDrinksAsync().Result);

            PromilleButton.Text = (newPromille).ToString("F2") + "‰";
            ExhaledAirLabel.Text = $"or {string.Format("{0:f2}",newPromille/2)} mg/l in exhaled air";

            ShowAlertMessageOfHighBAC(newPromille);

            await Task.Delay(3000);
            UpdatePromille();
        }

        private double Formula(List<ShotDrink> shotDrinks)
        {
            var newPromille = 0.0;

            foreach (var drink in shotDrinks)
            {
                newPromille += Formula(drink);
            }
            return newPromille;
        }

        private double Formula(ShotDrink drink)
        {
            double newPromille;
            if (profile.Gender == "Male")
            {
                newPromille = (Formula(drink.Volume, drink.Degree, drink.DateTime, drink.Region) * 0.8)
                    / (2.447 - 0.09516 * profile.Age + 0.1074 * profile.Height + 0.3362 * profile.Weight);
            }
            else
            {
                newPromille = (Formula(drink.Volume, drink.Degree, drink.DateTime, drink.Region) * 0.8)
                    / (-2.097 + 0.1069 * profile.Height + 0.2466 * profile.Weight);
            }

            newPromille -= (0.15 * TimeFormula(drink.DateTime, drink.Region));

            if (newPromille <= 0)
            {
                //App.Database.DeleteShotDrinkAsync(drink);
                return 0.0;
            }

            return newPromille;
        }

        private double Formula(double volume, double degree, DateTime dateTime, double region)
        {
            return volume * 1000 * (degree / 100) * 0.789 * (1 - Math.Exp(-2 * (TimeFormula(dateTime, region))));
        }

        private double TimeFormula(DateTime dateTime, double region)
        {
            return (new TimeSpan(DateTime.Now.Ticks - dateTime.Ticks).TotalHours) + Region - region;
        }

        private async void DeleteOldDrinks()
        {
            var ourDrinks = App.Database.GetShotDrinksAsync().Result;
            ourDrinks.FindAll(x => DateTime.Now + new TimeSpan(0, (int)Math.Round(Math.Truncate(60 * Region), 0), 0)
                              < x.DateTime + new TimeSpan(0, (int)Math.Round((Math.Truncate(60 * x.Region)), 0), 0) )
                              .Select(x => App.Database.DeleteShotDrinkAsync(x));
            ourDrinks.FindAll(x => x.DateTime.TimeOfDay + new TimeSpan(0, (int)(Math.Truncate(60 * x.Region)), 0)
                              > new TimeSpan(24,0,0) + new TimeSpan(0, (int)Math.Round(Math.Truncate(60 * Region), 0), 0));
            await Task.Delay(60000);
            DeleteOldDrinks();
        }

        void DegreeDoublePickerSelectedIndexesChanged(object sender, SelectedIndexesEventArgs e)
        {
            DegreeButton.Text = NewDegreeString;
        }

        void VolumeDoublePickerSelectedIndexesChanged(object sender, SelectedIndexesEventArgs e)
        {
            VolumeButton.Text = NewVolumeString;
        }

        void OnDrinkTypePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            TypeImage.Source = NewImageURL;
            VolumeDoublePicker.SelectedItem = baseDrinks[DrinkTypePicker.SelectedIndex].Volume.ToString();
            DegreeDoublePicker.SelectedItem = baseDrinks[DrinkTypePicker.SelectedIndex].Degree.ToString();
            DegreeButton.Text = NewDegreeString;
            VolumeButton.Text = NewVolumeString;
        }

        async void OnListClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ShottedDrinks());
        }

        async void OnProfileClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProfilePage());
        }

        async void OnPromilleButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ShottedDrinks());
        }

        void OnTypeDrinkClicked(object sender, EventArgs e)
        {
            if (DrinkTypePicker.IsFocused)
                DrinkTypePicker.Unfocus();
            DrinkTypePicker.Focus();
        }

        void OnVolumeClicked(object sender, EventArgs e)
        {
            if (VolumeDoublePicker.IsFocused)
                VolumeDoublePicker.Unfocus();
            VolumeDoublePicker.Focus();
        }

        void OnDegreeClicked(object sender, EventArgs e)
        {
            if (DegreeDoublePicker.IsFocused)
                DegreeDoublePicker.Unfocus();
            DegreeDoublePicker.Focus();
        }

        async void OnConfirmClicked(object sender, EventArgs e)
        {
            var drink = new ShotDrink(NewName, NewVolume, NewDegree, DateTime.Now, NewImageURL, Region);
            drink.DateTime = DateTime.Now;
            await App.Database.SaveShotDrinkAsync(drink);
        }

        async void OnInfoClicked(object sender, EventArgs e)
        {
            await Application.Current.MainPage.DisplayAlert("Info",
                $"Drinking more than 2 standard drinks a day can seriously affect your health over your lifetime. {Environment.NewLine}" +
                $"It can lead to dependence and addiction, especially in people who have depression or anxiety, and can increase your risk of suicide.{Environment.NewLine}" +
                $"Here is how regular heavy drinking can affect your body long term.{Environment.NewLine}" +
                $"Brain: Drinking too much can affect your concentration, judgement, mood and memory. It increases your risk of having a stroke and developing dementia.{Environment.NewLine}" +
                $"Heart: Heavy drinking increases your blood pressure and can lead to heart damage and heart attacks.{Environment.NewLine}" +
                $"Liver: Drinking 3 to 4 standard drinks a day increases your risk of developing liver cancer. Long-term heavy drinking also puts you at increased risk of liver cirrhosis (scarring) and death.{Environment.NewLine}" +
                $"Stomach: Drinking even 1 to 2 standard drinks a day increases your risk of stomach and bowel cancer, as well as stomach ulcers.{Environment.NewLine}Fertility: Regular heavy drinking reduces men's testosterone levels, sperm count and fertility. For women, drinking too much can affect their periods.",
                "OK");
        }

        async void NewDisplayAlertOK(string title, string mainText)
        {
            await Application.Current.MainPage.DisplayAlert(title, mainText, "OK");
        }
    }
}
