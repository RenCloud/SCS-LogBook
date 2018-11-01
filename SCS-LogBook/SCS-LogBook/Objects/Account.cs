namespace SCS_LogBook.Objects {
    public class Account {
        public Account()
        {
        }

        public Account(string name, SCSGame game, double playTime, double inGameTime, double miles)
        {
            Name = name;
            Game = game;
            PlayTime = playTime;
            InGameTime = inGameTime;
            Miles = miles;
        }

        public string Name { get; set; }
        public SCSGame Game { get; set; }
        /// <summary>
        /// Data Base Identifier
        /// </summary>
        public int id { get; set; }
        public double PlayTime { get; set; }
        public double InGameTime { get; set; }
        public double Miles { get; set; }
        

    }
}