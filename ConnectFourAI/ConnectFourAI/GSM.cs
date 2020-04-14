using System;
using System.Threading;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;

namespace ConnectFourAI
{
    public class GSM : Core
    {
        public static GameState myGameState;
        public static GameState nextGameState = GameState.None;
        public enum GameState
        {
            Start,
            PlayerTurn,
            AITurn,
            Victory,
            Defeat,
            None,
            Draw
        }
        private static bool firstTurn = true;
        public static bool chipDropping = false;
        private static IList<int> connectFourLocs = new List<int>();

        public static void ConnectFourMade(int startLoc, int thisAxis, Vector2 thisAmountEa)
        {
            connectFourLocs.Clear();
            connectFourLocs.Add(startLoc);
            IList<DirLabel> theseDirs = new List<DirLabel>();

            switch (thisAxis)
            {
                case 0:
                    theseDirs.Add(DirLabel.Up);
                    theseDirs.Add(DirLabel.Down);
                    break;
                case 1:
                    theseDirs.Add(DirLabel.UpRight);
                    theseDirs.Add(DirLabel.DownLeft);
                    break;
                case 2:
                    theseDirs.Add(DirLabel.Right);
                    theseDirs.Add(DirLabel.Left);
                    break;
                case 3:
                    theseDirs.Add(DirLabel.DownRight);
                    theseDirs.Add(DirLabel.UpLeft);
                    break;
            }
            int toNextBoardArea = 0;

            for (int i = 0; i < 2; i++)
            {
                DirLabel dir = theseDirs[i];

                toNextBoardArea = MMath.ToNextInDir(dir);
                /*
                switch (dir)
                {
                    case DirLabel.Up:
                        toNextBoardArea = -slotCollumns;
                        break;
                    case DirLabel.UpRight:
                        toNextBoardArea = -slotCollumns + 1;
                        break;
                    case DirLabel.Right:
                        toNextBoardArea = 1;
                        break;
                    case DirLabel.DownRight:
                        toNextBoardArea = slotCollumns + 1;
                        break;
                    case DirLabel.Down:
                        toNextBoardArea = slotCollumns;
                        break;
                    case DirLabel.DownLeft:
                        toNextBoardArea = slotCollumns - 1;
                        break;
                    case DirLabel.Left:
                        toNextBoardArea = -1;
                        break;
                    case DirLabel.UpLeft:
                        toNextBoardArea = -slotCollumns - 1;
                        break;
                }*/
                int thisMany = (int)thisAmountEa.X;
                if (i == 1)
                {
                    thisMany = (int)thisAmountEa.Y;
                }
                for (int i2 = 0; i2 < thisMany; i2++)
                {
                    int thisLoc = startLoc + toNextBoardArea * (i2 + 1);
                    connectFourLocs.Add(thisLoc);
                }
            }
        }

        private static void DoSuccessfulConnectFourAnimation()
        {
            List<string> mFA = new List<string>();
            List<string> mFA2 = new List<string>();
           // int doneTimes = 0;

            while (mFA.Count < slotTotalSpaces)
            {
                bool winSpace = false;
                for (int z = 0; z < connectFourLocs.Count; z++)
                {
                    if ((int)connectFourLocs[z] == (int)mFA.Count)
                    {
                        winSpace = true;
                    }
                }
                if (winSpace)
                {
                    mFA.Add("");
                    mFA2.Add("");
                }
                else
                {
                    mFA.Add(art[(int)SlotState.Star]);
                    mFA2.Add(art[(int)SlotState.Empty]);
                }
            }
            int repeated = 0;
            int droppingHere = -1;
            do
            {
                droppingHere = repeated * slotCollumns + collumnSelected;
                if (repeated % 2 == 0)
                {
                    DrawGameState(true, mFA);
                }
                else
                {
                    DrawGameState(true, mFA2);
                }
                repeated++;
                Thread.Sleep(sDurMili);
            } while (repeated < 3);
        }

