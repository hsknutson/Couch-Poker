using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Seat : MonoBehaviour {
	[SerializeField] private Text nameText;
	[SerializeField] Text stackText;
	[SerializeField] Text betAmountText;
	[SerializeField] Image glowImage;
	[SerializeField] Transform dealerButton;
	[SerializeField] Transform betDisplayPanel;
	[SerializeField] Transform cardPlaceholder1;
	[SerializeField] Transform cardPlaceholder2;
	[SerializeField] Transform holeCardPlaceholder1;
	[SerializeField] Transform holeCardPlaceholder2;

	public Player Player {get; set;}

	public void ShowGlow() {
		glowImage.enabled = true;
	}

	public void HideGlow() {
		Debug.Log("hide glow");
		glowImage.enabled = false;
	}
	public void ShowBet(int amount) {
		betAmountText.text = amount.ToString();
		betDisplayPanel.gameObject.SetActive(true);
	}
	public void HideBet() {
		betDisplayPanel.gameObject.SetActive(false);
	}

	public void HideDealerButton() {
		dealerButton.gameObject.SetActive(false);
	}
	public void ShowDealerButton() {
		dealerButton.gameObject.SetActive(true);
	}

	public Transform GetHoleCardPlaceholder(int index) {
		if (index == 1)
			return holeCardPlaceholder1;
		return holeCardPlaceholder2;
	}

	public void ShowCards() {
		List<Card> cards = Player.GetHand();
		float width = cardPlaceholder1.GetComponent<RectTransform>().rect.width;
		float height = cardPlaceholder1.GetComponent<RectTransform>().rect.height;
		cards[0].GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
		cards[1].GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
		cards[0].transform.SetParent(cardPlaceholder1, false);
		cards[1].transform.SetParent(cardPlaceholder2, false);
		HideHoleCards();
	}

	public void HideHoleCards() {
		if (holeCardPlaceholder1.transform.childCount == 1) {
			Destroy(holeCardPlaceholder1.GetChild(0).gameObject);
		}
		if (holeCardPlaceholder2.transform.childCount == 1) {
			Destroy(holeCardPlaceholder2.GetChild(0).gameObject);
		}
	}

	public Vector2 GetStackTextPosition() {
		return stackText.transform.position;
	}

	public IEnumerator ShowAction(ActionType actionType) {
		if (actionType == ActionType.FOLD) {
			nameText.text = "Fold";
		}
		else if (actionType == ActionType.CHECK) {
			nameText.text = "Check";
		}
		else if (actionType == ActionType.CALL) {
			nameText.text = "Call";
		}
		else if (actionType == ActionType.BET) {
			nameText.text = "Bet";
		}
		else if (actionType == ActionType.RAISE) {
			nameText.text = "Raise";
		}
		yield return new WaitForSeconds(0.75f);
		nameText.text = Player.Name;
	}

	public void HideCards() {
		List<Card> cards = Player.GetHand();
		cards[0].transform.SetParent(Deck.Instance.transform, false);
		cards[1].transform.SetParent(Deck.Instance.transform, false);
	}
	
	public void Initialize(Player player) {
		Player = player;
		HideDealerButton();
		HideBet();
		nameText.text = player.Name;
		stackText.text = player.Stack.ToString();
		gameObject.SetActive(true);
	}

	public void UpdateChipCountText() {
		stackText.text = Player.Stack.ToString();
	}
}
