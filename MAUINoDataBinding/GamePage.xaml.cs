using AVFoundation;
using CoreLocation;

namespace MAUINoDataBinding;

public partial class GamePage : ContentPage
{
	public GamePage()
	{
		InitializeComponent();
	}

    private void SubmitButton_Clicked(object sender, EventArgs e)
    {
        string response = Console.ReadLine();
        int numberOfPlayers;
        while (int.TryParse(response, out numberOfPlayers) == false
            || numberOfPlayers < 2 || numberOfPlayers > 8)
        {
            Console.WriteLine("Invalid number of players.");
            Console.Write("How many players?");
            response = Console.ReadLine();
        }
        return numberOfPlayers;
    }

    public void DoNextTask()
    {
        Console.WriteLine("================================"); // this line should be elsewhere right?
        if (nextTask == Task.GetNumberOfPlayers)
        {
            numberOfPlayers = cardTable.GetNumberOfPlayers();
            nextTask = Task.GetNames;
        }
        else if (nextTask == Task.GetNames)
        {
            for (var count = 1; count <= numberOfPlayers; count++)
            {
                var name = cardTable.GetPlayerName(count);
                AddPlayer(name); // NOTE: player list will start from 0 index even though we use 1 for our count here to make the player numbering more human-friendly
            }
            nextTask = Task.IntroducePlayers;
        }
        else if (nextTask == Task.IntroducePlayers)
        {
            cardTable.ShowPlayers(players);
            nextTask = Task.PlayerTurn;
        }
        else if (nextTask == Task.PlayerTurn)
        {
            cardTable.ShowHands(players);
            Player player = players[currentPlayer];
            if (player.status == PlayerStatus.active)
            {
                if (cardTable.OfferACard(player))
                {
                    Card card = deck.DealTopCard();
                    player.cardInfo().Add(card);
                    player.score = ScoreHand(player);
                    if (player.score > 21)
                    {
                        player.status = PlayerStatus.bust;
                    }
                    else if (player.score == 21)
                    {
                        player.status = PlayerStatus.win;
                        Player winnerplayer = DoFinalScoring();
                        cardTable.AnnounceWinner(winnerplayer);
                    }
                }
                else
                {
                    player.status = PlayerStatus.stay;
                    cardTable.ShowHand(player);
                }
            }
            cardTable.ShowHand(player);
            nextTask = Task.CheckForEnd;
        }
        else if (nextTask == Task.CheckForEnd)
        {
            if (!CheckActivePlayers())
            {
                Player winner = DoFinalScoring();
                cardTable.AnnounceWinner(winner);
                nextTask = Task.GameOver;
            }
            else
            {
                currentPlayer++;
                if (currentPlayer > players.Count - 1)
                {
                    currentPlayer = 0; // back to the first player...
                }
                nextTask = Task.PlayerTurn;
            }
        }
    }

    /// <summary>
    /// Score the cards in the player's list of cards OR (if cheating)
    /// ask for a value and set player score to it.
    /// </summary>
    /// <param name="player">Instance representing one player</param>
    /// <returns></returns>
    public int ScoreHand(Player player)
    {
        int score = 0;
        if (cheating == true && player.status == PlayerStatus.active)
        {
            string response = null;
            while (int.TryParse(response, out score) == false)
            {
                Console.Write("OK, what should player " + player.name + "'s score be?");
                response = Console.ReadLine();
            }
            return score;
        }
        else
        {
            foreach (Card card in player.cardInfo())
            {
                string faceValue = card.ID.Remove(card.ID.Length - 1);
                switch (faceValue)
                {
                    case "K":
                    case "Q":
                    case "J":
                        score = score + 10;
                        break;
                    case "A":
                        score = score + 1;
                        break;
                    default:
                        score = score + int.Parse(faceValue);
                        break;
                }
            }
            /* Alternative method of handling the above foreach loop using char math instead of strings.
             * No need to do this; just showing you a trick!
             */
            //foreach (Card card in player.cards)
            //{
            //    char faceValue = card.ID[0];
            //    switch (faceValue)
            //    {
            //        case 'K':
            //        case 'Q':
            //        case 'J':
            //            score = score + 10;
            //            break;
            //        case 'A':
            //            score = score + 1;
            //            break;
            //        default:
            //            score = score + (faceValue - '0'); // clever char math!
            //            break;
            //    }
            //}
        }
        return score;
    }

