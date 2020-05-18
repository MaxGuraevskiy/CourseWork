using System;
using SQLite;

namespace AlcoLib
{
    public class Profile
    {

        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public int Age { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public string Gender { get; set; }
        public double Region { get; set; }

        public Profile()
        {
            Age = 25;
            Height = 180;
            Weight = 80;
            Gender = "Male";
            Region = 0;
        }

        public Profile(int age, int height, int weight, string gender, double region)
        {
            Age = age;
            Height = height;
            Weight = weight;
            Gender = gender;
            Region = region;
        }
    }
}
