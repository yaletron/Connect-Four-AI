using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;

namespace ConnectFourAI
{
    // CONFIRM ANA ABOVE IS SHOWING CORRECT NUMBER SLOT IN CONSOLE WRITE OUT (assigned above or below??)
    public class AI : Core
    {
        // switching dirlabels to get next equasion for slot in direction should be under MMath.cs
        // makes scripts have less than 200 lines ea
        // make a class for slotscore from perspective and 'during turn'
        // variable to loop out of gameturns if either of below is true
        public static bool connectFourMade = false;
        public static bool boardFilled = false;

        // AI takes a turn, priorities 1. make friendly connect 4, 2. block enemy connect 4, 3. dont place when opens above slot to opportunities
        public static void AITurn()
        {
            if (connectFourMade || boardFilled)
            {
                return;
            }
            ClearBoardScores();
            int bestCol = 0;
            // check above slot score this collumn for what opportunities are opened next turn
            int aboveScoresChecked = 0;
            List<bool> bScoreAboveChecked = new List<bool>();
            List<int> bScoreAbove = new List<int>();
            List<int> bScore = new List<int>();
            List<bool> availCollumns = new List<bool>();
            List<AIChoice> mAIChoice = new List<AIChoice>();
            List<AIChoice> mAICAbove = new List<AIChoice>();
            List<string> anaFrom = new List<string>();
            List<string> aboveAnaFrom = new List<string>();
            Tuple<AIChoice, int> sD = new Tuple<AIChoice, int>(AIChoice.None, 0);

            for (int i = 0; i < slotCollumns; i++)
            {
                aboveAnaFrom.Add("n/a");
                anaFrom.Add("n/a");
                collumnSelected = i;
                bScore.Add(0);
                mAIChoice.Add(AIChoice.None);
                mAICAbove.Add(AIChoice.None);
                bScoreAboveChecked.Add(false);
                bScoreAbove.Add(0);
                int thisSlot = MMath.ActiveSlot;
                availCollumns.Add(false);


                if (thisSlot != -1)
                {
                    availCollumns.Add(false);
                    int thisScore = 0;
                    AIChoice thisAIC = AIChoice.None;
                    int enemyScore = 0;
                    AIChoice enemyAIC = AIChoice.None;
                    availCollumns[i] = true;


                    sD = Analyze(thisSlot, aiDC, true);
                    thisScore = sD.Item2;
                    thisAIC = sD.Item1;

                    // prefer to build friendly so -1 point here

                    sD = Analyze(thisSlot, playerDC);
                    enemyScore = sD.Item2 - 1;
                    enemyAIC = sD.Item1;

                    if (thisScore > bScore[i])
                    {
                        anaFrom[i] = "thisScore";
                        bScore[i] = thisScore;
                        mAIChoice[i] = thisAIC;
                    }
                    // will block enemy with better score (-1) unless AI will make connect 4
                    if (enemyScore > bScore[i] && !AI.connectFourMade)
                    {
                        anaFrom[i] = "enemyScore";
                        bScore[i] = enemyScore;
                        mAIChoice[i] = enemyAIC;
                    }
                    int visCol = i + 1;
                }
            }
            int mBScore = -9999;
            for (int e = 0; e < slotCollumns; e++)
            {

                if (bScore[e] > mBScore && availCollumns[e])
                {
                    mBScore = bScore[e];
                    bestCol = e;
                }
            }
            if (!AI.connectFourMade)
            {
                bool finalApproval = false;
                while (!finalApproval)
                {

                    if (bScoreAboveChecked[bestCol] == false)
                    {
                        aboveScoresChecked++;
                        bScoreAboveChecked[bestCol] = true;
                        if (MMath.ActiveSlotByCol(bestCol) - slotCollumns > 0)
                        {

                            sD = Analyze(MMath.ActiveSlotByCol(bestCol) - slotCollumns, aiDC);
                            int fScoreAbove = sD.Item2;
                            AIChoice fAboveAIC = sD.Item1;



                            sD = Analyze(MMath.ActiveSlotByCol(bestCol) - slotCollumns, playerDC);
                            int eScoreAbove = sD.Item2 - 1;
                            AIChoice eAboveAIC = sD.Item1;

                            if (fScoreAbove >= eScoreAbove)
                            {
                                aboveAnaFrom[bestCol] = "fScoreAbove";
                                bScoreAbove[bestCol] = fScoreAbove;
                                mAICAbove[bestCol] = fAboveAIC;
                            }
                            else
                            {
                                aboveAnaFrom[bestCol] = "eScoreAbove";
                                bScoreAbove[bestCol] = eScoreAbove;
                                mAICAbove[bestCol] = eAboveAIC;
                            }

                        }
                        // if urgency is less than 2 step or less than urgency or original position discard
                        if ((int)mAICAbove[bestCol] < (int)AIChoice.Make2Step || (int)mAICAbove[bestCol] <= (int)mAIChoice[bestCol])
                        {
                            finalApproval = true;
                            break;
                        }
                        else
                        {
                            // if enemy will make connect four above or 2 step (without you making 2 step or connect four) this is most serious
                            if (aboveAnaFrom[bestCol] == "eScoreAbove")
                            {
                                bScore[bestCol] = bScoreAbove[bestCol] * -1;
                            }
                            else
                            {
                                // if protecting your own combo from being blocked just use -1 as score
                                bScore[bestCol] = -1;
                            }

                            int mBScore2 = -9999;
                            for (int e = 0; e < slotCollumns; e++)
                            {
                                if (bScore[e] > mBScore2 && availCollumns[e])
                                {
                                    mBScore2 = bScore[e];
                                    bestCol = e;
                                }
                            }
                            if (aboveScoresChecked == availCollumns.Count)
                            {
                                finalApproval = true;
                                break;
                            }
                        }
                    }
                }
            }




            // check is collumn is filled redundancy
            if (!availCollumns[bestCol])
            {
                for (int g = 0; g < slotCollumns; g++)
                {
                    if (availCollumns[g])
                    {
                        bestCol = g;
                    }
                }
            }




            bool showWeightedValues = false;
            if (showWeightedValues)
            {
                GSM.DrawGameState(true, bAAS);
                Console.WriteLine("^ score for Yellow   v score For Red");
                GSM.DrawGameState(true, bAASE);
                Console.ForegroundColor = (ConsoleColor)colorGrey;
                int vC = -1;

                for (int x = 0; x < slotCollumns; x++)
                {
                    string abovDesc = " no above measured ";
                    if (bScoreAboveChecked[x])
                    {
                        abovDesc = " above desc: " + bScoreAbove[bestCol] + " " + mAICAbove[bestCol] + " above from: " + aboveAnaFrom[bestCol];
                    }
                    vC = x + 1;
                    Console.WriteLine("col: " + vC + " : " + mAIChoice[x] + " bScore[i] " + bScore[x] + " ana from: " + anaFrom[x] + abovDesc);
                }
                
                Console.ForegroundColor = (ConsoleColor)colorWhite;
                

            }
            int vC2 = bestCol + 1;
            Console.WriteLine("AI Chooses To Place In Collumn: " + vC2 + ".");
            Thread.Sleep(2000);
            AIMakeFinalDecision(bestCol);
            

        }
        private static void AIMakeFinalDecision(int bestCol)
        {
            collumnSelected = bestCol;
            ClearBoardScores();
            GSM.ChangeGameState(GSM.GameState.PlayerTurn);
        }
        public static void ClearBoardScores()
        {
            bAAS.Clear();
            bAASE.Clear();
            for (int i = 0; i < slotTotalSpaces; i++)
            {
                bAAS.Add("");
                bAASE.Add("");
            }
        }