    /// <summary>
    /// Checks if any player remain active
    /// </summary>
    /// <returns>true if any player still can take a turn</returns>
    public bool CheckActivePlayers()
    {
        /* Reminder that var is perfectly OK in C# unlike in JavaScript; it is handy for temporary variables! */
        foreach (var player in players)
        {
            if (player.status == PlayerStatus.active)
            {
                return true; // at least one player is still going!
            }
        }
        return false; // everyone has stayed or busted, or someone won!
    }

    /// <summary>
    /// Check win conditions from best to worst:
    /// player hit 21, player scored highest, player didn't bust
    /// </summary>
    /// <returns>winning player or null if everyone busted</returns>
    public Player DoFinalScoring()
    {
        int highScore = 0;
        foreach (var player in players)
        {
            cardTable.ShowHand(player);
            if (player.status == PlayerStatus.win) // someone hit 21
            {
                return player;
            }
            if (player.status == PlayerStatus.stay) // still could win...
            {
                if (player.score > highScore)
                {
                    highScore = player.score;
                }
            }
            if (highScore > 0) // someone scored, anyway!
            {
                // find the FIRST player in list who meets win condition
                return players.Find(player => player.score == highScore);
            }
            // if busted don't bother checking!
        }

        return null; // everyone must have busted because nobody won!
    }
}
}public class Deck
{
    List<Card> cards = new List<Card>();

    /// <summary>
    /// Constructor for an ordered deck of cards. Define long name and ID
    /// </summary>
    public Deck()
    {
        Console.WriteLine("*********** Building deck...");
        string[] suits = { "S", "H", "C", "D" };

        for (int cardVal = 1; cardVal <= 13; cardVal++)
        {
            foreach (string cardSuit in suits)
            {
                string cardName;
                string cardLongName;

                switch (cardVal)
                {
                    case 1:
                        cardName = "A";
                        cardLongName = "Ace of ";
                        break;
                    case 11:
                        cardName = "J";
                        cardLongName = "Jack of ";
                        break;
                    case 12:
                        cardName = "Q";
                        cardLongName = "Queen of ";
                        break;
                    case 13:
                        cardName = "K";
                        cardLongName = "King of ";
                        break;
                    default:
                        cardName = cardVal.ToString();
                        cardLongName = cardVal.ToString() + " of ";
                        break;
                }

                switch (cardSuit)
                {
                    case "S":
                        cardLongName += "Spades";
                        break;
                    case "H":
                        cardLongName += "Hearts";
                        break;
                    case "C":
                        cardLongName += "Clubs";
                        break;
                    case "D":
                        cardLongName += "Diamonds";
                        break;
                }
                cards.Add(new Card { ID = cardName + cardSuit, name = cardLongName });
            }
        }
    }

    /// <summary>
    /// Randomly swap cards to shuffle the deck.
    /// </summary>
    public void Shuffle()
    {
        Console.WriteLine("Shuffling Cards...");

        Random rng = new Random(); // rng is short for "Random Number Generator"

        // one-line method that uses Linq
        // (only uncomment this and comment out the multi-line approach
        // after you understand this approach!):
        // cards = cards.OrderBy(a => rng.Next()).ToList();

        // multi-line approach that uses Array notation on a list!
        // (this should be easier to understand):
        for (int i = 0; i < cards.Count; i++)
        {
            Card tmp = cards[i];
            int swapindex = rng.Next(cards.Count);
            cards[i] = cards[swapindex];
            cards[swapindex] = tmp;
        }
    }


    /// <summary>
    /// Shows all cards. Kinda hacky. See comment below.
    /// </summary>
    /* Maybe we can make a variation on this that's more useful,
     * but at the moment it's just really to confirm that our 
     * shuffling method(s) worked! And normally we want our card 
     * table to do all of the displaying, don't we?!
     */
    public void ShowAllCards()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            Console.Write(i + ":" + cards[i].ID); // NOTE: a list property can be accessed by an index just like an Array!
            if (i < cards.Count - 1)
            {
                Console.Write(" ");
            }
            else
            {
                Console.WriteLine("");
            }
        }
    }
    /// <summary>
    /// Remove top card (defined here as last card in the list), an instance of Card
    /// </summary>
    /// <returns>the removed instance of Card, representing one of the 52 cards in the deck</returns>
    public Card DealTopCard()
    {
        Card card = cards[cards.Count - 1];
        cards.RemoveAt(cards.Count - 1);
        // Console.WriteLine("I'm giving you " + card);
        return card;
    }
}
