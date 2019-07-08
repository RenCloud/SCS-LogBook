using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using LiteDB;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using NLog;
using SCSSdkClient;
using SCSSdkClient.Object;
using SCS_LogBook.Objects;
using CartesianChart = LiveCharts.WinForms.CartesianChart;
using Logger = NLog.Logger;

//TODO: Create SETTINGS FOR LIVE GRAPHS (DISABLE SIZE ETC)
//TODO: SET MIN OF GRAPHS TO 0 
//TODO: THINK ABOUT MORE LIVE CHARTS (ODOMETER, Gesamt FUEL, GEesamt way, Time , etc.)
//TODO: Check basic CPU AND RAM NEEDINGS and after adding settings to give a ca. requirement 
namespace SCS_LogBook {
    /// <summary>
    ///     Logbook for a account
    ///     After initialization logging start immediately
    /// </summary>
    public partial class LogBook : Form {
        /// <summary>
        ///     How often is the data refreshed until we saved it?
        ///     so _dataFetch*span == SaveTime
        /// </summary>
        private const int DataFetchAmount = 2;

        /// <summary>
        ///     How many time/entries until we write savelist to db
        /// </summary>
        private const int SaveTime = 120;

        /// <summary>
        ///     Graph entry rate
        /// </summary>
        private const long Span = 500;

        /// <summary>
        ///     Logger Instance for logging purpose
        /// </summary>
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Account of the Logbook
        /// </summary>
        private readonly Account _account;

        /// <summary>
        ///     List to create the avg fuel live panel
        /// </summary>
        private readonly List<double> _fuel;

        /// <summary>
        ///     List to create the avg fuel live panel
        /// </summary>
        private readonly List<double> _odoMeter;

        /// <summary>
        ///     Telemetry object
        /// </summary>
        private readonly SCSSdkTelemetry _telemetry;

        /// <summary>
        ///     Contains entries that will be written to db in the next phase
        /// </summary>
        private readonly List<LogEntry> _toSave;

        /// <summary>
        ///     Used to differentiate between closing and account changing
        /// </summary>
        private bool _closing;

        /// <summary>
        ///     We don't want to save data in the same rate as we update the live panels
        ///     This counter counts the times we don't save the data
        /// </summary>
        private int _dataFetch;

        /// <summary>
        ///     Displays the Time we refreshed last time
        /// </summary>
        private long _lastTime;

