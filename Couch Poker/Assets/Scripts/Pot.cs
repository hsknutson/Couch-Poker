using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Pot : MonoBehaviour {
	private static Pot instance;
    public static Pot Instance { get { return instance; } }
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

	int chipCount = 0;
	[SerializeField] Text totalPotText;
	[SerializeField] Text mainPotText;
	[SerializeField] Text sidePotText;
	[SerializeField] Image totalPotTextBackground;
	[SerializeField] Image mainPotTextBackground;
	[SerializeField] Image sidePotTextBackground;

	[SerializeField] GameObject chipDisplay;
	[SerializeField] Transform background;

	public void AddChips(int amount) {
		chipCount += amount;
		UpdateDisplay();
	}

	public void SubtractChips(int amount) {
		chipCount -= amount;
		UpdateDisplay();
	}

	class SidePotHelper {
		public SidePotHelper(Player player, int chipsCommitted) {
			this.player = player;
			this.chipsCommitted = chipsCommitted;
		}
		public Player player;
		public int chipsCommitted;
	}

	public void UpdateDisplay() {
		LinkedList<Player> players = RoundManager.Instance.Players;

		List<Player> playersInTotalWagerOrder = players.Where(x => x.StillInHand() && x.ChipsCommitted > 0).OrderBy(x => x.ChipsCommitted).ToList();

		List<SidePotHelper> sidePotHelpers = new List<SidePotHelper>();
		foreach (Player player in playersInTotalWagerOrder) {
			sidePotHelpers.Add(new SidePotHelper(player, player.ChipsCommitted));
		}

		List<int> pots = new List<int>();

		foreach (SidePotHelper sidePotHelper in sidePotHelpers) {
			if (sidePotHelper.player.IsAllin()) {
				int amountOfChipsCommitted = sidePotHelper.player.ChipsCommitted;
				int potSize = 0;
				foreach (SidePotHelper sidePotHelper2 in sidePotHelpers) {
					if (sidePotHelper2.chipsCommitted > 0) {
						if (sidePotHelper2.chipsCommitted <= amountOfChipsCommitted) {
							potSize += sidePotHelper2.chipsCommitted;
							sidePotHelper2.chipsCommitted = 0;
						}
						else {
							potSize += amountOfChipsCommitted;
							sidePotHelper2.chipsCommitted -= amountOfChipsCommitted;
						}
						
					}	
				}
				if (sidePotHelpers.Count(x => x.chipsCommitted > 0) >= 2)
					pots.Add(potSize);
			}
		}

		if (pots.Count > 0) {
			int chipsRemaining = chipCount;
			int mainPotAmount = pots[0];
			chipsRemaining -= mainPotAmount;
			List<int> sidePots = new List<int>();
			for (int i = 1; i < pots.Count; i++) {
				int sidePotAmount = pots[i];
				sidePots.Add(pots[i]);
				chipsRemaining -= pots[i];
			}
			sidePots.Add(chipsRemaining);

			totalPotText.text = "Total pot: " + chipCount;
			totalPotTextBackground.enabled = true;
			mainPotText.text = "Main pot: " + mainPotAmount;
			mainPotTextBackground.enabled = true;

			sidePotText.text = "Side pot(s): ";
			for (int i = 0; i < sidePots.Count; i++) {
				if (i == 0)
					sidePotText.text += sidePots[i];
				else
					sidePotText.text += ", " + sidePots[i];
			}
			sidePotTextBackground.enabled = true;
		}
		else {
			totalPotText.text = "Total pot: " + chipCount;
			mainPotText.text = "";
			sidePotText.text = "";
			totalPotTextBackground.enabled = true;
			mainPotTextBackground.enabled = false;
			sidePotTextBackground.enabled = false;
		}
	}

	public void Clear() {
		chipCount = 0;
		totalPotText.text = "Total pot: 0";
		totalPotTextBackground.enabled = true;
	}

	public int GetPotSize() {
		return chipCount;
	}

	class SplitPot {
		public List<Player> players;
		public int potSize;
	}

	Player GetPlayerClosestToDealer(List<Player> playersToSelectFrom, List<Player> playersInOrder, Player dealer) {
		int index = playersInOrder.FindIndex(x => x == dealer);
		index = (index + 1) % playersInOrder.Count;
		while (!playersToSelectFrom.Contains(playersInOrder[index])) {
			index = (index + 1) % playersInOrder.Count;
		}
		return playersInOrder[index];
	}

	public IEnumerator DividePot(List<Player> winningPlayers, List<Player> playersInOrder, Player dealer) {
		List<Player> playersInTotalWagerOrder = playersInOrder.Where(x => x.ChipsCommitted > 0).OrderBy(x => x.ChipsCommitted).ToList();

		List<SidePotHelper> sidePotHelpers = new List<SidePotHelper>();
		foreach (Player player in playersInTotalWagerOrder) {
			sidePotHelpers.Add(new SidePotHelper(player, player.ChipsCommitted));
		}

		List<SplitPot> splitPots = new List<SplitPot>();

		foreach (SidePotHelper sidePotHelper in sidePotHelpers) {
			if (!sidePotHelper.player.StillInHand())
				continue;
			int amountOfChipsCommitted = sidePotHelper.player.ChipsCommitted;
			List<Player> playersInvolved = new List<Player>();
			int potSize = 0;
			foreach (SidePotHelper sidePotHelper2 in sidePotHelpers) {
				if (sidePotHelper2.chipsCommitted >= amountOfChipsCommitted) {
					potSize += amountOfChipsCommitted;
					sidePotHelper2.chipsCommitted -= amountOfChipsCommitted;
					if (sidePotHelper2.player.StillInHand() && winningPlayers.Contains(sidePotHelper2.player))
						playersInvolved.Add(sidePotHelper2.player);
				}
				else if (sidePotHelper2.chipsCommitted > 0) {
					potSize += sidePotHelper2.chipsCommitted;
					sidePotHelper2.chipsCommitted = 0;
				}	
			}
			if (potSize > 0 && playersInvolved.Count > 0) {
				SplitPot splitPot = new SplitPot();
				splitPot.potSize = potSize;
				splitPot.players = playersInvolved;
				splitPots.Add(splitPot);
			}
		}

		Dictionary<Player, int> playersToPotShare = new Dictionary<Player, int>();
		foreach (SplitPot splitPot in splitPots) {
			if (splitPot.players.Count > 1 && splitPot.potSize % 2 == 1) {
				int shareAmount = (splitPot.potSize - 1) / winningPlayers.Count;
				Player playerClosestToDealer = GetPlayerClosestToDealer(splitPot.players, playersInOrder, dealer);
				foreach (Player player in splitPot.players) {
					if (player == playerClosestToDealer) {
						if (playersToPotShare.ContainsKey(player))
							playersToPotShare[player] += (shareAmount + 1);
						else
							playersToPotShare.Add(player, shareAmount+1);
					}
					else {
						if (playersToPotShare.ContainsKey(player))
							playersToPotShare[player] += shareAmount;
						else
							playersToPotShare.Add(player, shareAmount);
					}
					
				}
			}
			else {
				int shareAmount = splitPot.potSize / splitPot.players.Count;
				foreach (Player player in splitPot.players) {
					if (playersToPotShare.ContainsKey(player))
							playersToPotShare[player] += shareAmount;
					else
						playersToPotShare.Add(player, shareAmount);
				}
			}
		}

		yield return DividePotLast(playersToPotShare);
	}

	IEnumerator DividePotLast(Dictionary<Player, int> playersToPotShare) {
		GameObject totalPotDisplay = Instantiate(chipDisplay);
		totalPotDisplay.GetComponentInChildren<Text>().text = chipCount.ToString();
		totalPotDisplay.transform.SetParent(background, false);
		totalPotText.text = "";
		mainPotText.text = "";
		sidePotText.text = "";
		totalPotTextBackground.enabled = false;
		mainPotTextBackground.enabled = false;
		sidePotTextBackground.enabled = false;
		totalPotDisplay.transform.position = totalPotText.transform.position;
		yield return new WaitForSeconds(1.0f);

		Destroy(totalPotDisplay);

		foreach (KeyValuePair<Player, int> kvp in playersToPotShare) {
			GameObject go = Instantiate(chipDisplay);
			go.GetComponentInChildren<Text>().text = kvp.Value.ToString();
			go.transform.SetParent(background, false);
			go.transform.position = totalPotText.transform.position;
			go.GetComponentInChildren<Text>().text = kvp.Value.ToString();
			StartCoroutine(MovePotShareToPlayer(go, kvp.Key, kvp.Value));
		}
		yield return new WaitForSeconds(1.5f);
		yield return new WaitForSeconds(0.5f);
		
		Clear();
	}

	IEnumerator MovePotShareToPlayer(GameObject potShareDisplay, Player player, int amount) {
		iTween.MoveTo(potShareDisplay, player.Seat.GetStackTextPosition(), 1.5f);
		yield return new WaitForSeconds(1.5f);
		player.AddChipsToStack(amount);
		Destroy(potShareDisplay);
	}
}
