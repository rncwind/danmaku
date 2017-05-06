using System;

namespace mgtsrw
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
            string input;
            input = Microsoft.VisualBasic.Interaction.InputBox("Chose game","Chose game");

            if (input == "1")
            {
                using (var game = new Game1())
                    game.Run();
            }

            else if (input == "2")
            {
                using (var game2 = new Game2())
                    game2.Run();
            }

            else if (input == "rewrite1")
            {
            */
                using (var Game1_rw = new Game1_rw())
                {
                    Game1_rw.Run();
                }
            //}
        }
    }
#endif
}
