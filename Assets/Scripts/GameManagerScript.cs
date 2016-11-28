using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class GameManagerScript : MonoBehaviour {
	public ComboTextScript comboText; //Text for combo
	public ScoreTextScript scoreText; //Text for ongoing score
	public Text resultScoreText; //Text of score on result pane
	public CountdownScript countdown; //Game ongoing coundown
	public StartCountdownScrpt startCountdown; //Countdown before game starts
	public Image resultPanel; //Result panel to be shwon when game ends
	public Button hintButton; //Hint button to be pressd when nothing noves
	public Color readyColor; //Color of text when game is ready to be played
	public Color notReadyColor; //Color of text when game is not ready to be played
	public Sprite[] sprites; //Sprites of the balls
	public Sprite bombSprite;
	public BallGeneratorScript ballGenerator; //Ball Generator is needed to get the balls and their conditions
	public LineManagerScript lineManagerScript; //Line Manager that manage user interface
	public float raycastWidth = 1.0f; //Width of raycast used for chain
	public float time;
	bool isPlaying; //Bool to check if game is playing or not

	// Use this for initialization
	void Start () {
		countdown.CountdownTime = time;
		StartCountdown ();
	}

	// Update is called once per frame
	void Update () {
		if (IsPlaying && !ballGenerator.IsRecycling) {
			if (IsAllBallNotMoving ()) {
				ballGenerator.DisableBallRigidbodies ();
				hintButton.interactable = true;
			} else {
				hintButton.interactable = false;
			}
		} else {
			hintButton.interactable = false;
		}
	}

	//Called everytime start countdown needs to be started , GameOver need to be called before this
	public void StartCountdown(){
		IsPlaying = false;
		scoreText.Score = 0;
		resultPanel.gameObject.SetActive (false);
		startCountdown.Restart ();
		startCountdown.gameObject.SetActive (true);
	}

	//Called when start coundown ends
	public void Restart(){
		IsPlaying = true;
		lineManagerScript.enabled = true;
		lineManagerScript.Restart ();
		ballGenerator.Restart ();
		countdown.Restart ();
		startCountdown.gameObject.SetActive (false);
	}

	//Called when game ends
	public void GameOver(){
		IsPlaying = false;
		resultScoreText.text = scoreText.Score.ToString ("000000");
		lineManagerScript.Restart ();
		lineManagerScript.enabled = false;
		ballGenerator.RetrieveAllBalls ();
		resultPanel.gameObject.SetActive (true);
	}

	//Reload the balls and set their animation to Idle
	public void RefreshAllBalls(){
		IEnumerator enumerator = ballGenerator.transform.GetEnumerator ();
		while (enumerator.MoveNext ()) {
			Transform ballTransform = enumerator.Current as Transform;
			ballTransform.GetComponent<BallScript> ().Idle();
		}
	}

	//Check if all balls is moving or not
	public bool IsAllBallNotMoving(){
//		IEnumerator enumerator = ballGenerator.GetAllBalls();
//		while(enumerator.MoveNext ()){
//			Transform child = enumerator.Current as Transform;
//			BallScript ball = child.GetComponent<BallScript>();
//			if(ball.IsMoving()){
//				return false;
//			}
//		};
		return true;
	}

	//Add score and put it into ScoreText
	public void AddScore(int scoreAdded){
		scoreText.Score += scoreAdded;
	}


	//Add 1 combo point
	public void AddCombo(){
		comboText.Combo++;
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
