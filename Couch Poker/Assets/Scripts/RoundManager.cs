using System.Collections;
using System.Collections.Generic;
using CouchPoker;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum Street {PREFLOP, FLOP, TURN, RIVER}

public class RoundManager : MonoBehaviour {
	private static RoundManager instance;
    public static RoundManager Instance { get { return instance; } }
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

	int roundNumber = 0;
	public Street CurrentStreet {get; set;}

	public LinkedList<Player> Players {get; set;}

	LinkedListNode<Player> dealer;
	LinkedListNode<Player> smallBlind;
	LinkedListNode<Player> bigBlind;
	LinkedListNode<Player> current;
	LinkedListNode<Player> lastPlayerToAct;
	int lastRaiseAmount;

	public int GetHighestWager() {
		int highestWager = 0;
		LinkedListNode<Player> temp = Players.First;
		while (temp != null) {
			Player player = temp.Value;
			if (player.Wager > highestWager)
				highestWager = player.Wager;
            temp = temp.Next;
		}
		return highestWager;
	}
	public int GetCallAmount() {
		int callAmount = GetHighestWager() - current.Value.Wager;
		// if (callAmount < BlindStructure.Instance.GetBigBlindSize() && GetNumPlayersThatCanAct() > 1)
		// 	callAmount = BlindStructure.Instance.GetBigBlindSize();
		if (current.Value.Stack < callAmount)
			callAmount = current.Value.Stack;
		return callAmount;
	}
	//returns the number of players that are still in the hand (haven't folded and not busto)
	public int GetNumPlayersStillInHand() {
		Debug.Log(Players.Count(player => player.StillInHand()));
		return Players.Count(player => player.StillInHand());
	}
	//returns the number of players that still need to act
	public int GetNumPlayersThatCanAct() {
		return Players.Count(x => x.CanAct());
	}

	void InitializeCurrent() {
		if (Players.Count == 2) {
			if (CurrentStreet == Street.PREFLOP) {
				current =  dealer;
			}
			else {
				current = Next(dealer);
			}
		}
		else {
			if (CurrentStreet == Street.PREFLOP) {
				current = GetCurrentOrNextActivePlayer(Next(bigBlind));
			}
			else {
				current = GetCurrentOrNextActivePlayer(Next(dealer));
			}
		}
	}

	void ResetLastPlayerToAct(LinkedListNode<Player> lastRaiser) {
		lastPlayerToAct = Previous(lastRaiser);
		while (!lastPlayerToAct.Value.CanAct())
			lastPlayerToAct = Previous(lastPlayerToAct);
	}

	void SetLastPlayerToAct() {
		if (Players.Count == 2) {
			if (CurrentStreet == Street.PREFLOP) {
				lastPlayerToAct = Next(dealer);
			}
			else {
				lastPlayerToAct = dealer;
			}
			
		}
		else {
			if (CurrentStreet == Street.PREFLOP) {
				lastPlayerToAct = bigBlind;
			}
			else {
				lastPlayerToAct = dealer;
				while (!lastPlayerToAct.Value.CanAct())
					lastPlayerToAct = Previous(lastPlayerToAct);
			}
		}
	}

	LinkedListNode<Player> GetNextActivePlayer() {
		current = Next(current);
		while (!current.Value.CanAct())
			current = Next(current);
		return current;
	}

	LinkedListNode<Player> GetCurrentOrNextActivePlayer(LinkedListNode<Player> playerNode) {
		while (!playerNode.Value.CanAct())
			playerNode = Next(playerNode);
		return playerNode;
	}

	List<Player> GetWinningPlayers() {
		Dictionary<Player, int> handValues = new Dictionary<Player, int>();
		foreach (Player player in Players) {
			if (player.HasFolded() || player.IsOut())
				continue;
			List<Card> sevenCardHand = GetPlayerSevenCardHand(player);
			handValues.Add(player, HandEvaluator.Instance.GetHandValue(sevenCardHand));
		}
		int maxHandValue = handValues.Values.Max();
		return handValues.Where(x => x.Value == maxHandValue).Select(x => x.Key).ToList();
	}

	List<Card> GetPlayerSevenCardHand(Player player) {
		List<Card> cards = player.GetHand();
		cards.AddRange(Board.Instance.GetCardsOnBoard());
		return cards;
	}

