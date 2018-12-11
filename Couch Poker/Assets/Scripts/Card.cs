using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum Rank {
	TWO=2,
	THREE=3,
	FOUR=4,
	FIVE=5,
	SIX=6,
	SEVEN=7,
	EIGHT=8,
	NINE=9,
	TEN=10,
	JACK=11,
	QUEEN=12,
	KING=13,
	ACE=14
}

public enum Suit {
	CLUBS=1,
	DIAMONDS=2,
	HEARTS=3,
	SPADES=4
}

public class Card : MonoBehaviour {
	public Sprite Sprite { get; set;}
    public Suit Suit { get; set; }
	public Rank Rank { get; set; }

	public string GetRankAsString() {
		switch (Rank) {
			case Rank.TWO: return "2";
			case Rank.THREE: return "3";
			case Rank.FOUR: return "4";
			case Rank.FIVE: return "5";
			case Rank.SIX: return "6";
			case Rank.SEVEN: return "7";
			case Rank.EIGHT: return "9";
			case Rank.NINE: return "9";
			case Rank.TEN: return "T";
			case Rank.JACK: return "J";
			case Rank.QUEEN: return "Q";
			case Rank.KING: return "K";
			case Rank.ACE: return "A";
			default: return "";
		}
	}

	public string GetSuitAsString() {
		switch (Suit) {
			case Suit.CLUBS: return "c";
			case Suit.DIAMONDS: return "d";
			case Suit.HEARTS: return "h";
			case Suit.SPADES: return "s";
			default: return "";
		}
	}

	public override string ToString() {
		return GetRankAsString() + GetSuitAsString();
	}
}