        /// <summary>
        ///     Create a Logbook view/Window
        /// </summary>
        /// <param name="account">Account of the Logbook</param>
        public LogBook(Account account) {
            _account = account;
            _toSave = new List<LogEntry>(SaveTime);
            InitializeComponent();
            Text = string.Format(LogBookCode.LogBook_LogBook_Logbook_of__0_, account.Name);

            // init the telemetry to start getting data
            _telemetry = new SCSSdkTelemetry();
            _telemetry.Data += Telemetry_Data;

            // not used atm
            //_telemetry.JobFinished += TelemetryOnJobFinished;
            //_telemetry.JobStarted += TelemetryOnJobStarted;

            // Graph stuff need later
            //To handle live data easily, in this case we built a specialized type
            //the MeasureModel class, it only contains 2 properties
            //DateTime and Value
            //We need to configure LiveCharts to handle MeasureModel class
            //The next code configures MEasureModel  globally, this means
            //that livecharts learns to plot MeasureModel and will use this config every time
            //a speedChart instance uses this type.
            //this code ideally should only run once, when application starts is reccomended.
            //you can configure series in many ways, learn more at http://lvcharts.net/App/examples/v1/wpf/Types%20and%20Configuration
            // TODO: Create a function for later -> more graphics (live graphics)
            var mapper = Mappers.Xy<MeasureModel>()
                                .X(model => model.DateTime.Ticks) //use DateTime.Ticks as X
                                .Y(model => model.Value);         //use the value property as Y

            //lets save the mapper globally.
            Charting.For<MeasureModel>(mapper);

            //the speedChart property will store our values array
            speedChart = new ChartValues<MeasureModel>();
            fuelNeed = new ChartValues<MeasureModel>();
            odoMeters = new ChartValues<MeasureModel>();
            cartesianChart1.Series = new SeriesCollection {
                                                              new LineSeries {
                                                                                 Values = speedChart,
                                                                                 PointGeometrySize = 5,
                                                                                 StrokeThickness = 1
                                                                             }
                                                          };
            cartesianChart1.AxisX.Add(new Axis {
                                                   DisableAnimations = true,
                                                   LabelFormatter =
                                                       value => new DateTime((long) value).ToString("mm:ss"),
                                                   Separator = new Separator {
                                                                                 Step = TimeSpan.FromSeconds(5).Ticks
                                                                             }
                                               });
            cartesianChart2.Series = new SeriesCollection {
                                                              new LineSeries {
                                                                                 Values = fuelNeed,
                                                                                 PointGeometrySize = 5,
                                                                                 StrokeThickness = 1
                                                                             }
                                                          };
            cartesianChart2.AxisX.Add(new Axis {
                                                   DisableAnimations = true,
                                                   LabelFormatter =
                                                       value => new DateTime((long) value).ToString("mm:ss"),
                                                   Separator = new Separator {
                                                                                 Step = TimeSpan.FromSeconds(5).Ticks
                                                                             }
                                               });
            cartesianChart3.Series = new SeriesCollection {
                                                              new LineSeries {
                                                                                 Values = odoMeters,
                                                                                 PointGeometrySize = 5,
                                                                                 StrokeThickness = 1
                                                                             }
                                                          };
            cartesianChart3.AxisX.Add(new Axis {
                                                   DisableAnimations = true,
                                                   LabelFormatter =
                                                       value => new DateTime((long) value).ToString("mm:ss"),
                                                   Separator = new Separator {
                                                                                 Step = TimeSpan.FromSeconds(5).Ticks
                                                                             }
                                               });
            SetAxisLimits(DateTime.Now, cartesianChart1);
            SetAxisLimits(DateTime.Now, cartesianChart2);
            SetAxisLimits(DateTime.Now, cartesianChart3);
            solidGauge1.To = 120;
            solidGauge2.To = 1000; //TODO: SET TO FUEL SIZE
            _odoMeter = new List<double>(11);
            _fuel = new List<double>(11);
        }

        private int valueCounter => _toSave.Count;


        public sealed override string Text {
            get => base.Text;
            set => base.Text = value;
        }


        private void SetAxisLimits(DateTime now, CartesianChart cont) {
            cont.AxisX[0].MaxValue = now.Ticks + TimeSpan.FromSeconds(1).Ticks; // lets force the axis to be 100ms ahead
            cont.AxisX[0].MinValue =
                now.Ticks - TimeSpan.FromSeconds(60).Ticks; //we only care about the last 60 seconds
        }

        private void TelemetryOnJobFinished(object sender, EventArgs args) {
            // TODO: Fill this and also may have to recheck SDK Field because of Trailer ownership
        }

        private void TelemetryOnJobStarted(object sender, EventArgs e) {
            // TODO: Fill this and also may have to recheck SDK Field because of Trailer ownership
        }

        private void LogBook_Load(object sender, EventArgs e) =>
            _data = new LiteDatabase(_path + _account.Name + "/logbook.lite");


