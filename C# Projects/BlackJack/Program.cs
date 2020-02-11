using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using static System.Console;

namespace Blackjack
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // We need this to render Unicode
            OutputEncoding = Encoding.UTF8;

            
            Title = "♠♥♣♦ Blackjack";

            WriteLine("♠♥♣♦ Welcome to Blackjack");
            WriteLine("Press any key to play.");
            ReadKey();

            StartRound();
        }

        // How we start the game
        static void StartRound()
        {
            Clear();

            if (!TakeBet())
            {
                EndRound(RoundResult.INVALID_BET);
                return;
            }
            Clear();

            StartHands();
            Actions();

            Dealer.ShowCard();

            Clear();
            player.WriteHand();
            Dealer.WriteHand();

            player.HandsCompleted++;

            if (player.Hand.Count == 0)
            {
                EndRound(RoundResult.FOLD);
                return;
            }
            else if (player.GetHandValue() > 21)
            {
                EndRound(RoundResult.PLAYER_BUST);
                return;
            }

            while (Dealer.GetHandValue() <= 16)
            {
                
                Dealer.RevealedCards.Add(deck.DrawCard());

                Clear();
                player.WriteHand();
                Dealer.WriteHand();
            }


            if (player.GetHandValue() > Dealer.GetHandValue())
            {
                player.Wins++;
                if (Casino.IsHandBlackjack(player.Hand))
                {
                    EndRound(RoundResult.PLAYER_BLACKJACK);
                }
                else
                {
                    EndRound(RoundResult.PLAYER_WIN);
                }
            }
            else if (Dealer.GetHandValue() > 21)
            {
                player.Wins++;
                EndRound(RoundResult.PLAYER_WIN);
            }
            else if (Dealer.GetHandValue() > player.GetHandValue())
            {
                EndRound(RoundResult.DEALER_WIN);
            }
            else
            {
                EndRound(RoundResult.PUSH);
            }

        }

        public static Deck deck = new Deck();
        public static Player player = new Player();


        private enum RoundResult
        {
            PUSH,
            PLAYER_WIN,
            PLAYER_BUST,
            PLAYER_BLACKJACK,
            DEALER_WIN,
            FOLD,
            INVALID_BET
        }

        // Initialize the Deck and deal to player and dealer
        static void StartHands()
        {
            deck.Initialize();

            player.Hand = deck.DealHand();
            Dealer.HiddenCards = deck.DealHand();
            Dealer.RevealedCards = new List<Card>();

            // If hand contains two aces, make one Hard.
            if (player.Hand[0].Face == Face.Ace && player.Hand[1].Face == Face.Ace)
            {
                player.Hand[1].Value = 1;
            }

            if (Dealer.HiddenCards[0].Face == Face.Ace && Dealer.HiddenCards[1].Face == Face.Ace)
            {
                Dealer.HiddenCards[1].Value = 1;
            }

            Dealer.ShowCard();

            player.WriteHand();
            Dealer.WriteHand();
        }

        
        // Ask user for action and perform that action until they stay, double, or bust
        static void Actions()
        {
            string action;
            do
            {
                Clear();
                player.WriteHand();
                Dealer.WriteHand();

                Write("Enter Action (? for help): ");
                action = ReadLine();
                Casino.ColorReset();
                

                switch (action.ToUpper())
                {
                    case "HIT":
                        player.Hand.Add(deck.DrawCard());
                        break;
                    case "STAY":
                        break;
                    case "FOLD":
                        player.Hand.Clear();
                        break;
                    case "DOUBLE":
                        if (player.Chips <= player.Bet)
                        {
                            player.AddBet(player.Chips);
                        }
                        else
                        {
                            player.AddBet(player.Bet);
                        }
                        player.Hand.Add(deck.DrawCard());
                        break;
                    default:
                        WriteLine("Valid Moves:");
                        WriteLine("Hit, Stay, Fold, Double");
                        WriteLine("Press any key to continue.");
                        ReadKey();
                        break;
                }

                if (player.GetHandValue() > 21)
                {
                    foreach (Card card in player.Hand)
                    {
                        if (card.Value == 11) // Only a soft ace can have a value of 11
                        {
                            card.Value = 1;
                            break;
                        }
                    }
                }
            } while (!action.ToUpper().Equals("STAY") && !action.ToUpper().Equals("DOUBLE")
                && !action.ToUpper().Equals("FOLD") && player.GetHandValue() <= 21);
        }

        // Take player's bet and checks to see if bet is valid
        static bool TakeBet()
        {
            Write("Current Chip Count: ");
            WriteLine(player.Chips);
            Casino.ColorReset();
            

            Write("Minimum Bet: ");
            WriteLine(Casino.MinimumBet);
            Casino.ColorReset();

            Write("Enter bet to begin hand " + player.HandsCompleted + ": ");
            string s = ReadLine();
            Casino.ColorReset();

            if (Int32.TryParse(s, out int bet) && bet >= Casino.MinimumBet && player.Chips >= bet)
            {
                player.AddBet(bet);
                return true;
            }
            return false;
        }

        // Perform action based on result of round and starts next round
        /// <param name="result">The result of the round</param>
        static void EndRound(RoundResult result)
        {
            switch (result)
            {
                case RoundResult.PUSH:
                    Casino.ColorReset();
                    player.ReturnBet();
                    WriteLine("Player and Dealer Push.");
                    break;
                case RoundResult.PLAYER_WIN:
                    Casino.ColorReset();
                    WriteLine("Player Wins " + player.WinBet(false) + " chips");
                    break;
                case RoundResult.PLAYER_BUST:
                    Casino.ColorReset();
                    player.ClearBet();
                    WriteLine("Player Busts");
                    break;
                case RoundResult.PLAYER_BLACKJACK:
                    Casino.ColorReset();
                    WriteLine("Player Wins " + player.WinBet(true) + " chips with Blackjack.");
                    break;
                case RoundResult.DEALER_WIN:
                    Casino.ColorReset();
                    player.ClearBet();
                    WriteLine("Dealer Wins.");
                    break;
                case RoundResult.FOLD:
                    Casino.ColorReset();
                    WriteLine("Player Folds " + (player.Bet / 2) + " chips");
                    player.Chips += player.Bet / 2;
                    player.ClearBet();
                    break;
                case RoundResult.INVALID_BET:
                    Casino.ColorReset();
                    WriteLine("Invalid Bet.");
                    break;
            }

            if (player.Chips <= 0)
            {
                WriteLine();
                WriteLine("You ran out of Chips after " + (player.HandsCompleted - 1) + " rounds.");
                WriteLine("500 Chips will be added and your statistics have been reset.");

                player = new Player();
            }

            Casino.ColorReset();
            WriteLine("Press any key to continue");
            ReadKey();

            StartRound();
        }
    }
}