	void ApplyAction(Player player, Action action, int callAmount) {
		Debug.Log("apply action part 2");
		if (action.ActionType == ActionType.FOLD) {
			current.Value.Fold();
		}
		else if (action.ActionType == ActionType.CHECK) {
			//do nothing
		}
		else if (action.ActionType == ActionType.CALL) {
			current.Value.PlaceChips(callAmount);
		}
		else if (action.ActionType == ActionType.BET) {
			int amount = action.Amount;
			lastRaiseAmount = amount - GetHighestWager();
			current.Value.PlaceChips(amount);
		}
		else if (action.ActionType == ActionType.RAISE) {
			int amount = action.Amount;
			lastRaiseAmount = amount - GetHighestWager();
			current.Value.PlaceChips(amount-current.Value.Wager);
		}
		current.Value.Seat.HideGlow();
		StartCoroutine(player.Seat.ShowAction(action.ActionType));
		AirConsoleOutput.Instance.HideActionControls(player);
		AirConsoleOutput.Instance.HideActivePlayer();
	}

	IEnumerator ApplyForcedBets() {
		int anteSize = BlindStructure.Instance.GetAnteSize();
		int smallBlindSize = BlindStructure.Instance.GetSmallBlindSize();
		int bigBlindSize = BlindStructure.Instance.GetBigBlindSize();

		if (anteSize > 0) {
			foreach (Player player in Players) {
				if (!player.IsOut())
					player.PayAnte(anteSize);
			}
		}

		smallBlind.Value.PlaceChips(smallBlindSize);
		yield return new WaitForSeconds(0.5f);
		bigBlind.Value.PlaceChips(bigBlindSize);
	}

	IEnumerator ShowDown() {
		ReturnUnmatchedBets();
		CollectWagers();
		List<Player> winningPlayers = GetWinningPlayers();
		PrintPlayerHandValues(Players.ToList());
		ShowPlayerHands(winningPlayers);
		yield return new WaitForSeconds(5);
		yield return Pot.Instance.DividePot(winningPlayers, Players.ToList(), dealer.Value);
		foreach (Player player in Players) {
			if (!player.HasFolded() && !player.IsOut())
				player.Seat.HideCards();
		}

		foreach (Player player in Players) {
			if (player.Stack <= 0)
				PlayerManager.Instance.KickPlayer(player);
		}
		UncommitChips();
		dealer.Value.Seat.HideDealerButton();
		Dealer.Instance.ResetDeck();
		AirConsoleOutput.Instance.RoundEnd();
		StartCoroutine(InitializeRound());
		yield return null;
	}

	void PrintPlayerHandValues(List<Player> players) {
		foreach (Player player in players) {
			if (player.IsOut() || player.HasFolded())
				continue;
			List<Card> sevenCardHand = GetPlayerSevenCardHand(player);
			Debug.Log(player.Name + "'s hand value is " + HandEvaluator.Instance.GetHandValue(sevenCardHand));
		}
	}

	IEnumerator Uncontested() {
		Debug.Log("uncontested pot");
		ReturnUnmatchedBets();
		CollectWagers();
        Player player = GetLastPlayerInHand();
		yield return Pot.Instance.DividePot(new List<Player> {player}, Players.ToList(), player);
		player.Seat.HideHoleCards();
		UncommitChips();
		dealer.Value.Seat.HideDealerButton();
		Dealer.Instance.ResetDeck();
		AirConsoleOutput.Instance.RoundEnd();
		StartCoroutine(InitializeRound());
	}

	bool IsEveryPlayerAllin() {
		Debug.Log("is every player allin");
		int playersStillInHand = 0;
		int playersAllin = 0;
		foreach (Player player in Players) {
			if (player.StillInHand())
				playersStillInHand++;
			if (player.IsAllin())
				playersAllin++;
		}
		if (playersAllin == 0)
			return false;
		if (playersStillInHand - playersAllin == 0)
			return true;
		else if (playersStillInHand - playersAllin == 1) {
			foreach (Player player in Players) {
				if (player.CanAct()) {
					if (GetHighestWager() - player.Wager > 0)
						return false;
					else
						return true;
				}
			}
		}
		return false;
	}

