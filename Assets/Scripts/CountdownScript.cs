using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CountdownScript : MonoBehaviour {
	public float time;
	public GameManagerScript gameManager;
	float timeLeft;
	int lastTime = 0;
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
		if (gameManager.IsPlaying) {
			if (gameManager.IsAllBallNotMoving ()) {
				timeLeft -= Time.deltaTime;
				if ((int)timeLeft != lastTime) {
					countdownText.text = convertSecondToTimeFormat ((int)timeLeft);
					if ((int)timeLeft > 10) {
						countdownText.color = Color.black;
					} else {
						countdownText.color = Color.red;
						if ((int)timeLeft == 0) {
							gameManager.GameOver ();
						}
					}
				}
			} else {
				countdownText.color = Color.yellow;
			}
		}
	}

	string convertSecondToTimeFormat(int second){
		lastTime = second;
		int minute = second / 60;
		second = second % 60; 
		string minuteText = minute.ToString("00");
		string secondText = second.ToString("00");
		return minuteText + ":" + secondText;
	}
}
