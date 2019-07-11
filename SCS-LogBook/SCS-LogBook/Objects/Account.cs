namespace SCS_LogBook.Objects {
    /// <summary>
    ///     Holds data of an account of the logbook.
    /// </summary>
    public class Account {
        /// <summary>
        ///     Empty constructor
        /// </summary>
        public Account() { }

        /// <summary>
        ///     Init a new Account with given parameter
        /// </summary>
        /// <param name="name">Name of the new account</param>
        /// <param name="game">Game for this account (ETS2 or ATS)</param>
        /// <param name="playTime">Init playTime</param>
        /// <param name="inGameTime">Init inGameTime</param>
        /// <param name="miles">Init miles</param>
        public Account(string name, SCSGame game, double playTime = 0, double inGameTime = 0, double miles = 0) {
            Name = name;
            Game = game;
            PlayTime = playTime;
            InGameTime = inGameTime;
            Miles = miles;
        }

        /// <summary>
        ///     Name of the account
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Game of the account (ETS2 or ATS)
        /// </summary>
        public SCSGame Game { get; set; }

        /// <summary>
        ///     Data Base Identifier
        /// </summary>
        public int id { get; set; }

        /// <summary>
        ///     PlayTime of this account (real time). In hours.
        /// </summary>
        public double PlayTime { get; set; }

        /// <summary>
        ///     InGameTime of this account (game time). In inGame hours.
        /// </summary>
        public double InGameTime { get; set; }

        /// <summary>
        ///     Miles driven in game of the account. In km.
        /// </summary>
        public double Miles { get; set; }
    }
}