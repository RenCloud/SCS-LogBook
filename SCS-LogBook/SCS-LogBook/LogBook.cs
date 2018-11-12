using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
        private string _path = "Data/";
        private LiteDatabase _data;
        private readonly Account _account;
        private SCSSdkTelemetry _telemetry;
      
        
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        public LogBook(Account account) {
            _account = account;
            InitializeComponent();
            Text = string.Format(LogBookCode.LogBook_LogBook_Logbook_of__0_, account.Name);
            
            _telemetry = new SCSSdkTelemetry();
            _telemetry.Data += Telemetry_Data;
            _telemetry.JobFinished += TelemetryOnJobFinished;
            _telemetry.JobStarted += TelemetryOnJobStarted;
            //To handle live data easily, in this case we built a specialized type
            //the MeasureModel class, it only contains 2 properties
            //DateTime and Value
            //We need to configure LiveCharts to handle MeasureModel class
            //The next code configures MEasureModel  globally, this means
            //that livecharts learns to plot MeasureModel and will use this config every time
            //a ChartValues instance uses this type.
            //this code ideally should only run once, when application starts is reccomended.
            //you can configure series in many ways, learn more at http://lvcharts.net/App/examples/v1/wpf/Types%20and%20Configuration

            var mapper = Mappers.Xy<MeasureModel>()
                .X(model => model.DateTime.Ticks)   //use DateTime.Ticks as X
                .Y(model => model.Value);           //use the value property as Y

            //lets save the mapper globally.
            Charting.For<MeasureModel>(mapper);

            //the ChartValues property will store our values array
            ChartValues = new ChartValues<MeasureModel>();
            cartesianChart1.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Values = ChartValues,
                    PointGeometrySize = 5,
                    StrokeThickness = 1
                }
            };
            cartesianChart1.AxisX.Add(new Axis
            {
                DisableAnimations = true,
                LabelFormatter = value => new System.DateTime((long)value).ToString("mm:ss"),
                Separator = new Separator
                {
                    Step = TimeSpan.FromSeconds(30).Ticks
                }
            });

            SetAxisLimits(System.DateTime.Now);

            //The next code simulates data changes every 500 ms
            
        }
        public ChartValues<MeasureModel> ChartValues { get; set; }
       
        public Random R { get; set; } 
        private long lastTime;
        private long span = 250;
        private void SetAxisLimits(System.DateTime now)
        {
            cartesianChart1.AxisX[0].MaxValue = now.Ticks + TimeSpan.FromSeconds(1).Ticks; // lets force the axis to be 100ms ahead
            cartesianChart1.AxisX[0].MinValue = now.Ticks - TimeSpan.FromSeconds(60).Ticks; //we only care about the last 8 seconds
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

                if (DateTime.Now.Ticks - lastTime>= TimeSpan.FromMilliseconds(span).Ticks ) {
                    lastTime = DateTime.Now.Ticks;
                    var now = System.DateTime.Now;
                    ChartValues.Add(new MeasureModel
                                    {
                                        DateTime = now,
                                        Value =  data.TruckValues.CurrentValues.DashboardValues.Speed.Kph
                                    });

                    SetAxisLimits(now);

                    //lets only use the last 30 values
                    if (ChartValues.Count > 1200) ChartValues.RemoveAt(0);

                }
               

                // TODO:




            }
            catch(Exception ex)
            {
                Log.Error(ex);
            }

        }

        private void LogBook_FormClosing(object sender, FormClosingEventArgs e) {
            if (!closing) {
                // TODO: Make it possible to change logbook -> that means here we have to check if it a close or change  
                // TODO: Not nice but works,... may change that later,, but can be used to implement an changer
                if (MessageBox.Show("Are you sure you want to quit?", "SCS-Logbook", MessageBoxButtons.YesNo) ==
                    DialogResult.No) {
                    e.Cancel = true;
                    return;
                }

                _telemetry.Dispose();
                Log.Debug("LogBook Closed");
                closing = true;
                Application.Exit();
            } else {
                Application.Exit();
            }
        }

        private bool closing = false;

    }
    public class MeasureModel
    {
        public System.DateTime DateTime { get; set; }
        public double Value { get; set; }
    }
}
