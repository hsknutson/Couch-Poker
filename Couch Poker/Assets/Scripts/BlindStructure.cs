using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlindStructure : MonoBehaviour {
	private static BlindStructure instance;
    public static BlindStructure Instance { get { return instance; } }
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

	[SerializeField] Image displayPanel;
	[SerializeField] Text displayText;

	public int StartingChipCount {get; set;}
	int currentLevel;
	int levelOfCurrentHand;

	public float BlindInterval {get; set;}
	public float CurrentBlindLevelStartTime {get; set;}

	bool hasBeenInitialized = false;

	void Start() {
		StartingChipCount = 1500;
		BlindInterval = 2*60;
	}

	IEnumerator ShowBlindLevelChange() {
		levelOfCurrentHand = currentLevel;
		displayPanel.color = new Color(displayPanel.color.r, displayPanel.color.g, displayPanel.color.b, 0.5f);
		if (GetAnteSize() > 0) {
			displayText.text = "Blinds have been changed to " + GetSmallBlindSize() + "/" + GetBigBlindSize() + "\n" + "Antes have been changed to " + GetAnteSize();
		}
		else {
			displayText.text = "Blinds have been changed to " + GetSmallBlindSize() + "/" + GetBigBlindSize();
		}
		yield return new WaitForSeconds(3);
		displayPanel.color = new Color(displayPanel.color.r, displayPanel.color.g, displayPanel.color.b, 0f);
		displayText.text = "";
	}

	void Update() {
		if (hasBeenInitialized) {
			if (Time.time - CurrentBlindLevelStartTime > BlindInterval) {
				if (currentLevel < 30) {
					showNewBlindLevel = true;
					currentLevel++;
					CurrentBlindLevelStartTime = Time.time;
				}	
			}
		}
	}

	bool showNewBlindLevel = false;

	public void CheckForNewBlindLevel() {
		if (showNewBlindLevel) {
			StartCoroutine(ShowBlindLevelChange());
			showNewBlindLevel = false;
		}
	}

	public void Initialize() {
		currentLevel = 1;
		levelOfCurrentHand = 1;
		CurrentBlindLevelStartTime = Time.time;
		hasBeenInitialized = true;
	}

	class BlindLevel {
		public BlindLevel (int ante, int smallBlind, int bigBlind) {
			this.ante = ante;
			this.smallBlind = smallBlind;
			this.bigBlind = bigBlind;
		}
		public int ante;
		public int smallBlind;
		public int bigBlind;
	}

	List<BlindLevel> blindLevels = new List<BlindLevel> {
		new BlindLevel(0,10,20),
		new BlindLevel(0,15,30),
		new BlindLevel(0,25,50),
		new BlindLevel(0,50,100),
		new BlindLevel(0,75,150),
		new BlindLevel(0,100,200),
		new BlindLevel(30,150,300),
		new BlindLevel(40,200,400),
		new BlindLevel(50,250,500),
		new BlindLevel(60,300,600),
		new BlindLevel(80,400,800),
		new BlindLevel(120,600,1200),
		new BlindLevel(160,800,1600),
		new BlindLevel(200,1200,2000),
		new BlindLevel(300,1500,3000),
		new BlindLevel(400,2000,4000),
		new BlindLevel(500,2500,5000),
		new BlindLevel(600,3000,6000),
		new BlindLevel(800,4000,8000),
		new BlindLevel(1200,6000,12000),
		new BlindLevel(1600,8000,16000),
		new BlindLevel(2000,10000,20000),
		new BlindLevel(3000,15000,30000),
		new BlindLevel(4000,20000,40000),
		new BlindLevel(5000,25000,50000),
		new BlindLevel(6000,30000,60000),
		new BlindLevel(8000,40000,80000),
		new BlindLevel(12000,60000,120000),
		new BlindLevel(16000,80000,160000),
		new BlindLevel(20000,100000,200000),
	};

	public int GetAnteSize() {
		return blindLevels[levelOfCurrentHand-1].ante;
	}
	
	public int GetSmallBlindSize() {
		return blindLevels[levelOfCurrentHand-1].smallBlind;
	}

	public int GetBigBlindSize() {
		return blindLevels[levelOfCurrentHand-1].bigBlind;
	}

	public int GetLevel(int bigBlind) {
		for (int i = 0; i < blindLevels.Count; i++) {
			if (blindLevels[i].bigBlind >= bigBlind)
				return (i + 1);
		}
		return 1;
	}
}
