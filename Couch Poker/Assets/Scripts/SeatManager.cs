using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeatManager : MonoBehaviour {
	private static SeatManager instance;
    public static SeatManager Instance { get { return instance; } }
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

	[SerializeField] List<Seat> seats;
	List<Seat> openSeats;

	System.Random rand = new System.Random();

	// Use this for initialization
	void Start () {
		openSeats = new List<Seat>(seats);
	}
	
	public Seat GetOpenSeat() {
		if (openSeats.Count == 0)
			return null;
		int index = rand.Next(0, openSeats.Count -1);
		Seat seat = openSeats[index];
		openSeats.Remove(seat);
		return seat;
	}

	public List<Seat> GetSeatsInOrder() {
		return seats;
	}
}
