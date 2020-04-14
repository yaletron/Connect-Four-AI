using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;


namespace ConnectFourAI
{
    public class Core
    {
        // debug build
        protected static readonly bool debugBuild = false;
        // running? if false quit program
        public static bool running = true;
        // game board details
        protected static readonly SlotState playerDC = SlotState.Red;
        protected static readonly SlotState aiDC = SlotState.Yellow;
        protected static readonly int slotRows = 6;
        protected static readonly int slotCollumns = 7;
        // last player selected column
        public static int lFCSel = 0;
        public static int collumnSelected = 0;
        public static int slotTotalSpaces = slotRows * slotCollumns;
        public static IList<SlotState> boardAreaState = new List<SlotState>();
        // board area analyse state
        public static IList<string> bAAS = new List<string>();
        public static IList<string> bAASE = new List<string>();
        // # of chips placed by either player in a collumn for easy ref
        public static IList<int> chipsPlacedInCollumn = new List<int>();
        // art and sentences to display
        public static IList<string> art = new List<string>() { " - ", "(R)", "[Y]", " | ", " v ", " ? ", " * " };
        public static IList<string> gameStateDesc = new List<string>() { "Press 'Enter' To Play Connect Four! 'Esc' To Quit.", "Use Left And Right Arrow Keys And 'Enter' To Place Red Chip. 'Esc' To Quit.", "AI Is Placing Yellow Chip.", "Red Has Made Connect Four! Press 'Enter' To Play Again. 'Esc' To Quit.", "Yellow Has Made Connect Four! Press 'Enter' To Play Again. 'Esc' To Quit.", "Draw! Press 'Enter' To Play Again. 'Esc' To Quit." };
        // wait durations
        public static int inputRepeatWaitMili = 2000;
        public static int sDurMili = 140;


        static void Main()
        {
            GSM.SetUp();
            while (running)
            {
            }
            return;
        }

        // state of a chip area labels
        public enum SlotState
        {
            Empty,
            Red,
            Yellow,
            DownDrop,
            DownArrow,
            Question,
            Star,
            OffBoard,
            None
        }

        public static IList<int> slotStateColors = new List<int>();
        public static readonly int colorBlue = 3;
        public static readonly int colorYellow = 6;
        public static readonly int colorRed = 4;
        public static readonly int colorWhite = 15;
        public static readonly int colorGrey = 8;

        // direction labels
        public enum DirLabel
        {
            Up,
            UpRight,
            Right,
            DownRight,
            Down,
            DownLeft,
            Left,
            UpLeft,
            None

        }
    }
}
