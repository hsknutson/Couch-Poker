using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	private static GameManager instance;
    public static GameManager Instance { get { return instance; } }
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

	public bool isGameReady = false;
	public bool hasGameStarted = false;

	// Use this for initialization
	void Start () {
		isGameReady = true;
	}

	public void StartGame() {
		hasGameStarted = true;
		BlindStructure.Instance.Initialize();
		StartCoroutine(RoundManager.Instance.InitializeRound());
		AirConsoleOutput.Instance.GameHasBegun();
	}
}
