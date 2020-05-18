using System;
using SQLite;

namespace AlcoLib
{
    public class ShotDrink : Drink
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public DateTime DateTime { get; set; }
        public string ImageURL { get; set; }
        public double Region { get; set; }


        public ShotDrink(string name, double volume, double degree, DateTime dateTime, string imageURL, double region) : base(name, volume, degree)
        {
            DateTime = dateTime;
            ImageURL = imageURL;
            Region = region;
        }

        public ShotDrink() : base()
        {
            DateTime = DateTime.MinValue;
            ImageURL = string.Empty;
            Region = 0;
        }
    }
}
