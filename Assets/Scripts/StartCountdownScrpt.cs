using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StartCountdownScrpt : MonoBehaviour {
	public float time;
	public GameManagerScript gameManager;
	float timeLeft;
	Text countdownText;

	// Use this for initialization
	void Start () {
		countdownText = GetComponent<Text> ();
	}

	public void Restart(){
		timeLeft = time;
	}

	// Update is called once per frame
	void Update () {
		if (!gameManager.IsPlaying) {
			timeLeft -= Time.deltaTime;
			countdownText.text = ((int)timeLeft).ToString ();
			switch ((int)timeLeft) {
			case 3: countdownText.color = Color.green;
					break;
			case 2: countdownText.color = Color.yellow;
					break;
			case 1: countdownText.color = Color.red;
					break;
			case 0: Standby ();
					break;
				
			}
		}
	}

	void Standby(){
		gameManager.Restart ();
	}
}
