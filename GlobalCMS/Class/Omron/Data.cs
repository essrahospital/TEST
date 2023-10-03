namespace GlobalCMS
{
    internal class Data
    {
        public int avg_cnt;
        public int avg_num;
        public double latest;
        public double avg;
        public double avg_sum;
        public double[] avg_array;
        public bool round;

        public Data()
        {
            this.round = false;
            this.avg_num = 1;
            this.avg_cnt = 0;
            this.latest = 0.0;
            this.avg = 0.0;
            this.avg_sum = 0.0;
            this.avg_array = new double[1];
        }

        public Data(int avgNum)
        {
            this.round = false;
            this.avg_num = avgNum;
            this.avg_cnt = 0;
            this.latest = 0.0;
            this.avg = 0.0;
            this.avg_sum = 0.0;
            this.avg_array = new double[avgNum];
        }

        public void resetAvg(int avgNum)
        {
            this.round = false;
            this.avg_num = avgNum;
            this.avg_cnt = 0;
            this.avg_sum = 0.0;
            this.avg_array = new double[avgNum];
        }

        public void movingAverage()
        {
            ++this.avg_cnt;
            if (this.avg_cnt >= this.avg_num)
            {
                this.avg_cnt = 0;
                this.round = true;
            }
            this.avg_sum -= this.avg_array[this.avg_cnt];
            this.avg_array[this.avg_cnt] = this.latest;
            this.avg_sum += this.latest;
            if (this.round)
                this.avg = this.avg_sum / (double)this.avg_num;
            else
                this.avg = this.avg_sum / (double)this.avg_cnt;
        }
    }
}
