using System;
namespace AlcoLib
{
    public class Drink
    {
        public string Name { get;  set; }

        // Degree of alcohol in drink
        private double degree = 0;
        public double Degree
        {
            get => degree;
            set {
                if (value < 0)
                    throw new AlcoException($"The degree ({value}) of your drink ({Name}) is less than 0");
                degree = value;
            }
        }

        // Volume is measured in liters
        private double volume = 1;
        public double Volume
        {
            get => volume;
            set
            {
                if (value < 0)
                    throw new AlcoException($"The volume ({value}) of your drink ({Name}) is less than 0");
                volume = value;
            }
        }

        public Drink(string name, double volume, double degree)
        {
            Name = name;
            Volume = volume;
            Degree = degree;
        }

        public Drink()
        {
            Name = string.Empty;
            Volume = 1;
            Degree = 0;
        }


    }
}
