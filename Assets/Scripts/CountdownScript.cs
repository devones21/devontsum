using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CountdownScript : MonoBehaviour {
	float countdownTime;
	public GameManagerScript gameManager;
	float timeLeft;
	int lastTime = 0;
	Text countdownText;

	// Use this for initialization
	void Start () {
		countdownText = GetComponent<Text> ();
	}

	//Restart countdown
	public void Restart(){
		timeLeft = countdownTime + 1;
	}
	
	// Update is called once per frame
	void Update () {
		if (gameManager.IsPlaying) {
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
		}
	}

	//Generate text for countdown from second
	string convertSecondToTimeFormat(int second){
		lastTime = second;
		int minute = second / 60;
		second = second % 60; 
		string minuteText = minute.ToString("00");
		string secondText = second.ToString("00");
		return minuteText + ":" + secondText;
	}

	public float CountdownTime{
		get{
			return countdownTime;
		}
		set{
			countdownTime = value;
		}
	}
}