        public static void ChangeGameState(GameState toThisState)
        {
            myGameState = toThisState;
            switch (myGameState)
            {
                case GameState.Start:
                    firstTurn = true;
                    DrawGameState();
                    break;
                case GameState.PlayerTurn:
                    if (!firstTurn)
                        DropChip(SlotState.Yellow);
                    while (chipDropping)
                    {
                    }
                    firstTurn = false;
                    DrawGameState();
                    break;
                case GameState.AITurn:
                    if (!firstTurn)
                        DropChip(SlotState.Red);
                    while (chipDropping)
                    {
                    }
                    firstTurn = false;
                    DrawGameState();
                    AI.AITurn();
                    break;
                case GameState.Victory:
                    DrawGameState(false);
                    break;
                case GameState.Defeat:
                    DrawGameState(false);
                    break;
                case GameState.Draw:
                    DrawGameState(false);
                    break;

            }
            if (nextGameState != GameState.None && (AI.connectFourMade || AI.boardFilled))
            {
                toThisState = nextGameState;
                nextGameState = GameState.None;
                ChangeGameState(toThisState);
            }
        }


        public static void DrawGameState(bool drawBoard = true, IList<string> forceArt = null)
        {
            if (drawBoard)
            {
                //blue
                Console.ForegroundColor = (ConsoleColor)colorWhite;
                Console.WriteLine("");
                IList<SlotState> drawBoardAreaState = boardAreaState;
                


                bool aTurn = myGameState == GameState.AITurn || myGameState == GameState.PlayerTurn;

                IList<string> fRowArt = new List<string>();


                if (myGameState == GameState.PlayerTurn && collumnSelected != -1 && !AI.connectFourMade)
                {
                    for (int x2 = 0; x2 < slotCollumns; x2++)
                    {
                        if (aTurn && !chipDropping && x2 == collumnSelected)
                        {
                            fRowArt.Add(art[(int)SlotState.DownDrop]);
                      
                        }
                        else
                        {
                            fRowArt.Add("   ");
                           
                        }
                    }
                    for (int o = 0; o < fRowArt.Count; o++)
                    {
                        Console.Write(fRowArt[o]);
                    }
                    Console.WriteLine("");
                }

                // draw the game board
                for (int i = 0; i < slotRows; i++)
                {
                    IList<int> rowColors = new List<int>();
                    IList<string> rowArt = new List<string>();
                    for (int i2 = 0; i2 < slotCollumns; i2++)
                    {
                        bool yesForceArt = false;
                        if (forceArt != null)
                        {
                            if (forceArt.Count > i * slotCollumns + i2)
                            {
                                if (forceArt[i * slotCollumns + i2] != "")
                                {
                                    yesForceArt = true;
                                }
                            }
                        }
                        if (yesForceArt)
                        {
                            rowArt.Add(forceArt[i * slotCollumns + i2]);

                            if (forceArt[i * slotCollumns + i2] == art[1])

                                     {
                                rowColors.Add(colorRed);
                            }
                            else if (forceArt[i * slotCollumns + i2] == art[2])
                            {


                                rowColors.Add(colorYellow);
                            }
                            else {
                                rowColors.Add(colorWhite);
                            }
                        }
                        else if (!AI.connectFourMade && myGameState == GameState.PlayerTurn && aTurn && !chipDropping && i2 == collumnSelected && drawBoardAreaState[i * slotCollumns + i2] == SlotState.Empty)
                        {
                            int occupiedOrMissingBelow = 0;

                            if (i * slotCollumns + i2 + slotCollumns < slotTotalSpaces)
                            {
                                if (drawBoardAreaState[i * slotCollumns + i2 + slotCollumns] != SlotState.Empty)
                                {
                                    occupiedOrMissingBelow++;
                                }
                            }
                            else
                            {
                                occupiedOrMissingBelow++;
                            }
                            if (i * slotCollumns + i2 + slotCollumns * 2 < slotTotalSpaces)
                            {
                                if (drawBoardAreaState[i * slotCollumns + i2 + slotCollumns * 2] != SlotState.Empty)
                                {

                                    occupiedOrMissingBelow++;
                                }
                            }
                            else
                            {
                                occupiedOrMissingBelow++;
                            }
                            if (occupiedOrMissingBelow == 2)
                            {
                                rowArt.Add(art[(int)SlotState.Question]);
                                rowColors.Add(colorWhite);
                            }
                            else if (occupiedOrMissingBelow == 1)
                            {
                                rowArt.Add(art[(int)SlotState.DownArrow]);
                                rowColors.Add(colorWhite);
                            }
                            else if (occupiedOrMissingBelow == 0)
                            {
                                rowArt.Add(art[(int)SlotState.DownDrop]);
                                rowColors.Add(colorWhite);
                            }
                        }
                        else
                        {
                            rowArt.Add(art[(int)drawBoardAreaState[i * slotCollumns + i2]]);
                            if ((int)drawBoardAreaState[i * slotCollumns + i2] == 1)
                            {
                                rowColors.Add(colorRed);
                            }
                            else if ((int)drawBoardAreaState[i * slotCollumns + i2] == 2)
                            {
                                rowColors.Add(colorYellow);
                            }
                            else
                            {


                                rowColors.Add(colorWhite);
                            }
                        }
                    }
                    int lastSetColor = -1;
                    //Console.ForegroundColor = (ConsoleColor)10;
                    for (int o = 0; o < rowArt.Count; o++)
                    {
                        //yellow
                        if (rowColors[o] != lastSetColor)
                        {
                            lastSetColor = rowColors[o];
                            Console.ForegroundColor = (ConsoleColor)rowColors[o];
                        }
                       
                        Console.Write(rowArt[o]);
                        
                    }
                    Console.WriteLine("");
                    Console.ForegroundColor = (ConsoleColor)colorWhite;
                    
                    //Console.ForegroundColor = (ConsoleColor)6; // 4 red 6 yellow 16 white
                    
                    
                    /*
                    var colors = Enum.GetValues(typeof(ConsoleColor)).Cast<ConsoleColor>().ToArray();
                    int doneTimes = 0;
                    foreach (var color in colors)
                    {
                        Console.ForegroundColor = color;

                        Console.Write("ABC" + doneTimes);
                        doneTimes++;
                    }*/


                }
            }
            if (!chipDropping)
            {
                if (!AI.connectFourMade || myGameState == GameState.Victory || myGameState == GameState.Defeat)
                {
                    // write the game description
                    Console.WriteLine(gameStateDesc[(int)myGameState]);
                }
                else
                {
                    Console.WriteLine(" ");
                }
            }
        }

