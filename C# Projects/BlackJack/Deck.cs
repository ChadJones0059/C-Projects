using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack
{
    public class Deck
    {
        private List<Card> cards;

        // Start the process of dealing the cards
        public Deck()
        {
            Initialize();
        }

        // Makes a list of 2 cards
        public List<Card> DealHand()
        {
            // Create a temporary list of cards and give it the top two cards of the deck.
            List<Card> hand = new List<Card>();
            hand.Add(cards[0]);
            hand.Add(cards[1]);

            // Remove the cards added to the hand.
            cards.RemoveRange(0, 2);

            return hand;
        }

        // Draws a card from top of deck
        public Card DrawCard()
        {
            Card card = cards[0];
            cards.Remove(card);

            return card;
        }

        // Shuffles the deck
        public void Shuffle()
        {
            Random rng = new Random();

            int n = cards.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Card card = cards[k];
                cards[k] = cards[n];
                cards[n] = card;
            }
        }

        // Replace the deck with a new deck and then shuffle it
        public void Initialize()
        {
            cards = GetNewDeck();
            Shuffle();
        }

        // Returns a Cold Deck-- a deck organized by Suit and Face
        public List<Card> GetNewDeck()
        {
            List<Card> coldDeck = new List<Card>();

            for (int i = 0; i < 13; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    coldDeck.Add(new Card((Suit)j, (Face)i));
                }
            }

            return coldDeck;
        }
    }
}