        // state of a chip area labels
        public enum AIChoice
        {
            None,
            Normal, // normal includes blocking enemy // needs to calc spot below / 'real' spot as friendly chip color
            Make2Step,
            MakeConnectFour
        }

        /*
             ENEMY PRIORITIES FOR REF: 

             None,
             Normal, // normal includes blocking enemy
             AvoidAboveHasFriendlyConnectFour,
             AvoidAboveHasEnemy2Step, // needs to calc spot below / 'real' spot as friendly chip color
             Make2Step,
             BlockEnemy2Step,
             AvoidAboveHasEnemyConnectFour,
             BlockConnectFour,
             MakeConnectFour

          */



        // anaylyze the 'score' of placing a chip in a board area default from AI perspective -- yellow chips
        public static Tuple<AIChoice, int> Analyze(int startLoc, SlotState fSC, bool forActivePlacement = false)
        {
            Tuple<AIChoice, int> mR = new Tuple<AIChoice, int>(AIChoice.None, 0);
            if (connectFourMade)
            {
                return mR;
            }

            List<SlotState> bASReturns = new List<SlotState>();
            List<List<SlotState>> bASAllDir = new List<List<SlotState>>();
            // first vacant slot direction grounded?
            IList<int> fVSGDir = new List<int>();
            int thisDir = 0;
            while (bASAllDir.Count < DirLabel.GetNames(typeof(DirLabel)).Length)
            {
                DirLabel thisDirLabel = (DirLabel)thisDir;
                Tuple<int, List<SlotState>> mCD = CheckDirection(startLoc, thisDirLabel);
                bASReturns = mCD.Item2;
                fVSGDir.Add(mCD.Item1);
                thisDir++;
                bASAllDir.Add(bASReturns);
            }
            // relative friendly and enemy chip colors for analysis
            SlotState eSC;
            if (fSC == SlotState.Yellow)
            {
                eSC = SlotState.Red;
            }
            else
            {
                eSC = SlotState.Yellow;
            }
            int dataE = 0;
            IList<int> adjGoodSlotsDir = new List<int>();
            IList<int> adjFSlotsDir = new List<int>();
            IList<int> adjFSlotsTouching = new List<int>();
            // first vacant spot grounded for direction


            while (adjGoodSlotsDir.Count < DirLabel.GetNames(typeof(DirLabel)).Length)
            {
                adjGoodSlotsDir.Add(0);
                adjFSlotsDir.Add(0);
                adjFSlotsTouching.Add(0);
            }
            int aThisDir = 0;
            foreach (List<SlotState> subList in bASAllDir)
            {
                bool hitEnemyChip = false;
                bool nonTouchingFound = false;
                foreach (SlotState item in subList)
                {
                    if (item == eSC)
                    {
                        hitEnemyChip = true;
                        nonTouchingFound = true;
                    }

                    if (!hitEnemyChip)
                    {
                        if (item == fSC || item == SlotState.Empty)
                        {
                            adjGoodSlotsDir[aThisDir]++;
                            if (item == fSC)
                            {
                                adjFSlotsDir[aThisDir]++;
                                if (!nonTouchingFound)
                                {
                                    adjFSlotsTouching[aThisDir]++;
                                }
                            }
                            else
                            {
                                nonTouchingFound = true;
                            }
                        }
                    }
                    dataE++;
                    // Console.WriteLine(item);
                }
                aThisDir++;
            }
            Tuple<AIChoice, int> axisScore = AxisScore(fVSGDir, adjGoodSlotsDir, adjFSlotsDir, adjFSlotsTouching, fSC, forActivePlacement, startLoc);
            string dThis = axisScore.Item2.ToString();
            int addToLength = 3;
            while (dThis.Length < addToLength)
            {
                dThis += " ";
            }
            //HighlightBoardSpace(startLoc, dThis);
            if (fSC == SlotState.Yellow)
            {
                bAAS[startLoc] = dThis;
            }
            else if (fSC == SlotState.Red)
            {
                bAASE[startLoc] = dThis;
            }
            //Console.WriteLine("totalM = " + dataE + " colS: " + collumnSelected + " axisS: " + axisScore);
            return axisScore;
        }

