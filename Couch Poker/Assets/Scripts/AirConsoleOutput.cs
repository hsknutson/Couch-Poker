using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;

public class AirConsoleOutput : MonoBehaviour {
	private static AirConsoleOutput instance;
	public static AirConsoleOutput Instance { get { return instance; } }
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

	public void MuckCards(Player player) {
		var data = new {
			action = "MUCK_CARDS"
		};
		AirConsole.instance.Message(player.DeviceId, data);
	}

	public void UpdateChipCount(Player player) {
		var data = new {
			action = "UPDATE_CHIP_COUNT",
			chipCount = player.Stack
		};
		AirConsole.instance.Message(player.DeviceId, data);
	}

	public void GameHasBegun() {
		var data = new {
			action = "GAME_HAS_STARTED"
		};
		foreach (Player player in PlayerManager.Instance.GetPlayers()) {
			AirConsole.instance.Message(player.DeviceId, data);
		}
	}

	public void HideActivePlayer() {
		var data = new {
			action = "HIDE_ACTIVE_PLAYER"
		};
		foreach (Player player in PlayerManager.Instance.GetPlayers()) {
			AirConsole.instance.Message(player.DeviceId, data);
		}
	}

	public void HideActionControls(Player player) {
		var data = new {
			action = "HIDE_ACTION_CONTROLS"
		};
		AirConsole.instance.Message(player.DeviceId, data);
	}

	public void RoundEnd() {
		var data = new {
			action = "ROUND_END"
		};
		foreach (Player player in PlayerManager.Instance.GetPlayers()) {
			AirConsole.instance.Message(player.DeviceId, data);
		}
	}

	public void ChangeActivePlayer(Player currentPlayer, int CallAmount, int highestWager, int lastRaiseAmount) {
        Debug.Log("pot size: " + Pot.Instance.GetPotSize());
		var data = new {
			action = "YOUR_TURN",

			canFold = CouchPoker.Action.CanFold(currentPlayer.Wager, highestWager),
			canCheck = CouchPoker.Action.CanCheck(currentPlayer.Wager, highestWager),
			canCall = CouchPoker.Action.CanCall(currentPlayer.Wager, highestWager),
			canBet = CouchPoker.Action.CanBet(currentPlayer.Wager, highestWager),
			canRaise = CouchPoker.Action.CanRaise(currentPlayer.Stack, currentPlayer.Wager, highestWager),

			callAmount = CallAmount,
			minimumBetAmount = Math.Min(BlindStructure.Instance.GetBigBlindSize(), currentPlayer.Stack),
			minimumRaiseAmount = Math.Min(currentPlayer.Wager + CallAmount + lastRaiseAmount, currentPlayer.Stack),

			betInterval = BlindStructure.Instance.GetBigBlindSize(),
			raiseInterval = lastRaiseAmount,

			wager = currentPlayer.Wager,

			maximumBetAmount = currentPlayer.Stack,
			maximumRaiseAmount = currentPlayer.Stack + currentPlayer.Wager,
			potSize = Pot.Instance.GetPotSize()
		};
		AirConsole.instance.Message(currentPlayer.DeviceId, data);

        var message = new {
			action = "NOT_YOUR_TURN",
			active_player_name = currentPlayer.Name
		};
		foreach (Player player in PlayerManager.Instance.GetPlayers()) {
			if (player.DeviceId == currentPlayer.DeviceId)
				continue;
			AirConsole.instance.Message(player.DeviceId, message);
		}
	}

	public void SetCards(Player player) {
		List<Card> cards = player.GetHand();
		var data = new {
			action = "YOUR_CARDS",
			holeCard1 = cards[0].GetRankAsString() + cards[0].GetSuitAsString(),
			holeCard2 = cards[1].GetRankAsString() + cards[1].GetSuitAsString()
		};
		AirConsole.instance.Message(player.DeviceId, data);
	}

	
}
