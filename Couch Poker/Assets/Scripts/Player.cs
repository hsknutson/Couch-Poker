using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;

public class Player : MonoBehaviour {
	public string Name {get; set;}
	public int DeviceId {get; set;}
	public Seat Seat {get; set;}

	List<Card> hand = new List<Card>();

	public int Stack {get; set;}
	public int Wager {get; set;}
	public int ChipsCommitted {get; set;}

	bool folded;

	public bool HasCards() {
		return hand.Count > 0;
	}
	
	public bool IsAllin() {
		return Stack <=0 && ChipsCommitted > 0;
	}
	public bool IsOut() {
		return Stack <= 0 && ChipsCommitted <= 0;
	}
	public bool HasFolded() {
		return folded;
	}

	public bool StillInHand() {
		return !HasFolded() && !IsOut();
	}
	
	public void AddCardToHand(Card card) {
		hand.Add(card);
		if (hand.Count == 2) {
			AirConsoleOutput.Instance.SetCards(this);
			folded = false;
		}
	}

	public void ClearHand() {
		hand.Clear();
		AirConsoleOutput.Instance.MuckCards(this);
		Seat.HideHoleCards();
	}

	public List<Card> GetHand() {
		return new List<Card>(hand);
	}

	public void Fold() {
		ClearHand();
		folded = true;
	}

	public bool CanAct() {
		return !folded && Stack > 0;
	}

	public void AddChipsToStack(int amount) {
		Stack += amount;
		Seat.UpdateChipCountText();
		AirConsoleOutput.Instance.UpdateChipCount(this);
	}
	
	public void ReturnChips(int amount) {
		Wager -= amount;
		Stack += amount;
		Seat.UpdateChipCountText();
		if (Wager == 0)
			Seat.HideBet();
		else
			Seat.ShowBet(Wager);
		ChipsCommitted -= amount;
		AirConsoleOutput.Instance.UpdateChipCount(this);
		Pot.Instance.SubtractChips(amount);
	}

	public void PlaceChips(int amount) {
		if (amount > Stack) {
			amount = Stack;
		}
		Stack -= amount;
		Seat.UpdateChipCountText();
		Wager += amount;
		Seat.ShowBet(Wager);
		ChipsCommitted += amount;
		AirConsoleOutput.Instance.UpdateChipCount(this);
		Pot.Instance.AddChips(amount);
	}

	public void PayAnte(int amount) {
		if (amount > Stack) {
			amount = Stack;
		}
		Stack -= amount;
		ChipsCommitted += amount;
		Seat.UpdateChipCountText();
		AirConsoleOutput.Instance.UpdateChipCount(this);
		Pot.Instance.AddChips(amount);
	}
}
