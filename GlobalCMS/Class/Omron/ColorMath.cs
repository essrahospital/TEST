using System;
using System.Drawing;

namespace GlobalCMS
{
    public static class ColorMath
    {
        public static Color Blend(Color color1, Color color2, double ratio)
        {
            int alpha = (int)Math.Round((double)color1.A * (1.0 - ratio) + (double)color2.A * ratio);
            int num1 = (int)Math.Round((double)color1.R * (1.0 - ratio) + (double)color2.R * ratio);
            int num2 = (int)Math.Round((double)color1.G * (1.0 - ratio) + (double)color2.G * ratio);
            int num3 = (int)Math.Round((double)color1.B * (1.0 - ratio) + (double)color2.B * ratio);
            int red = num1;
            int green = num2;
            int blue = num3;
            return Color.FromArgb(alpha, red, green, blue);
        }

        public static Color Darken(Color color, double ratio)
        {
            return ColorMath.Blend(color, Color.Black, ratio);
        }

        public static Color Lighten(Color color, double ratio)
        {
            return ColorMath.Blend(color, Color.White, ratio);
        }

        public static HslColor RgbToHsl(Color rgb)
        {
            double val1 = (double)rgb.R / (double)byte.MaxValue;
            double val2_1 = (double)rgb.G / (double)byte.MaxValue;
            double val2_2 = (double)rgb.B / (double)byte.MaxValue;
            double num1 = Math.Min(Math.Min(val1, val2_1), val2_2);
            double num2 = Math.Max(Math.Max(val1, val2_1), val2_2);
            double num3 = (num2 + num1) / 2.0;
            double num4 = num2 != num1 ? (num2 != val1 ? (num2 != val2_1 ? (60.0 * (val1 - val2_1) / (num2 - num1) + 240.0) % 360.0 : (60.0 * (val2_2 - val1) / (num2 - num1) + 120.0) % 360.0) : 60.0 * (val2_1 - val2_2) / (num2 - num1) % 360.0) : 0.0;
            if (num4 < 0.0)
                num4 += 360.0;
            double num5 = num2 != num1 ? (num3 > 0.5 ? (num2 - num1) / (2.0 - 2.0 * num3) : (num2 - num1) / (2.0 * num3)) : 0.0;
            return new HslColor((byte)Math.Round(num4 / 360.0 * 256.0 % 256.0), (byte)Math.Round(num5 * (double)byte.MaxValue), (byte)Math.Round(num3 * (double)byte.MaxValue));
        }

        public static Color HslToRgb(HslColor hsl)
        {
            double num1 = (double)hsl.H / 256.0;
            double num2 = (double)hsl.S / (double)byte.MaxValue;
            double num3 = (double)hsl.L / (double)byte.MaxValue;
            double num4 = num3 >= 0.5 ? num3 + num2 - num3 * num2 : num3 * (1.0 + num2);
            double num5 = 2.0 * num3 - num4;
            double[] numArray1 = new double[3]
            {
        num1 + 1.0 / 3.0,
        num1,
        num1 - 1.0 / 3.0
            };
            byte[] numArray2 = new byte[3];
            for (int index = 0; index < 3; ++index)
            {
                if (numArray1[index] < 0.0)
                    ++numArray1[index];
                if (numArray1[index] > 1.0)
                    --numArray1[index];
                numArray2[index] = numArray1[index] >= 1.0 / 6.0 ? (numArray1[index] >= 0.5 ? (numArray1[index] >= 2.0 / 3.0 ? (byte)Math.Round(num5 * (double)byte.MaxValue) : (byte)Math.Round((num5 + (num4 - num5) * 6.0 * (2.0 / 3.0 - numArray1[index])) * (double)byte.MaxValue)) : (byte)Math.Round(num4 * (double)byte.MaxValue)) : (byte)Math.Round((num5 + (num4 - num5) * 6.0 * numArray1[index]) * (double)byte.MaxValue);
            }
            return Color.FromArgb((int)numArray2[0], (int)numArray2[1], (int)numArray2[2]);
        }

        private static int Mod(int dividend, int divisor)
        {
            if (divisor <= 0)
                throw new ArgumentOutOfRangeException(nameof(divisor), "The divisor cannot be zero or negative.");
            int num = dividend % divisor;
            if (num < 0)
                num += divisor;
            return num;
        }

        public static byte ToGray(Color c)
        {
            return (byte)((double)c.R * 0.3 + (double)c.G * 0.59 + (double)c.B * 0.11);
        }

        public static bool IsDarkColor(Color c)
        {
            return ColorMath.ToGray(c) < (byte)144;
        }
    }
}
