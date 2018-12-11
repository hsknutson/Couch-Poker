using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {
	private static Board instance;
    public static Board Instance { get { return instance; } }
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

	[SerializeField] private Transform cardPlaceholder1;
	[SerializeField] private Transform cardPlaceholder2;
	[SerializeField] private Transform cardPlaceholder3;
	[SerializeField] private Transform cardPlaceholder4;
	[SerializeField] private Transform cardPlaceholder5;

	Card boardCard1;
	Card boardCard2;
	Card boardCard3;
	Card boardCard4;
	Card boardCard5;

	float cardWidth;
	float cardHeight;

	public List<Card> GetCardsOnBoard() {
		List<Card> cardsOnBoard = new List<Card>();
		if (boardCard1 != null)
			cardsOnBoard.Add(boardCard1);
		if (boardCard2 != null)
			cardsOnBoard.Add(boardCard2);
		if (boardCard3 != null)
			cardsOnBoard.Add(boardCard3);
		if (boardCard4 != null)
			cardsOnBoard.Add(boardCard4);
		if (boardCard5 != null)
			cardsOnBoard.Add(boardCard5);
		return cardsOnBoard;
	}

	public void SetFlop(Card card1, Card card2, Card card3) {
		boardCard1 = card1;
		boardCard2 = card2;
		boardCard3 = card3;

		float width = cardPlaceholder1.GetComponent<RectTransform>().rect.width;
		float height = cardPlaceholder1.GetComponent<RectTransform>().rect.height;

		card1.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
		card2.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
		card3.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);

				
		card1.transform.SetParent(cardPlaceholder1, false);
		card2.transform.SetParent(cardPlaceholder2, false);
		card3.transform.SetParent(cardPlaceholder3, false);

	}

	public void Clear() {
		if (boardCard1 != null)
			boardCard1.transform.SetParent(Deck.Instance.transform, false);
		if (boardCard2 != null)
			boardCard2.transform.SetParent(Deck.Instance.transform, false);
		if (boardCard3 != null)
			boardCard3.transform.SetParent(Deck.Instance.transform, false);
		if (boardCard4 != null)
			boardCard4.transform.SetParent(Deck.Instance.transform, false);
		if (boardCard5 != null)
			boardCard5.transform.SetParent(Deck.Instance.transform, false);

		boardCard1 = null;
		boardCard2 = null;
		boardCard3 = null;
		boardCard4 = null;
		boardCard5 = null;
	}

	public void SetTurn(Card card) {
		boardCard4 = card;

		float width = cardPlaceholder1.GetComponent<RectTransform>().rect.width;
		float height = cardPlaceholder1.GetComponent<RectTransform>().rect.height;
		card.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);

		card.transform.SetParent(cardPlaceholder4, false);
	}

	public void SetRiver(Card card) {
		boardCard5 = card;
		
		float width = cardPlaceholder1.GetComponent<RectTransform>().rect.width;
		float height = cardPlaceholder1.GetComponent<RectTransform>().rect.height;
		card.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);

		card.transform.SetParent(cardPlaceholder5, false);
	}
}
