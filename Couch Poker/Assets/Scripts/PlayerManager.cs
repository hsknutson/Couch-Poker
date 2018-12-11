using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;

public class PlayerManager : MonoBehaviour {
	private static PlayerManager instance;
    public static PlayerManager Instance { get { return instance; } }

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

	public const int MAX_PLAYER_COUNT = 9;
	int currentPlayerCount = 0;

	[SerializeField] Player playerPrefab;

	public Dictionary<int, Player> deviceIdsToPlayers = new Dictionary<int, Player>();
	public Dictionary<Player, int> playersToDeviceIds = new Dictionary<Player, int>();

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public List<Player> GetPlayers() {
		return playersToDeviceIds.Keys.ToList();
	}

	public Player GetPlayerFromDeviceId(int deviceId) {
		return deviceIdsToPlayers[deviceId];
	}

	public void OnConnect(int device_id) {
		Debug.Log("device connected");
		//TODO: reconnect logic
		if (GameManager.Instance.isGameReady) {
			if (currentPlayerCount < MAX_PLAYER_COUNT) {
				var sendData = new {
					action = "CAN_JOIN"
				};
				AirConsole.instance.Message(device_id, sendData);
				Debug.Log(device_id + " can join");
			}
			else { //max player count reached
				var sendData = new {
					action = "MAX_PLAYERS_REACHED"
				};
				AirConsole.instance.Message(device_id, sendData);
			}
		}
		else { //game not ready
			var sendData = new {
				action = "GAME_NOT_READY"
			};
			AirConsole.instance.Message(device_id, sendData);
		}
	}

	public void SeatPlayer(int from, JToken data) {
		if(currentPlayerCount >= MAX_PLAYER_COUNT) {
			var sendData = new {
				action = "MAX_PLAYERS_REACHED"
			};
			AirConsole.instance.Message(from, sendData);
			return;
		}

		Player player = Instantiate(playerPrefab);
		Seat seat = SeatManager.Instance.GetOpenSeat();
		string playerName = (string)data["username"];
		player.Name = playerName;
		player.Stack = BlindStructure.Instance.StartingChipCount;
		player.Seat = seat;
        player.DeviceId = from;
		deviceIdsToPlayers.Add(from, player);
		playersToDeviceIds.Add(player, from);
		seat.Initialize(player);

		currentPlayerCount++;

		if (currentPlayerCount == 1) {
			var sendData = new {
				action = "YOU_ARE_HOST"
			};
			AirConsole.instance.Message(from, sendData);
		}
		else {
			var sendData = new {
				action = "WAIT_FOR_HOST"
			};
			AirConsole.instance.Message(from, sendData);
		}

		AirConsoleOutput.Instance.UpdateChipCount(player);
	}

	public void KickPlayer(Player player) {
		player.Seat.gameObject.SetActive(false);

		var sendData = new {
				action = "KNOCKED_OUT"
			};
		AirConsole.instance.Message(player.DeviceId, sendData);
	}
}
