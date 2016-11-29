﻿using UnityEngine;
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
	public Color readyColor; //Color of text when game is ready to be played
	public Color notReadyColor; //Color of text when game is not ready to be played
	public Sprite[] sprites; //Sprites of the balls
	public Sprite bombSprite; //Sprite for bomb
	public BallGeneratorScript ballGenerator; //Ball Generator is needed to get the balls and their conditions
	public LineManagerScript lineManagerScript; //Line Manager that manage user interface
	public float raycastWidth = 1.0f; //Width of raycast used for chain
	public float time; //Game time length
	public float hintTime; //Auto hint time
	public int minChainForBomb = 7; //Min of ball need to be chained to make a bomb
	bool isPlaying; //Bool to check if game is playing or not
	float hintTimeLeft; //Auto hint time left

	public float HintTimeLeft{
		get{
			return hintTimeLeft;
		}
		set{
			hintTimeLeft = value;
		}
	}

	// Use this for initialization
	void Start () {
		countdown.CountdownTime = time;
		lineManagerScript.MinChainForBomb = minChainForBomb;
		StartCountdown ();
	}

	// Update is called once per frame
	void Update () {
		if (isPlaying) {
			hintTimeLeft -= Time.deltaTime;
			if (hintTimeLeft < 0.0f && !lineManagerScript.IsChaining()) {
				lineManagerScript.ShowHint ();
				ResetHintCountdown ();
			}
		}
//		if (IsPlaying && !ballGenerator.IsRecycling) {
//			if (IsAllBallNotMoving ()) {
//				ballGenerator.DisableBallRigidbodies ();
//				hintButton.interactable = true;
//			} else {
//				hintButton.interactable = false;
//			}
//		} else {
//			hintButton.interactable = false;
//		}
	}


	//Reset hint time left
	public void ResetHintCountdown(){
		hintTimeLeft = hintTime;
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
		hintTimeLeft = hintTime;
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
		comboText.Idle ();
		comboText.Combo = 0;
		scoreText.Idle ();
		scoreText.Score = 0;
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
