using System;
using System.Runtime.InteropServices;
using SCSSdkClient.Object;

namespace SCS_LogBook.Objects {
    public class LogEntry {
        public LogEntry(SCSTelemetry data)
        {
            // TODO
            GameTime = data.CommonValues.GameTime.Value;
            RealTime = DateTime.Now.Ticks;
            CityDestination = data.JobValues.CityDestination;
            CitySource = data.JobValues.CitySource;
            CompanyDestination = data.JobValues.CompanyDestination;
            CompanySource = data.JobValues.CompanySource;
            Income = data.JobValues.Income;
            SpeedLimit = data.NavigationValues.SpeedLimit;
            
            CabinDamage = data.TruckValues.CurrentValues.DamageValues.Cabin;
            ChassisDamage = data.TruckValues.CurrentValues.DamageValues.Chassis;
            Engine = data.TruckValues.CurrentValues.DamageValues.Engine;
            Transmission = data.TruckValues.CurrentValues.DamageValues.Transmission;
            WheelsAvg = data.TruckValues.CurrentValues.DamageValues.WheelsAvg;
            AdBlue = data.TruckValues.CurrentValues.DashboardValues.AdBlue;
            Fuel = data.TruckValues.CurrentValues.DashboardValues.FuelValue.Amount;
            Odometer = data.TruckValues.CurrentValues.DashboardValues.Odometer;
        }

        public uint GameTime { get; internal set; }
        public long RealTime { get; internal set; }







        #region Job
        public string CityDestination { get; internal set; }
        public string CitySource { get; internal set; }

        public string CompanyDestination { get; internal set; }

        public string CompanySource { get; internal set; }

        public ulong Income { get; internal set; }



        #endregion

        #region Navigation
        public SCSTelemetry.Movement SpeedLimit { get; internal set; }
        #endregion

        #region Trailer
        public float TrailerMass { get; internal set; }
        public float TrailerDamage { get; internal set; }
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