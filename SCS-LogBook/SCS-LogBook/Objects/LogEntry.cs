using System.Runtime.InteropServices;

namespace SCS_LogBook.Objects {
    public class LogEntry {
        public uint GameTime { get; internal set; }
        public long RealTime { get; internal set; }







        #region Job
        public string CityDestination { get; internal set; }
        public string CitySource { get; internal set; }

        public string CompanyDestination { get; internal set; }

        public string CompanySource { get; internal set; }

        public long Income { get; internal set; }



        #endregion

        #region Navigation
        public float SpeedLimit { get; internal set; }
        #endregion

        #region Trailer
        public float TrailerMass { get; internal set; }
        public float TrailerDamge { get; internal set; }
        public string TrailerName { get; internal set; }
        public string Chassis { get; internal set; }

        #endregion

        #region Truck
        public float CabinDamage { get; internal set; }
        public float ChassisDamage { get; internal set; }
        public float Engine { get; internal set; }
        public float Transmission { get; internal set; }
        public float WheelsAvg { get; internal set; }

        public float AdBlue { get; internal set; }
        public float Fuel { get; internal set; }
        public float Odometer { get; internal set; }

        #endregion
    }
}