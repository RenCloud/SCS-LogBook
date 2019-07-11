using SCSSdkClient.Object;

namespace SCS_LogBook.Objects {
    /// <summary>
    ///     LogEntry holds data that is logged in interval to analyse later.
    ///     For now it logs the whole object. May change this later when it's to big.
    /// </summary>
    public class LogEntry {
        /// <summary>
        ///     Create a new LogEntry with SCSTelemetry data.
        /// </summary>
        /// <param name="data"></param>
        public LogEntry(SCSTelemetry data) => Data = data;

        /// <summary>
        ///     Database ID 
        /// </summary>
        public int ID { get; set; }

        // TODO: check size if too big it need to be cutted
        /// <summary>
        ///     Game Data (Complete atm)
        /// </summary>
        public SCSTelemetry Data { get; set; }
    }
}