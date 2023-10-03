using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalCMS
{
    public class Temperature
    {
        private double _Celsius = 0;
        public double Celsius
        {
            get { return _Celsius; }
            set
            {
                _Fahrenheit = (value * 9 / 5) + 32;
                _Kelvin = value + 273.15;
            }
        }
        private double _Fahrenheit = 0;
        public double Fahrenheit
        {
            get { return _Fahrenheit; }
            set
            {
                _Celsius = (value - 32) * 5 / 9;
                _Kelvin = _Celsius + 273.15;
            }
        }
        private double _Kelvin = 0;
        public double Kelvin
        {
            get { return _Kelvin; }
            set
            {
                _Celsius = value - 273.15;
                _Fahrenheit = (_Celsius * 9 / 5) + 32;
            }
        }


        public static Temperature FromKelvin(double kelvin)
        {
            var temperature = new Temperature();
            temperature.Kelvin = kelvin;
            return temperature;
        }

        public static Temperature FromFahrenheit(double fahrenheit)
        {
            var temperature = new Temperature();
            temperature.Fahrenheit = fahrenheit;
            return temperature;
        }

        public static Temperature FromCelsius(double celsius)
        {
            var temperature = new Temperature();
            temperature.Celsius = celsius;
            return temperature;
        }

    }
}
