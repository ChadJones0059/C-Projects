using System;
using System.Collections.Generic;
using static System.Console;

namespace Blackjack
{
    public class Casino
    {
        public static int MinimumBet { get; } = 10;
        
        /// <param name="hand">The hand to check</param>
        // Returns true if the hand is blackjack
        public static bool IsHandBlackjack(List<Card> hand)
        {
            if (hand.Count == 2)
            {
                if (hand[0].Face == Face.Ace && hand[1].Value == 10) return true;
                else if (hand[1].Face == Face.Ace && hand[0].Value == 10) return true;
            }
            return false;
        }
        public static void ColorReset()
        {
            ForegroundColor = ConsoleColor.White;
        }
        
    }

    public class Player
    {
        public int Chips { get; set; } = 500;
        public int Bet { get; set; }
        public int Wins { get; set; }
        public int HandsCompleted { get; set; } = 1;

        public List<Card> Hand { get; set; }

        // Add Player's chips to their bet
        /// <param name="bet">The number of Chips to bet</param>
        public void AddBet(int bet)
        {
            Bet += bet;
            Chips -= bet;
        }

        // Set Bet to 0
        public void ClearBet()
        {
            Bet = 0;
        }

        // Cancel player's bet. They will neither lose nor gain any chips
        public void ReturnBet()
        {
            Chips += Bet;
            ClearBet();
        }

        // Give player chips that they won from their bet.
        /// <param name="blackjack">If player won with blackjack, player wins 1.5 times their bet
        public int WinBet(bool blackjack)
        {
            int chipsWon;
            if (blackjack)
            {
                chipsWon = (int)Math.Floor(Bet * 1.5);
            }
            else
            {
                chipsWon = Bet * 2;
            }

            Chips += chipsWon;
            ClearBet();
            return chipsWon;
        }

        // Value of all cards in Hand
        public int GetHandValue()
        {
            int value = 0;
            foreach (Card card in Hand)
            {
                value += card.Value;
            }
            return value;
        }

        // Write player's hand to console
        public void WriteHand()
        {
            // Write Bet, Chip, Win, Amount with color, and write Round #
            Write("Bet: ");
            Write(Bet + "  ");
            
            Write("Chips: ");
            Write(Chips + "  ");
            
            Write("Wins: ");
            WriteLine(Wins);
            
            WriteLine("Round #" + HandsCompleted);

            WriteLine();
            WriteLine("Your Hand (" + GetHandValue() + "):");
            foreach (Card card in Hand)
            {
                card.WriteDescription();
            }
            WriteLine();
        }
    }

    public class Dealer
    {
        public static List<Card> HiddenCards { get; set; } = new List<Card>();
        public static List<Card> RevealedCards { get; set; } = new List<Card>();

        // Take the top card from HiddenCards, remove it, and add it to RevealedCards 
        public static void ShowCard()
        {
            RevealedCards.Add(HiddenCards[0]);
            HiddenCards.RemoveAt(0);
        }

        // Value of all cards in RevealedCards
        public static int GetHandValue()
        {
            int value = 0;
            foreach (Card card in RevealedCards)
            {
                value += card.Value;
            }
            return value;
        }

        // Write Dealer's RevealedCards to Console.
        public static void WriteHand()
        {
            WriteLine("Dealer's Hand (" + GetHandValue() + "):");
            foreach (Card card in RevealedCards)
            {
                card.WriteDescription();
            }
            for (int i = 0; i < HiddenCards.Count; i++)
            {
                WriteLine("<hidden>");
                
            }
            WriteLine();
        }
    }
}