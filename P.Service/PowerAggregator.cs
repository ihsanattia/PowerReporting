using Services;

namespace P.Service
{
    public class PowerAggregator
    {
        #region Properties
        public double[] TotalVolumes { get; private set; }
        public DateTime Timestamp { get; private set; }
        #endregion Fields

        #region Constructor
        public PowerAggregator(DateTime timestamp_)
        {
            Timestamp = timestamp_;
            TotalVolumes = new double[24];
        }
        #endregion Constructor

        #region Methods
        private void Add(int ind, double val)
        {
            TotalVolumes[ind] += val;
        }

        public void Aggregate(IEnumerable<PowerTrade> trades)
        {
            foreach (PowerTrade trade in trades)
            {
                foreach (PowerPeriod p in trade.Periods)
                {
                    Add(p.Period - 1, p.Volume);
                }
            }
        }
        #endregion Methods
    }
}
