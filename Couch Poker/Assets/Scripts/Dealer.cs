using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Dealer : MonoBehaviour {
	private static Dealer instance;
    public static Dealer Instance { get { return instance; } }
	private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

	[SerializeField] Transform centerOfBoard;
	[SerializeField] GameObject holeCardPrefab;

	List<Card> muck;

	void Start () {
		muck = new List<Card>();
	}

	public void ResetDeck() {
		Debug.Log("deck size: " + Deck.Instance.Cards.Count);
		foreach (Player player in PlayerManager.Instance.GetPlayers()) {
			if (player.HasCards()) {
				Debug.Log("retrieving cards from player");
				Deck.Instance.Cards.AddRange(player.GetHand());
				player.ClearHand();
			}
		}
		Debug.Log("deck size after retrieving player hands: " + Deck.Instance.Cards.Count);
		Deck.Instance.Cards.AddRange(muck);
		muck.Clear();
		Debug.Log("deck size after retrieving muck: " + Deck.Instance.Cards.Count);
		Deck.Instance.Cards.AddRange(Board.Instance.GetCardsOnBoard());
		Board.Instance.Clear();
		Debug.Log("deck size after retrieving cards on board: " + Deck.Instance.Cards.Count);
	}

	public IEnumerator DealPreflop(List<Player> players, Player dealer) {
		Deck.Instance.Shuffle();
		Debug.Log("deck size before dealing preflop: " + Deck.Instance.Cards.Count);
		int index = players.FindIndex(x => x == dealer);
		index = (index + 1) % players.Count;

		int i = 0;
		while (i < players.Count) {
			if (!players[index].IsOut()) {
				Debug.Log("dealt card to player");
				yield return DealCardToPlayer(players[index], 1);
			}	
			index = (index + 1) % players.Count;
			i++;
		}

		index = players.FindIndex(x => x == dealer);
		index = (index + 1) % players.Count;
		i = 0;
		while (i < players.Count) {
			if (!players[index].IsOut()) {
				Debug.Log("dealt card to player");
				yield return DealCardToPlayer(players[index], 2);
			}	
			index = (index + 1) % players.Count;
			i++;
		}
	}

	public void DealFlop() {
		muck.Add(Deck.Instance.Next());

		Card card1 = Deck.Instance.Next();
		Card card2 = Deck.Instance.Next();
		Card card3 = Deck.Instance.Next();
		Board.Instance.SetFlop(card1, card2, card3);

		Debug.Log("Flop: " + card1.ToString() + card2.ToString() + card3.ToString());
	}

	public void DealTurn() {
		muck.Add(Deck.Instance.Next());

		Card card = Deck.Instance.Next();
		Board.Instance.SetTurn(card);

		Debug.Log("Turn: " + card.ToString());
	}

	public void DealRiver() {
		muck.Add(Deck.Instance.Next());

		Card card = Deck.Instance.Next();
		Board.Instance.SetRiver(card);

		Debug.Log("River: " + card.ToString());
	}

	IEnumerator DealCardToPlayer(Player player, int index) {
		Transform holeCardPlaceholder = player.Seat.GetHoleCardPlaceholder(index);
		GameObject go = Instantiate(holeCardPrefab);
		go.transform.SetParent(holeCardPlaceholder, false);
		go.transform.position = centerOfBoard.position;
		go.GetComponent<SVGImage>().enabled = true;
		iTween.MoveTo(go, holeCardPlaceholder.position, 0.25f);
		yield return new WaitForSeconds(0.25f);
		player.AddCardToHand(Deck.Instance.Next());
	}
}
