using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;

public class AirConsoleInput : MonoBehaviour {
	void OnEnable () {
		AirConsole.instance.onMessage += OnMessage;
		AirConsole.instance.onConnect += OnConnect;
	}

	void OnMessage(int from, JToken data) {
		string action = (string) data["action"];
		if (action == "SHUFFLE_UP_AND_DEAL") {
			GameManager.Instance.StartGame();
		}
		else if (action == "TAKE_SEAT") {
			PlayerManager.Instance.SeatPlayer(from, data);
		}
		else if (action == "FOLD") {
			Player player = PlayerManager.Instance.GetPlayerFromDeviceId(from);
			CouchPoker.Action playerAction = new CouchPoker.Action(ActionType.FOLD);
			RoundManager.Instance.ApplyAction(player, playerAction);
		}
		else if (action == "CHECK") {
			Player player = PlayerManager.Instance.GetPlayerFromDeviceId(from);
			CouchPoker.Action playerAction = new CouchPoker.Action(ActionType.CHECK);
			RoundManager.Instance.ApplyAction(player, playerAction);
		}
		else if (action == "CALL") {
			Player player = PlayerManager.Instance.GetPlayerFromDeviceId(from);
			int callAmount = Int32.Parse((string) data["callAmount"]);
			CouchPoker.Action playerAction = new CouchPoker.Action(ActionType.CALL, callAmount);
			RoundManager.Instance.ApplyAction(player, playerAction);
		}
		else if (action == "BET") {
			Player player = PlayerManager.Instance.GetPlayerFromDeviceId(from);
			int betAmount = Int32.Parse((string) data["betAmount"]);
			CouchPoker.Action playerAction = new CouchPoker.Action(ActionType.BET, betAmount);
			RoundManager.Instance.ApplyAction(player, playerAction);
		}
		else if (action == "RAISE") {
			Player player = PlayerManager.Instance.GetPlayerFromDeviceId(from);
			int raiseAmount = Int32.Parse((string) data["raiseAmount"]);
			CouchPoker.Action playerAction = new CouchPoker.Action(ActionType.RAISE, raiseAmount);
			RoundManager.Instance.ApplyAction(player, playerAction);
		}
	}
	
	void OnConnect(int device_id) {
		PlayerManager.Instance.OnConnect(device_id);
	}
}