        public static Tuple<AIChoice, int> AxisScore(IList<int> fVSGDir, IList<int> adjGoodSlotsDir, IList<int> adjFSlotsDir, IList<int> adjFSlotsTouching, SlotState fSC, bool forActivePlacement, int startLoc)
        {
            AIChoice mC = AIChoice.None;
            Tuple<AIChoice, int> mR = new Tuple<AIChoice, int>(AIChoice.None, 0);
            if (connectFourMade)
            {
                return mR;
            }
            List<int> axisScore = new List<int>();
            int aS = 0;
            // normal number of measurements on axis for board position
            int normalNM = 6;
            for (int i = 0; i < 4; i++)
            {

                int adjGoodSlotsAxis = adjGoodSlotsDir[i] + adjGoodSlotsDir[i + 4];
                int addThis = adjGoodSlotsAxis;
                // less than connect 4 amount needed, discard axis score
                if (addThis < 3)
                {
                    addThis = 0;
                }
                else
                {
                    if ((int)mC < (int)AIChoice.Normal)
                        mC = AIChoice.Normal;

                    // Prefer touching chips
                    int inARow = adjFSlotsTouching[i] + adjFSlotsTouching[i + 4] + 1;
                    // + 1 if touching on both sides
                    if (adjFSlotsTouching[i] > 0 && adjFSlotsTouching[i + 4] > 0)
                    {
                        addThis++;
                    }
                    // +2 to score for each friendly chip as opposed to empty slots
                    int friendlyChipsOnAxis = adjFSlotsDir[i] + adjFSlotsDir[i + 4];
                    addThis += friendlyChipsOnAxis * 2;

                    int groundedVacantBothSides = fVSGDir[i] + fVSGDir[i + 4];

                    int vacantSpaces = adjGoodSlotsAxis + 1 - friendlyChipsOnAxis;

                    if (vacantSpaces >= 3 && inARow >= 2 && addThis >= 4 && friendlyChipsOnAxis >= 2 && (normalNM - addThis >= 2 || addThis - friendlyChipsOnAxis >= 2))
                    {
                        // Console.WriteLine("found opportunity - - -  - - vacantSpaces: " + vacantSpaces + " adjGoodSlotsAxis: " + adjGoodSlotsAxis + " minus friendlyChipsOnAxis: " + friendlyChipsOnAxis + " grounded vacant both sides:" + groundedVacantBothSides);
                        if (groundedVacantBothSides == 2)
                        {
                            if ((int)mC < (int)AIChoice.Make2Step)
                                mC = AIChoice.Make2Step;
                            addThis += 777;


                        }
                        else
                        {
                            addThis += 50;
                        }
                    }

                    if (inARow >= 4)
                    {
                        if ((int)mC < (int)AIChoice.MakeConnectFour)
                            mC = AIChoice.MakeConnectFour;

                        addThis += 999;



                        if (forActivePlacement)
                        {
                            if (!connectFourMade)
                            {

                                connectFourMade = true;
                                Vector2 tT = new Vector2((int)adjFSlotsTouching[i], (int)adjFSlotsTouching[i + 4]);
                                GSM.ConnectFourMade(startLoc, i, tT);
                                if (fSC == SlotState.Yellow)
                                {
                                    GSM.nextGameState = GSM.GameState.Defeat;
                                }
                                else
                                {
                                    GSM.nextGameState = GSM.GameState.Victory;
                                }
                            }
                        }
                    }
                }
                axisScore.Add(addThis);
                aS += addThis;
            }
            mR = new Tuple<AIChoice, int>(mC, aS);
            return mR;
        }

