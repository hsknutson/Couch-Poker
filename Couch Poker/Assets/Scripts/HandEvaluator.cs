using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;

public enum HandCategory {
	HIGH_CARD=1,
	ONE_PAIR=2,
	TWO_PAIR=3,
	THREE_OF_A_KIND=4,
	STRAIGHT=5,
	FLUSH=6,
	FULLL_HOUSE=7,
	FOUR_OF_A_KIND=8,
	STRAIGHT_FLUSH=9
}

public class HandEvaluator : MonoBehaviour {
    private static HandEvaluator instance;
    public static HandEvaluator Instance { get { return instance; } }
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

	int[] HR = new int[32487834];
    void InitTheEvaluator() {
        FileStream fs = new FileStream("Assets/HandRanks.dat", FileMode.Open, FileAccess.Read);

        using (var binaryStream = new BinaryReader(fs))
        {
            for (var i = 0; i < 32487834; i++)
            {
                HR[i] = binaryStream.ReadInt32();
            }
        }
    }

    void Start()
    {
        InitTheEvaluator();
    }

	int GetCardAsInt(Card card) {
		return ((int) card.Rank - 2) * 4 + (int) card.Suit;
	}

    public int GetHandValue(List<Card> cards)
    {
        Debug.Log("hand evluator card count: " + cards.Count);
        if (cards.Count == 7) {
            int i = 0;
            int p = HR[53 + GetCardAsInt(cards[i++])];
            p = HR[p + GetCardAsInt(cards[i++])];
            p = HR[p + GetCardAsInt(cards[i++])];
            p = HR[p + GetCardAsInt(cards[i++])];
            p = HR[p + GetCardAsInt(cards[i++])];
            p = HR[p + GetCardAsInt(cards[i++])];
            return HR[p + GetCardAsInt(cards[i++])];
        }
        else if (cards.Count == 5) {
            int i = 0;
            int p = HR[53 + GetCardAsInt(cards[i++])];
            p = HR[p + GetCardAsInt(cards[i++])];
            p = HR[p + GetCardAsInt(cards[i++])];
            p = HR[p + GetCardAsInt(cards[i++])];
            p = HR[p + GetCardAsInt(cards[i++])];
            return HR[p]; 
        }
        return 0;  
    }

    public int CompareTwoHands(List<Card> A, List<Card> B)
    {
        int handAScore = GetHandValue(A);
        int handBScore = GetHandValue(B);

        if ((handAScore >> 12) > (handBScore >> 12))
            return 1;
        else if ((handAScore >> 12) == (handBScore >> 12))
        {
            if ((handAScore & 0x00000FFF) > (handBScore & 0x00000FFF))
                return 1;
            else if ((handAScore & 0x00000FFF) == (handBScore & 0x00000FFF))
                return 0;
            else
                return -1;
        }
        return -1;
    }

	public HandCategory GetHandCategory(List<Card> cards)
	{
		int val = GetHandValue(cards);
		val = val >> 12;
		return (HandCategory) val;
	}

    // public List<Card> GetBestFiveCardHand(List<Card> cards) { //cards should contain seven items
    //     List<List<Card>> fiveCardCombinations = new List<List<Card>>();

    //     //generate all combinations of five cards
    //     for (int i = 0; i < cards.Count; i++) {
    //         for (int j = i+1; j < cards.Count; j++) {
    //             List<Card> combination = new List<Card>();
    //             foreach (Card card in cards) {
    //                 if (card != cards[i] && card != cards[j])
    //                     combination.Add(card);
    //             }
    //             fiveCardCombinations.Add(combination);
    //         }
    //     }

    //     Dictionary<List<Card>, int> handValues = new Dictionary<List<Card>, int>();
	// 	for (int i = 0; i < fiveCardCombinations.Count; i++) {
    //         List<Card> fiveCardCombo = fiveCardCombinations[i];
	// 		handValues.Add(fiveCardCombo, GetHandValue(fiveCardCombo));
	// 	}
	// 	int maxHandValue = handValues.Values.Max();
	// 	return handValues.Where(x => x.Value == maxHandValue).Select(x => x.Key).First();
    // }
}
