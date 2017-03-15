using System;


namespace moregameteststuff
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            /*
            using (var game = new Game1())
                game.Run();
        using (var game2 = new Game2())
            game2.Run();
            */
            using (var Game1_rw = new Game1_rw())
            {
                Game1_rw.Run();
            }
        }
    }
#endif
}
