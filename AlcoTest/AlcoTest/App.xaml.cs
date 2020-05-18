using System;
using Xamarin.Forms;
using AlcoLib;
using System.IO;

namespace AlcoTest
{
    public partial class App : Application
    {
        static DrinkDatabase database;

        public static DrinkDatabase Database
        {
            get
            {
                if (database == null)
                {
                    database = new DrinkDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AlcoTest.db3"));
                }
                return database;
            }
        }

        static ProfileDatabase profileDatabase;
        public static ProfileDatabase ProfileDatabase
        {
            get
            {
                if(profileDatabase == null)
                {
                    profileDatabase = new ProfileDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AlcoTestProfiles.db3"));
                }
                return profileDatabase;
            }
        }

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new Lobby());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