    Player GetLastPlayerInHand()
    {
        foreach (Player player in Players)
        {
            if (!player.HasFolded() && !player.IsOut())
                return player;
        }
        return null;
    }

	void ShowPlayerHands(List<Player> winningPlayers) {
		foreach (Player player in Players) {
			if (!player.HasFolded() && !player.IsOut())
				player.Seat.ShowCards();
		}
	}
	
	int GetNumPlayersLeftInGame() {
		return Players.Count(x => x.Stack > 0);
	}

	public IEnumerator InitializeRound() {
		Debug.Log("initialize round");
		roundNumber++;
		if (roundNumber != 1 && GetNumPlayersLeftInGame() == 1) {
			yield break;
		}
		CurrentStreet = Street.PREFLOP;
		if (roundNumber == 1) {
			InititializePlayers();
			InitializeButtonAndBlinds();
		}
		else {
			UpdateButtonAndBlinds();
		}
		BlindStructure.Instance.CheckForNewBlindLevel();
		dealer.Value.Seat.ShowDealerButton();
		yield return new WaitForSeconds(0.5f);
		yield return ApplyForcedBets();
		yield return new WaitForSeconds(0.5f);
		yield return Dealer.Instance.DealPreflop(Players.ToList(), dealer.Value);
		InitializeCurrent();
		SetLastPlayerToAct();
		betsSettled = false;
		lastRaiseAmount = BlindStructure.Instance.GetBigBlindSize();
		ChangeActivePlayer();
	}

	void ChangeActivePlayer() {
		Debug.Log("change active player");
		current.Value.Seat.ShowGlow();
		AirConsoleOutput.Instance.ChangeActivePlayer(current.Value, GetCallAmount(), GetHighestWager(), lastRaiseAmount);
	}

	void CollectWagers() {
		foreach (Player player in Players) {
			if (player.Wager > 0) {
				player.Wager = 0;
				player.Seat.HideBet();
			}
		}
	}

	void UncommitChips() {
		foreach (Player player in Players) {
			if (player.ChipsCommitted > 0) {
				player.ChipsCommitted = 0;
			}
		}
	}

	IEnumerator NextStreet() {
		if (GetNumPlayersStillInHand() == 1) { //everyone has folded except one player
			StartCoroutine(Uncontested());
			yield break;
		}
		CollectWagers();
		if (CurrentStreet == Street.PREFLOP) {
			yield return new WaitForSeconds(0.5f);
			CurrentStreet = Street.FLOP;
			Dealer.Instance.DealFlop();
		}
		else if (CurrentStreet == Street.FLOP) {
			yield return new WaitForSeconds(0.5f);
			CurrentStreet = Street.TURN;
			Dealer.Instance.DealTurn();
		}
		else if (CurrentStreet == Street.TURN) {
			yield return new WaitForSeconds(0.5f);
			CurrentStreet = Street.RIVER;
			Dealer.Instance.DealRiver();
		}
		else if (CurrentStreet == Street.RIVER) {
			yield return new WaitForSeconds(0.5f);
			StartCoroutine(ShowDown());
			yield break;
		}

		InitializeCurrent();
		SetLastPlayerToAct();
		betsSettled = false;
		lastRaiseAmount = 0;
		ChangeActivePlayer();
	}

	bool betsSettled;

	public void ApplyAction(Player player, Action action) {
		Debug.Log("apply action part 1");
		if (current.Value != player)
			Debug.Log(player.Name + " acted out of turn");
		if (!action.IsValidAction(action, player.Stack, player.Wager, GetHighestWager(), lastRaiseAmount))
			Debug.Log(player.Name + " took an illegal action.");

		if (action.ActionType == ActionType.FOLD) {
			if (current.Value == lastPlayerToAct.Value)
				betsSettled = true;

		}
		else if (action.ActionType == ActionType.CHECK) {
			if (current.Value == lastPlayerToAct.Value)
				betsSettled = true;
		}
		else if (action.ActionType == ActionType.CALL) {
			if (current.Value == lastPlayerToAct.Value)
				betsSettled = true;
		}
		else {
			ResetLastPlayerToAct(current);
		}
		ApplyAction(player, action, GetCallAmount());
		DoThing();
	}