        public static Tuple<int, List<SlotState>> CheckDirection(int startLoc, DirLabel dir)
        {
            // first vacant is grounded
            int mfVSG = 0;
            // first vacant check available
            bool fVA = true;
            bool onBoard = true;
            // start collum  and row
            Vector2 startCR = new Vector2();
            startCR = MMath.OnCollumnRow(startLoc);
            List<SlotState> slotsInThisDir = new List<SlotState>();
            int toNextBoardArea = MMath.ToNextInDir(dir);
            int checks = 0;
            do
            {
                checks++;
                int checkHere = startLoc + toNextBoardArea * checks;
                checkHere = startLoc + toNextBoardArea * checks;
                SlotState checkSlot = SlotState.None;
                onBoard = true;
                if (checkHere >= slotTotalSpaces || checkHere < 0)
                {
                    onBoard = false;
                }
                if (onBoard)
                {
                    bool checkUp = false, checkLeft = false, checkRight = false, checkDown = false;
                    // off top of game board?
                    if (dir == DirLabel.Up || dir == DirLabel.UpLeft || dir == DirLabel.UpRight)
                    {
                        checkUp = true;
                    }
                    // off right of game board?
                    if (dir == DirLabel.Right || dir == DirLabel.UpRight || dir == DirLabel.DownRight)
                    {
                        checkRight = true;
                    }
                    // off left of game board?
                    if (dir == DirLabel.Left || dir == DirLabel.UpLeft || dir == DirLabel.DownLeft)
                    {
                        checkLeft = true;
                    }
                    // off bottom of game board?
                    if (dir == DirLabel.Down || dir == DirLabel.DownLeft || dir == DirLabel.DownRight)
                    {
                        checkDown = true;
                    }
                    if (checkUp)
                    {
                        if (startCR.Y - checks < 0)
                        {
                            onBoard = false;
                        }
                    }
                    if (checkDown)
                    {
                        if (checks + startCR.Y >= slotRows)
                        {
                            onBoard = false;
                        }
                    }
                    if (checkLeft)
                    {
                        if (startCR.X - checks < 0)
                        {
                            onBoard = false;
                        }
                    }
                    if (checkRight)
                    {
                        if (checks + startCR.X >= slotCollumns)
                        {
                            onBoard = false;
                        }
                    }
                }
                if (onBoard)
                {
                    // check if first vacant spot is grounded
                    checkSlot = boardAreaState[checkHere];
                    if (fVA && checkSlot == SlotState.Empty)
                    {
                        if (MMath.IsGroundedVacant(checkHere))
                        {
                            mfVSG = 1;
                        }
                        fVA = false;
                    }

                    slotsInThisDir.Add(checkSlot);
                }
            } while (onBoard && checks < 3);
            Tuple<int, List<SlotState>> mR = new Tuple<int, List<SlotState>>(mfVSG, slotsInThisDir);
            return mR;
        }
    }
}
