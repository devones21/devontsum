using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class GameManagerScript : MonoBehaviour {
	public Text readyText;
	public Text scoreText;
	public Button hintButton;
	public Color readyColor;
	public Color notReadyColor;
	public Sprite[] sprites;
	public Sprite touchableBallSprite;
	public BallGeneratorScript ballGenerator;
	public int scoreLength = 5;
	public float raycastWidth = 1.0f;
	int score = 0;

	// Use this for initialization
	void Start () {
		score = 0;
		AddScore (0);
	}
	
	// Update is called once per frame
	void Update () {
		if (IsAllBallNotMoving ()) {
			readyText.color = readyColor;
			hintButton.interactable = true;
		} else {
			readyText.color = notReadyColor;
			hintButton.interactable = false;
		}
	}

	public void RefreshAllBalls(){
		IEnumerator enumerator = ballGenerator.transform.GetEnumerator ();
		while (enumerator.MoveNext ()) {
			Transform ballTransform = enumerator.Current as Transform;
			ballTransform.GetComponent<BallScript> ().Refresh ();
		}
	}

	public bool IsAllBallNotMoving(){
		IEnumerator enumerator = ballGenerator.transform.GetEnumerator ();
		while(enumerator.MoveNext ()){
			Transform child = enumerator.Current as Transform;
			BallScript ball = child.GetComponent<BallScript>();
			if(ball.IsMoving()){
				return false;
			}
		};
		return true;
	}

	string GenerateZeroes(int number, int length){
		string result = "";
		if (number.ToString ().Length < length) {
			for (int i = 0; i < length - number.ToString ().Length; i++) {
				result += "0";
			}
			result += number.ToString ();
		} else
			result = number.ToString ();
		return result;
	}

	public void AddScore(int scoreAdded){
		score += scoreAdded;
		string scoreString = GenerateZeroes (score, scoreLength);
		scoreText.text = scoreString;
	}
}