        private void Telemetry_Data(SCSTelemetry data, bool updated) {
            try {
                if (InvokeRequired) {
                    Invoke(new TelemetryData(Telemetry_Data), data, updated);
                    return;
                }

                if (data.Paused || data.DllVersion == 0) {
                    // TODO: SAVE DATA AND SET LAST TIME OVER AND OVER AGAIN
                    return;
                }

                if (DateTime.Now.Ticks - _lastTime >= TimeSpan.FromMilliseconds(Span).Ticks) {
                    _lastTime = DateTime.Now.Ticks;
                    var now = DateTime.Now;
                    _odoMeter.Add(data.TruckValues.CurrentValues.DashboardValues.Odometer);
                    _fuel.Add(data.TruckValues.CurrentValues.DashboardValues.FuelValue.Amount);
                    if (_odoMeter.Count > 10) {
                        _odoMeter.RemoveAt(0);
                        _fuel.RemoveAt(0);
                    }

                    speedChart.Add(new MeasureModel {
                                                        DateTime = now,
                                                        Value = data.TruckValues.CurrentValues.DashboardValues.Speed.Kph
                                                    });
                    // TODO: settings how long (how mutch values saved)
                    var tempTest = Math.Round(_odoMeter.Last() - _odoMeter[0], 2);
                    var tempTest2 = Math.Round(_fuel[0] - _fuel.Last(), 2);
                    var val = 0d;
                    if (tempTest != 0) {
                        val = tempTest2 / tempTest * 100;
                    }

                    fuelNeed.Add(new MeasureModel {
                                                      DateTime = now,
                                                      Value = val
                                                  });
                    odoMeters.Add(new MeasureModel {
                                                       DateTime = now,
                                                       Value =
                                                           Math
                                                               .Round(data.TruckValues.CurrentValues.DashboardValues.Odometer,
                                                                      2)
                                                   });
                    solidGauge1.Value = Math.Round(data.TruckValues.CurrentValues.DashboardValues.Speed.Kph, 2);
                    solidGauge2.Value = Math.Round(data.TruckValues.CurrentValues.DashboardValues.FuelValue.Amount, 2);
                    SetAxisLimits(now, cartesianChart1);
                    SetAxisLimits(now, cartesianChart2);
                    SetAxisLimits(now, cartesianChart3);
                    //lets only use the last 30 values
                    if (speedChart.Count > 250) {
                        speedChart.RemoveAt(0);
                    }

                    if (fuelNeed.Count > 250) {
                        fuelNeed.RemoveAt(0);
                    }

                    if (odoMeters.Count > 70250) {
                        odoMeters.RemoveAt(0);
                    }

                    label4.Text = Math.Round(data.TruckValues.CurrentValues.DashboardValues.Odometer, 2)
                                      .ToString(CultureInfo.CurrentCulture);
                    // TODO:
                    // need here to save the stuff we need, but not every 250ms 
                    // save it in an list at save it every time game is paused or after 30 secs? so 120 values like the live panel
                    _dataFetch++;
                    if (_dataFetch == DataFetchAmount) {
                        _dataFetch = 0;
                        _toSave.Add(new LogEntry(data));
                        if (valueCounter == SaveTime) {
                            Console.WriteLine("SAAAAAVEEE");

                            //TODO: SAVE IT
                            _toSave.Clear();
                        }
                    }
                }
            } catch (Exception ex) {
                Log.Error(ex);
            }
        }

        private void LogBook_FormClosing(object sender, FormClosingEventArgs e) {
            if (!_closing) {
                // TODO: Make it possible to change logbook -> that means here we have to check if it a close or change  
                // TODO: still not working as expected, but when running longer it works Ô.o
                if (MessageBox.Show("Are you sure you want to quit?", "SCS-Logbook", MessageBoxButtons.YesNo) ==
                    DialogResult.No) {
                    e.Cancel = true;
                    return;
                }

                _telemetry.Data -= Telemetry_Data;
                _telemetry.Dispose();
                Log.Debug("LogBook Closed");
                _closing = true;
            }

            Application.Exit();
        }

        #region Database Stuff

        private readonly string _path = "Data/";
        private LiteDatabase _data;

        #endregion

        #region graph stuff

        public ChartValues<MeasureModel> speedChart { get; set; }
        public ChartValues<MeasureModel> fuelNeed { get; set; }

        public ChartValues<MeasureModel> odoMeters { get; set; }

        #endregion
    }

    public class MeasureModel {
        public DateTime DateTime { get; set; }
        public double Value { get; set; }
    }
}