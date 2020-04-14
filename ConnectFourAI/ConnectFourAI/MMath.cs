using System;
using System.Numerics;

namespace ConnectFourAI
{
	public class MMath : Core
	{
        // is this slot grounded?
        public static bool IsGroundedVacant (int thisSlot)
        {
            bool iGV = false;
            Vector2 mCR = OnCollumnRow(thisSlot);
            int saveCS = collumnSelected;
            collumnSelected = (int)mCR.X;
            if (thisSlot == ActiveSlot)
            {
                iGV = true;
            }
            collumnSelected = saveCS;
            return iGV;
        }
        public static int ActiveSlot
        {
            // property to return board location the chip will land
            get
            {
                int mAS = -1;
                int rOccupied = chipsPlacedInCollumn[collumnSelected];
                mAS = slotTotalSpaces - slotCollumns + collumnSelected - rOccupied * slotCollumns;
                if (rOccupied >= slotRows || mAS < 0 || mAS >= slotTotalSpaces)
                {
                    mAS = -1;
                }
                return mAS;
            }
        }
        // return active slot of the collumn
        public static int ActiveSlotByCol(int col)
        {
                int mAS = -1;
                int rOccupied = chipsPlacedInCollumn[col];
                mAS = slotTotalSpaces - slotCollumns + col - rOccupied * slotCollumns;
                if (rOccupied >= slotRows || mAS < 0 || mAS >= slotTotalSpaces)
                {
                    mAS = -1;
                }
                return mAS;
        
        }
        // returns the collumn and row from the board location
        public static Vector2 OnCollumnRow(int thisLoc)
        {
            Vector2 mCR = new Vector2(thisLoc, 0);
            while (mCR.X >= slotCollumns)
            {
                mCR.X -= slotCollumns;
                mCR.Y++;
            }
            return mCR;
        }

        public static int ToNextInDir(DirLabel dir)
        {
            int toNextBoardArea = 0;
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
            }
            return toNextBoardArea;
        }
    }
}
