namespace GlobalCMS
{
    public struct HslColor
    {
        private byte h;
        private byte s;
        private byte l;

        public byte H
        {
            get
            {
                return this.h;
            }
            set
            {
                this.h = value;
            }
        }

        public byte S
        {
            get
            {
                return this.s;
            }
            set
            {
                this.s = value;
            }
        }

        public byte L
        {
            get
            {
                return this.l;
            }
            set
            {
                this.l = value;
            }
        }

        public HslColor(byte h, byte s, byte l)
        {
            this.h = h;
            this.s = s;
            this.l = l;
        }
    }
}