        private static void DropChip(SlotState dropThis)
        {
            chipDropping = true;
            int emptyVertSlots = 0;
            for (int i = 0; i < slotRows; i++)
            {
                if (i * slotCollumns + collumnSelected < slotTotalSpaces)
                {
                    if (boardAreaState[i * slotCollumns + collumnSelected] == SlotState.Empty)
                    {
                        emptyVertSlots++;
                    }
                }
            }
            int repeated = 0;
            int droppingHere = -1;
            do
            {
                droppingHere = repeated * slotCollumns + collumnSelected;
                List<string> mFA = new List<string>();
                while (mFA.Count < droppingHere)
                {
                    mFA.Add("");
                }
                mFA.Add(art[(int)dropThis]);
                if (!debugBuild)
                {
                    DrawGameState(true, mFA);
                }
                repeated++;
                if (!debugBuild)
                {
                    Thread.Sleep(sDurMili);
                }
            } while (repeated < emptyVertSlots);
            boardAreaState[droppingHere] = dropThis;
            chipsPlacedInCollumn[collumnSelected]++;

            if (!AI.connectFourMade)
            {
                int chipsPlaced = 0;
                for (int w = 0; w < chipsPlacedInCollumn.Count; w++)
                {
                    chipsPlaced += chipsPlacedInCollumn[w];
                    if (chipsPlaced == slotTotalSpaces)
                    {
                        if (!AI.boardFilled)
                        {
                            AI.boardFilled = true;
                            GSM.nextGameState = GSM.GameState.Draw;
                        }
                    }
                }
            }
                


            Thread.Sleep(sDurMili);
            SetDefaultColSelected();
            if (AI.connectFourMade)
            {
                DoSuccessfulConnectFourAnimation();
            }
            chipDropping = false;
        }
        private static void SetDefaultColSelected()
        {
            collumnSelected = (int)Math.Floor((float)slotCollumns * 0.5f);
            if (myGameState == GameState.PlayerTurn && lFCSel != -1)
            {
                collumnSelected = lFCSel;
            }
        }
        public static void SetUp()
        {
            for (int i = 0; i < SlotState.GetNames(typeof(SlotState)).Length; i++)
            {
                slotStateColors.Add(colorWhite);
            }


            // clear the board and set it up again
            lFCSel = -1;
            AI.connectFourMade = false;
            SetDefaultColSelected();
            boardAreaState.Clear();
            chipsPlacedInCollumn.Clear();
            for (int i = 0; i < slotTotalSpaces; i++)
            {
                boardAreaState.Add(SlotState.Empty);
            }
            for (int i2 = 0; i2 < slotCollumns; i2++)
            {
                chipsPlacedInCollumn.Add(0);
            }
            AI.ClearBoardScores();
            ChangeGameState(GameState.Start);
            Input.WaitForPlayerInput();
        }
    }
}
