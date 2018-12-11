using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SetupManager : MonoBehaviour {

	[SerializeField] RectTransform initialSetupPanel;
	[SerializeField] RectTransform advancedSetupPanel;
	[SerializeField] RectTransform cashGameSetupPanel;
	[SerializeField] RectTransform tournamentSetupPanel;

	[SerializeField] Text estimatedDurationText;
	[SerializeField] Dropdown blindIntervalDropdown;
	[SerializeField] Dropdown startingChipsDropdow;
	[SerializeField] Dropdown numPlayersDropdown;

	void Start() {
		startingChipsDropdow.onValueChanged.AddListener(delegate {
                TournamentDropdownValueChanged();
        });
		blindIntervalDropdown.onValueChanged.AddListener(delegate {
                TournamentDropdownValueChanged();
        });
		numPlayersDropdown.onValueChanged.AddListener(delegate {
                TournamentDropdownValueChanged();
        });
	}

	void TournamentDropdownValueChanged() {
		int totalChips = 0;

		if (startingChipsDropdow.value == 0)
			totalChips = 1500;
		else if (startingChipsDropdow.value == 1)
			totalChips = 3000;
		else if (startingChipsDropdow.value == 2)
			totalChips = 5000;
		else if (startingChipsDropdow.value == 3)
			totalChips = 10000;

		totalChips *= (numPlayersDropdown.value + 2);

		int bigBlindSize = totalChips/20;
		int level = BlindStructure.Instance.GetLevel(bigBlindSize);

		int timeEstimate = 0;
		if (blindIntervalDropdown.value == 0) {
			timeEstimate = level * 3;
		}
		else if (blindIntervalDropdown.value == 1) {
			timeEstimate = level * 5;
		}
		else if (blindIntervalDropdown.value == 2) {
			timeEstimate = level * 6;
		}
		else if (blindIntervalDropdown.value == 3) {
			timeEstimate = level * 10;
		}
		else if (blindIntervalDropdown.value == 4) {
			timeEstimate = level * 12;
		}
		else if (blindIntervalDropdown.value == 5) {
			timeEstimate = level * 15;
		}
		estimatedDurationText.text = "Estimated duration: " + timeEstimate + " minutes";
	}

	bool tournamentSetup = false;

	[SerializeField] Text loadingText;

	public void GoBackToInitialSetup() {
		initialSetupPanel.gameObject.SetActive(true);
		advancedSetupPanel.gameObject.SetActive(false);
		cashGameSetupPanel.gameObject.SetActive(false);
		tournamentSetupPanel.gameObject.SetActive(false);
		tournamentSetup = false;
	}

	public void GoToAdvancedSetup() {
		initialSetupPanel.gameObject.SetActive(false);
		advancedSetupPanel.gameObject.SetActive(true);
	}

	public void GoToCashGameSetup() {
		advancedSetupPanel.gameObject.SetActive(false);
		cashGameSetupPanel.gameObject.SetActive(true);
	}

	public void GoToTournamentSetup() {
		advancedSetupPanel.gameObject.SetActive(false);
		tournamentSetupPanel.gameObject.SetActive(true);
		tournamentSetup = true;
	}

	bool loadScene = false;

	void Update() {
		if (loadScene == true) {
            // ...then pulse the transparency of the loading text to let the player know that the computer is still working.
            //loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, Mathf.PingPong(Time.time, 1));
		}
	}

	public void StartGame() {
		initialSetupPanel.gameObject.SetActive(false);
		advancedSetupPanel.gameObject.SetActive(false);
		cashGameSetupPanel.gameObject.SetActive(false);
		tournamentSetupPanel.gameObject.SetActive(false);
		loadingText.text = "Loading game...";
		loadScene = true;
		StartCoroutine(LoadGame());
	}


    // The coroutine runs on its own at the same time as Update() and takes an integer indicating which scene to load.
    IEnumerator LoadGame() {
        // Start an asynchronous operation to load the scene that was passed to the LoadNewScene coroutine.
        
		Application.backgroundLoadingPriority = ThreadPriority.BelowNormal;
		AsyncOperation async = SceneManager.LoadSceneAsync("Main");
        // While the asynchronous operation to load the new scene is not yet complete, continue waiting until it's done.
        while (!async.isDone) {
            yield return null;
        }

    }
}
