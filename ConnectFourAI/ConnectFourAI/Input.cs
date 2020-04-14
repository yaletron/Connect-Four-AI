using System;

namespace ConnectFourAI
{
	public class Input : Core
	{
        public static void WaitForPlayerInput()
        {
            ConsoleKeyInfo keyPressed;
            do
            {
                keyPressed = Console.ReadKey();
                bool trueInput = PlayerPressedKey(keyPressed);
            } while (keyPressed.Key != ConsoleKey.Escape);
        }
        private static bool PlayerPressedKey(ConsoleKeyInfo keyPressed)
        {
            bool trueInput = false;
            int lastCollumnSelected = collumnSelected;
            GSM.GameState lastGameState = GSM.myGameState;
            if (keyPressed.Key == ConsoleKey.Enter)
            {
                if (GSM.myGameState == GSM.GameState.Start)
                {
                    // can coinflip for turn
                    GSM.ChangeGameState(GSM.GameState.PlayerTurn);
                    //GSM.ChangeGameState(GSM.GameState.AITurn);
                }
                else if (GSM.myGameState == GSM.GameState.PlayerTurn)
                {
                    if (!AI.connectFourMade && !AI.boardFilled)
                    {
                        if (boardAreaState[collumnSelected] == SlotState.Empty)
                        {
                            int activeSlot = MMath.ActiveSlot;
                            trueInput = true;
                            AI.Analyze(activeSlot, playerDC, true);
                            GSM.ChangeGameState(GSM.GameState.AITurn);
                        }
                        else
                        {
                            Console.WriteLine(">>> Can't Place Chip Here. <<< ");
                        }
                    }
                }
                else
                {
                    if (GSM.myGameState == GSM.GameState.Victory || GSM.myGameState == GSM.GameState.Defeat)
                    {
                        GSM.SetUp();
                    }
                }
            }
            else if (keyPressed.Key == ConsoleKey.Escape)
            {
                trueInput = true;
                running = false;
            }
            else if (keyPressed.Key == ConsoleKey.LeftArrow)
            {
                if (GSM.myGameState == GSM.GameState.PlayerTurn)
                {
                    collumnSelected--;
                    collumnSelected = Math.Clamp(collumnSelected, 0, slotCollumns - 1);
                }
            }
            else if (keyPressed.Key == ConsoleKey.RightArrow)
            {
                if (GSM.myGameState == GSM.GameState.PlayerTurn)
                {
                    collumnSelected++;
                    collumnSelected = Math.Clamp(collumnSelected, 0, slotCollumns - 1);
                }
            }
            if (lastCollumnSelected != collumnSelected || lastGameState != GSM.myGameState)
            {
                trueInput = true;
                if (lastCollumnSelected != collumnSelected)
                {
                    lFCSel = collumnSelected;
                    GSM.DrawGameState();
                }
            }
            return trueInput;
        }
    }
}
