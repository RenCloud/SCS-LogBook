using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiteDB;
using NLog;
using SCSSdkClient;
using SCSSdkClient.Object;
using SCS_LogBook.Objects;
using Logger = NLog.Logger;
using LiveCharts;
using LiveCharts.Configurations;
//Core of the library
using LiveCharts.Wpf;     //The WPF controls
using LiveCharts.WinForms; //the WinForm wrappers
using LiveCharts.Configurations;

namespace SCS_LogBook
{
    public partial class LogBook : Form {
        #region Database Stuff
        private string _path = "Data/";
        private LiteDatabase _data;

        #endregion

        #region graph stuff
        public ChartValues<MeasureModel> speedChart { get; set; }

        #endregion  

        private readonly Account _account;
        private SCSSdkTelemetry _telemetry;
        private long lastTime;
        private List<LogEntry> toSave;
        private int valueCounter => toSave.Count;
        private int dataFetch = 0;
        private int dataFetchAmount = 4;
        private int saveTime = 60;

        /// <summary>
        /// Database entry rate
        /// </summary>
        private long span = 250;
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Create a Logbook view/Window
        /// </summary>
        /// <param name="account">Account of the Logbook</param>
        public LogBook(Account account) {
            _account = account;
            this.toSave = new List<LogEntry>(saveTime);
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
                .X(model => model.DateTime.Ticks)   //use DateTime.Ticks as X
                .Y(model => model.Value);           //use the value property as Y

            //lets save the mapper globally.
            Charting.For<MeasureModel>(mapper);

            //the speedChart property will store our values array
            speedChart = new ChartValues<MeasureModel>();
            cartesianChart1.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Values = speedChart,
                    PointGeometrySize = 5,
                    StrokeThickness = 1
                }
            };
            cartesianChart1.AxisX.Add(new Axis
            {
                DisableAnimations = true,
                LabelFormatter = value => new DateTime((long)value).ToString("mm:ss"),
                Separator = new Separator
                {
                    Step = TimeSpan.FromSeconds(5).Ticks
                }
            });

            SetAxisLimits(DateTime.Now);

            
            
        }
       
       
         
        
       
        private void SetAxisLimits(DateTime now)
        {
            cartesianChart1.AxisX[0].MaxValue = now.Ticks + TimeSpan.FromSeconds(1).Ticks; // lets force the axis to be 100ms ahead
            cartesianChart1.AxisX[0].MinValue = now.Ticks - TimeSpan.FromSeconds(30).Ticks; //we only care about the last 60 seconds
        }

       
        public sealed override string Text {
            get => base.Text;
            set => base.Text = value;
        }
        private void TelemetryOnJobFinished(object sender, EventArgs args) {
            // TODO: Fill this and also may have to recheck SDK Field because of Trailer ownership
        }

        private void TelemetryOnJobStarted(object sender, EventArgs e) {
            // TODO: Fill this and also may have to recheck SDK Field because of Trailer ownership
        }

        private void LogBook_Load(object sender, EventArgs e)
        {
            _data = new LiteDatabase(_path+_account.Name+"/logbook.lite");
        }

  

        private void Telemetry_Data(SCSTelemetry data, bool updated) {
            try
            {
                 
                if (InvokeRequired)
                {
                    Invoke(new TelemetryData(Telemetry_Data), data, updated);
                    return;
                }

                if (data.Paused) {

                    // TODO: SAVE DATA AND SET LAST TIME OVER AND OVER AGAIN
                    return;
                }
                if (DateTime.Now.Ticks - lastTime>= TimeSpan.FromMilliseconds(span).Ticks ) {
                    lastTime = DateTime.Now.Ticks;
                    var now = System.DateTime.Now;
                    speedChart.Add(new MeasureModel
                                    {
                                        DateTime = now,
                                        Value =  data.TruckValues.CurrentValues.DashboardValues.Speed.Kph
                                    });

                    SetAxisLimits(now);

                    //lets only use the last 30 values
                    if (speedChart.Count > 130) speedChart.RemoveAt(0);
// TODO:
// need here to save the stuff we need, but not every 250ms 
                    // save it in an list at save it every time game is paused or after 30 secs? so 120 values like the live panel
                    dataFetch++;
                    if (dataFetch == dataFetchAmount) {
                        dataFetch = 0;
                        toSave.Add(new LogEntry(data));
                        if (valueCounter == saveTime) {
                            Console.WriteLine("SAAAAAVEEE");
                            //TODO: SAVE IT
                            toSave.Clear();
                        }
                    }

                }
               

                




            }
            catch(Exception ex)
            {
                Log.Error(ex);
            }

        }

        private void LogBook_FormClosing(object sender, FormClosingEventArgs e) {
            if (!closing) {
                // TODO: Make it possible to change logbook -> that means here we have to check if it a close or change  
                // TODO: still not working as expected, but when running longer it works Ô.o
                if (MessageBox.Show("Are you sure you want to quit?", "SCS-Logbook", MessageBoxButtons.YesNo) ==
                    DialogResult.No) {
                    e.Cancel = true;
                    return;
                }

                _telemetry.Dispose();
                Log.Debug("LogBook Closed");
                closing = true; 
            }  
                Application.Exit();
          
        }

        private bool closing = false;

    }
    public class MeasureModel
    {
        public DateTime DateTime { get; set; }
        public double Value { get; set; }
    }
}
