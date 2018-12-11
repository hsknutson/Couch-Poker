using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Deck : MonoBehaviour {
	private static Deck instance;
    public static Deck Instance { get { return instance; } }

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

    System.Random rand = new System.Random();
	public List<Card> Cards {get; set;}
	public RectTransform cardPrefab;

	void Start () {
        CreateDeck();
    }
	
	public void CreateDeck() {
        Cards = new List<Card>();
		foreach(Rank rank in Enum.GetValues(typeof(Rank))) {
			foreach(Suit suit in Enum.GetValues(typeof(Suit))) {
				RectTransform go = Instantiate(cardPrefab);
                Card card = go.GetComponent<Card>();
                card.Rank = rank;
                card.Suit = suit;
                string spriteName = card.GetRankAsString() + card.GetSuitAsString();
				card.Sprite =(Resources.Load("Cards/" + spriteName) as GameObject).GetComponent<SpriteRenderer>().sprite;
                Debug.Log(spriteName);	
				go.GetComponentInChildren<SVGImage>().sprite = card.Sprite;
                Cards.Add(card);
			}
		}
	}

	public Card Next() {
        if (Cards.Count <= 0)
            return null;
        Card card = Cards[0];
		Cards.RemoveAt(0);
		return card;
	}

	public void Shuffle()
    {
        Debug.Log("deck contains : " + Cards.Count + "cards");
        int n = Cards.Count;
        for (int i = 0; i < n; i++)
        {
            int r = i + rand.Next(n - i);
            Card card = Cards[r];
            Cards[r] = Cards[i];
            Cards[i] = card;
        }
    }
}
