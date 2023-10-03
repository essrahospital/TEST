using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GlobalCMS
{
    public class ColorWheel : Control
    {
        private Bitmap wheelBitmap;
        private Bitmap slBitmap;
        private byte hue;
        private byte saturation;
        private byte lightness;
        private byte[] secondaryHues;
        private bool draggingHue;
        private bool draggingSL;

        public event EventHandler HueChanged;

        public event EventHandler SLChanged;

        public byte Hue
        {
            get
            {
                return this.hue;
            }
            set
            {
                if ((int)value == (int)this.hue)
                    return;
                this.hue = value;
                this.PrepareSLBitmap();
                this.Invalidate();
            }
        }

        public byte Saturation
        {
            get
            {
                return this.saturation;
            }
            set
            {
                if ((int)value == (int)this.saturation)
                    return;
                this.saturation = value;
                this.Invalidate();
            }
        }

        public byte Lightness
        {
            get
            {
                return this.lightness;
            }
            set
            {
                if ((int)value == (int)this.lightness)
                    return;
                this.lightness = value;
                this.Invalidate();
            }
        }

        public byte[] SecondaryHues
        {
            get
            {
                return this.secondaryHues;
            }
            set
            {
                if (value == null != (this.secondaryHues == null) || value == null || value != null && value.Length != this.secondaryHues.Length)
                {
                    this.secondaryHues = value;
                    this.Invalidate();
                }
                else
                {
                    if (value == null)
                        return;
                    for (int index = 0; index < value.Length; ++index)
                    {
                        if ((int)value[index] != (int)this.secondaryHues[index])
                        {
                            this.secondaryHues = value;
                            this.Invalidate();
                            break;
                        }
                    }
                }
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool TabStop
        {
            get
            {
                return base.TabStop;
            }
            set
            {
                base.TabStop = false;
            }
        }

        public ColorWheel()
        {
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.DoubleBuffered = true;
            this.TabStop = false;
            this.PrepareWheelBitmap();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            this.PrepareWheelBitmap();
            this.PrepareSLBitmap();
            base.OnSizeChanged(e);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (this.wheelBitmap != null)
                pe.Graphics.DrawImage((Image)this.wheelBitmap, new Point());
            if (this.slBitmap != null)
                pe.Graphics.DrawImage((Image)this.slBitmap, new Point(this.slBitmap.Width / 2, this.slBitmap.Width / 2));
            pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            double num1 = (double)this.hue / 128.0 * Math.PI + Math.PI / 2.0;
            double num2 = 0.89 * (double)this.wheelBitmap.Width / 2.0;
            int num3 = (int)Math.Round(num2 * Math.Cos(num1));
            int num4 = (int)-Math.Round(num2 * Math.Sin(num1));
            int num5 = num3 + this.wheelBitmap.Width / 2;
            int num6 = num4 + this.wheelBitmap.Width / 2;
            using (Pen pen = new Pen(ColorMath.ToGray(ColorMath.HslToRgb(new HslColor(this.hue, byte.MaxValue, (byte)128))) > (byte)128 ? Color.Black : Color.White))
                pe.Graphics.DrawEllipse(pen, num5 - 3, num6 - 3, 6, 6);
            if (this.secondaryHues != null)
            {
                foreach (int secondaryHue in this.secondaryHues)
                {
                    int num7;
                    double num8 = (double)(num7 = secondaryHue) / 128.0 * Math.PI + Math.PI / 2.0;
                    double num9 = 0.89 * (double)this.wheelBitmap.Width / 2.0;
                    int num10 = (int)Math.Round(num9 * Math.Cos(num8));
                    int num11 = (int)-Math.Round(num9 * Math.Sin(num8));
                    int num12 = num10 + this.wheelBitmap.Width / 2;
                    int num13 = num11 + this.wheelBitmap.Width / 2;
                    using (Brush brush = (Brush)new SolidBrush(Color.FromArgb(128, ColorMath.ToGray(ColorMath.HslToRgb(new HslColor((byte)num7, byte.MaxValue, (byte)128))) > (byte)128 ? Color.Black : Color.White)))
                        pe.Graphics.FillRectangle(brush, num12 - 2, num13 - 2, 4, 4);
                }
            }
            int num14 = this.slBitmap.Width / 2 + (int)this.saturation * (this.slBitmap.Width - 1) / (int)byte.MaxValue;
            int num15 = this.slBitmap.Width / 2 + (int)this.lightness * (this.slBitmap.Width - 1) / (int)byte.MaxValue;
            using (Pen pen = new Pen(ColorMath.ToGray(ColorMath.HslToRgb(new HslColor(this.hue, this.saturation, this.lightness))) > (byte)128 ? Color.Black : Color.White))
                pe.Graphics.DrawEllipse(pen, num14 - 3, num15 - 3, 6, 6);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int num1 = this.slBitmap.Width / 2;
                if (e.X >= num1 && e.X < num1 * 3 && (e.Y >= num1 && e.Y < num1 * 3))
                {
                    this.draggingSL = true;
                    this.OnMouseMove(e);
                }
                else
                {
                    int num2 = this.wheelBitmap.Width / 2;
                    Point b = new Point(num2, num2);
                    double distance = this.GetDistance(new Point(e.X, e.Y), b);
                    if (distance >= (double)num2 * 0.78 && distance < (double)num2)
                    {
                        this.draggingHue = true;
                        this.OnMouseMove(e);
                    }
                }
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            this.draggingSL = false;
            this.draggingHue = false;
            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (this.draggingSL)
                {
                    int num = this.slBitmap.Width / 2;
                    this.Saturation = (byte)Math.Max(0, Math.Min((int)byte.MaxValue, (e.X - num) * (int)byte.MaxValue / this.slBitmap.Width));
                    this.Lightness = (byte)Math.Max(0, Math.Min((int)byte.MaxValue, (e.Y - num) * (int)byte.MaxValue / this.slBitmap.Width));
                    this.OnSLChanged();
                }
                else if (this.draggingHue)
                {
                    int num = this.wheelBitmap.Width / 2;
                    Point point = new Point(num, num);
                    this.Hue = (byte)ColorWheel.Mod((int)(-(128.0 / Math.PI) * Math.Atan2((double)(e.Y - point.Y), (double)(e.X - point.X)) + 192.0), 256);
                    this.OnHueChanged();
                }
            }
            base.OnMouseMove(e);
        }

        private void PrepareWheelBitmap()
        {
            if (this.wheelBitmap != null)
                this.wheelBitmap.Dispose();
            int num1 = Math.Min(this.ClientSize.Width, this.ClientSize.Height);
            Point b = new Point(num1 / 2, num1 / 2);
            if (num1 < 10)
            {
                this.wheelBitmap = (Bitmap)null;
            }
            else
            {
                this.wheelBitmap = new Bitmap(num1, num1);
                using (Brush brush = (Brush)new SolidBrush(Color.Transparent))
                    Graphics.FromImage((Image)this.wheelBitmap).FillRectangle(brush, 0, 0, num1, num1);
                double num2 = (double)(num1 / 2) * 0.78;
                double num3 = (double)(num1 / 2 - 1);
                double num4 = 128.0 / Math.PI;
                byte[] bytes;
                BitmapData bmData;
                this.BitmapReadBytes(this.wheelBitmap, out bytes, out bmData);
                for (int y = 0; y < num1; ++y)
                {
                    for (int x = 0; x < num1; ++x)
                    {
                        double distance = this.GetDistance(new Point(x, y), b);
                        byte num5 = distance >= num2 - 0.5 ? (distance >= num2 + 0.5 ? (distance >= num3 - 0.5 ? (distance >= num3 + 0.5 ? (byte)0 : (byte)((0.5 + num3 - distance) * (double)byte.MaxValue)) : byte.MaxValue) : (byte)((0.5 - num2 + distance) * (double)byte.MaxValue)) : (byte)0;
                        if (num5 > (byte)0)
                        {
                            double num6 = Math.Atan2((double)(y - b.Y), (double)(x - b.X));
                            byte h = (byte)ColorWheel.Mod((int)(-num4 * num6 + 192.0), 256);
                            Color c = Color.FromArgb((int)num5, ColorMath.HslToRgb(new HslColor(h, byte.MaxValue, (byte)128)));
                            this.BitmapSetPixel(bytes, bmData, x, y, c);
                        }
                    }
                }
                this.BitmapWriteBytes(this.wheelBitmap, bytes, bmData);
            }
        }

        private void PrepareSLBitmap()
        {
            if (this.slBitmap != null)
                this.slBitmap.Dispose();
            int num = Math.Min(this.ClientSize.Width, this.ClientSize.Height) / 2;
            if (num < 10)
            {
                this.slBitmap = (Bitmap)null;
            }
            else
            {
                this.slBitmap = new Bitmap(num, num);
                byte[] bytes;
                BitmapData bmData;
                this.BitmapReadBytes(this.slBitmap, out bytes, out bmData);
                for (int y = 0; y < num; ++y)
                {
                    for (int x = 0; x < num; ++x)
                    {
                        Color rgb = ColorMath.HslToRgb(new HslColor(this.hue, (byte)(x * (int)byte.MaxValue / num), (byte)(y * (int)byte.MaxValue / num)));
                        this.BitmapSetPixel(bytes, bmData, x, y, rgb);
                    }
                }
                this.BitmapWriteBytes(this.slBitmap, bytes, bmData);
            }
        }

        private void BitmapReadBytes(Bitmap bmp, out byte[] bytes, out BitmapData bmData)
        {
            bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            bytes = new byte[bmp.Width * bmp.Height * 4];
            Marshal.Copy(bmData.Scan0, bytes, 0, bytes.Length);
        }

        private void BitmapSetPixel(byte[] bytes, BitmapData bmData, int x, int y, Color c)
        {
            int index = y * bmData.Stride + x * 4;
            bytes[index] = c.B;
            bytes[index + 1] = c.G;
            bytes[index + 2] = c.R;
            bytes[index + 3] = c.A;
        }

        private void BitmapWriteBytes(Bitmap bmp, byte[] bytes, BitmapData bmData)
        {
            Marshal.Copy(bytes, 0, bmData.Scan0, bytes.Length);
            bmp.UnlockBits(bmData);
        }

        private double GetDistance(Point a, Point b)
        {
            return Math.Sqrt((double)((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y)));
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

        protected void OnHueChanged()
        {
            if (this.HueChanged == null)
                return;
            this.HueChanged((object)this, EventArgs.Empty);
        }

        protected void OnSLChanged()
        {
            if (this.SLChanged == null)
                return;
            this.SLChanged((object)this, EventArgs.Empty);
        }
    }
}