	IEnumerator DoAllin() {
		ReturnUnmatchedBets();
		CollectWagers();
		foreach (Player player in Players) {
			if (!player.HasFolded() && !player.IsOut())
				player.Seat.ShowCards();
		}
		yield return new WaitForSeconds(1);
		if (CurrentStreet == Street.PREFLOP) {
			Dealer.Instance.DealFlop();
			yield return new WaitForSeconds(1);
			Dealer.Instance.DealTurn();
			yield return new WaitForSeconds(1);
			Dealer.Instance.DealRiver();
		}
		else if (CurrentStreet == Street.FLOP) {
			Dealer.Instance.DealTurn();
			yield return new WaitForSeconds(1);
			Dealer.Instance.DealRiver();
		}
		else if (CurrentStreet == Street.TURN) {
			Dealer.Instance.DealRiver();
		}
		else if (CurrentStreet == Street.RIVER) {
			
		}
		yield return new WaitForSeconds(3);

		List<Player> winningPlayers = GetWinningPlayers();
		yield return Pot.Instance.DividePot(winningPlayers, Players.ToList(), dealer.Value);

		foreach (Player player in Players) {
			if (!player.HasFolded() && !player.IsOut())
				player.Seat.HideCards();
		}

		foreach (Player player in Players) {
			if (player.Stack <= 0)
				PlayerManager.Instance.KickPlayer(player);
		}
		UncommitChips();
		dealer.Value.Seat.HideDealerButton();
		Dealer.Instance.ResetDeck();
		AirConsoleOutput.Instance.RoundEnd();
		StartCoroutine(InitializeRound());
		yield return null;
	}

	void ReturnUnmatchedBets() {
		int highestWager = GetHighestWager();
		if (Players.Count(x => x.StillInHand() && x.Wager == highestWager) >= 2)
			return;
		int secondHighestWager = 0;
		Player playerWithHighestWager = null;
		foreach (Player player in Players) {
			if (player.Wager < highestWager && player.Wager > secondHighestWager)
				secondHighestWager = player.Wager;
			if (player.Wager == highestWager)
				playerWithHighestWager = player;
		}
		playerWithHighestWager.ReturnChips(highestWager - secondHighestWager);
	}

	void DoThing() {
		if (IsEveryPlayerAllin()) {
			StartCoroutine(DoAllin());
			return;
		}
		if (betsSettled) {
			StartCoroutine(NextStreet());
		}
		else if (GetNumPlayersStillInHand() == 1) {
			StartCoroutine(Uncontested());
		}
		else {
			GetNextActivePlayer();
			ChangeActivePlayer();
		}
	}

	void InititializePlayers() {
		Players = new LinkedList<Player>();
		List<Seat> seats = SeatManager.Instance.GetSeatsInOrder();
		foreach (Seat seat in seats) {
			if (seat.Player != null) {
				if (Players.Count == 0)
					Players.AddFirst(seat.Player);
				else
					Players.AddAfter(Players.Last, seat.Player);
			}
		}
	}

	
	public void InitializeButtonAndBlinds() {
		System.Random rand = new System.Random();
		int dealerIndex = rand.Next(0, Players.Count - 1);
		dealer = ElementAt(dealerIndex);
		if (Players.Count == 2) {
			smallBlind = dealer;
			bigBlind = Next(dealer);
		}
		else {
			smallBlind = Next(dealer);
			bigBlind = Next(smallBlind);
		}
	}

	public void UpdateButtonAndBlinds() {
		dealer = Next(dealer);
		while (dealer.Value.IsOut())
			dealer = Next(dealer);
		smallBlind = Next(smallBlind);
		while (smallBlind.Value.IsOut())
			smallBlind = Next(smallBlind);
		bigBlind = Next(bigBlind);
		while (bigBlind.Value.IsOut())
			bigBlind = Next(bigBlind);
	}

	LinkedListNode<Player> Next(LinkedListNode<Player> node) {
		return node.Next ?? node.List.First;
	}

	LinkedListNode<Player> Previous(LinkedListNode<Player> node) {
		return node.Previous ?? node.List.Last;
	}

	LinkedListNode<Player> ElementAt(int n) {
		LinkedListNode<Player> temp = Players.First;
		while (n > 0) {
			temp = temp.Next;
			n--;
		}
		return temp;
	}
}
