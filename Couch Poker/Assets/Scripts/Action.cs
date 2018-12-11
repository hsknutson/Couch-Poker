using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionType {
	FOLD,
	CHECK,
	CALL,
	BET,
	RAISE
}

namespace CouchPoker
{
	public class Action {
		public ActionType ActionType {get; set;}
		public int Amount {get; set;}

		public Action(ActionType actionType, int amount = 0) {
			ActionType = actionType;
			Amount = amount;
		}

		public bool IsValidAction(Action action, int stack, int wager, int highestWager, int highestRaise) {
			int callAmount = highestWager - wager;
			switch(action.ActionType) {
				case ActionType.FOLD:
					return true;
				case ActionType.CHECK:
					return callAmount == 0;
				case ActionType.CALL:
					if (callAmount == 0)
						return false;
					return stack > 0;
				case ActionType.BET:
				case ActionType.RAISE:
					int raiseAmount = action.Amount - callAmount;
					return (action.Amount <= stack && raiseAmount >= highestRaise)
						|| IsValidAllinAction(action, stack, wager, highestWager, highestRaise);
				default:
					return false;
			}
		}

		public bool IsValidAllinAction(Action action, int stack, int wager, int highestWager, int highestRaise) {
			int callAmount = highestWager - wager;
			switch(action.ActionType) {
				case ActionType.FOLD:
					return false;
				case ActionType.CHECK:
					return false;
				case ActionType.CALL:
					return callAmount >= stack;
				case ActionType.BET:
				case ActionType.RAISE:
					return action.Amount > 0 && action.Amount == stack;
				default:
					return false;
			}
		}

		public static bool CanFold(int wager, int highestWager) {
			int callAmount = highestWager - wager;
			return callAmount > 0;
		}
		public static bool CanCheck(int wager, int highestWager) {
			int callAmount = highestWager - wager;
			return callAmount == 0;
		}
		public static bool CanCall(int wager, int highestWager) {
			int callAmount = highestWager - wager;
			return callAmount > 0;
		}
		public static bool CanBet(int wager, int highestWager) {
			if (RoundManager.Instance.CurrentStreet == Street.PREFLOP)
				return false;
			int callAmount = highestWager - wager;
			return callAmount == 0;
		}
		public static bool CanRaise(int stack, int wager, int highestWager) {
			return (stack - wager) > highestWager && highestWager != 0;
		}
	}
}
