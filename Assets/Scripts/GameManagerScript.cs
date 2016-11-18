using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class GameManagerScript : MonoBehaviour {
	public Text readyText;
	public Text scoreText;
	public Text resultScoreText;
	public CountdownScript countdown;
	public StartCountdownScrpt startCountdown;
	public Image resultPanel;
	public Button hintButton;
	public Color readyColor;
	public Color notReadyColor;
	public Sprite[] sprites;
	public BallGeneratorScript ballGenerator;
	public LineManagerScript lineManagerScript;
	public int scoreLength = 5;
	public float raycastWidth = 1.0f;
	bool isPlaying;
	int score = 0;

	// Use this for initialization
	void Start () {
		StartCountdown ();
	}

	// Update is called once per frame
	void Update () {
		if (IsPlaying) {
			if (IsAllBallNotMoving ()) {
				readyText.color = readyColor;
				hintButton.interactable = true;
			} else {
				readyText.color = notReadyColor;
				hintButton.interactable = false;
			}
		} else {
			readyText.color = notReadyColor;
			hintButton.interactable = false;
		}
	}

	private IEnumerator RestartWithDelay(float delayTime){
		yield return new WaitForSeconds(delayTime);
		Restart ();
	}

	public void StartCountdown(){
		IsPlaying = false;
		score = 0;
		AddScore (0);
		resultPanel.gameObject.SetActive (false);
		startCountdown.Restart ();
		startCountdown.gameObject.SetActive (true);
	}

	public void Restart(){
		IsPlaying = true;
		lineManagerScript.enabled = true;
		lineManagerScript.Restart ();
		ballGenerator.Restart ();
		countdown.Restart ();
		startCountdown.gameObject.SetActive (false);
	}

	public void GameOver(){
		IsPlaying = false;
		Debug.Log (score);
		resultScoreText.text = score.ToString ("000000");
		lineManagerScript.Restart ();
		lineManagerScript.enabled = false;
		ballGenerator.RetrieveAllBalls ();
		resultPanel.gameObject.SetActive (true);
	}

	public void RefreshAllBalls(){
		IEnumerator enumerator = ballGenerator.transform.GetEnumerator ();
		while (enumerator.MoveNext ()) {
			Transform ballTransform = enumerator.Current as Transform;
			ballTransform.GetComponent<BallScript> ().Refresh ();
		}
	}

	public bool IsAllBallNotMoving(){
		IEnumerator enumerator = ballGenerator.GetAllBalls();
		while(enumerator.MoveNext ()){
			Transform child = enumerator.Current as Transform;
			BallScript ball = child.GetComponent<BallScript>();
			if(ball.IsMoving()){
				return false;
			}
		};
		return true;
	}

	public void AddScore(int scoreAdded){
		score += scoreAdded;
		string scoreString = score.ToString ("000000");
		scoreText.text = scoreString;
	}

	public bool IsPlaying{
		get{
			return isPlaying;
		}
		set{
			isPlaying = value;
		}
	}
}
