using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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
		addScore (0);
	}
	
	// Update is called once per frame
	void Update () {
		if (isAllBallNotMoving ()) {
			readyText.color = readyColor;
			hintButton.interactable = true;
		} else {
			readyText.color = notReadyColor;
			hintButton.interactable = false;
		}
	}

	public void refreshAllBalls(){
		List<BallScript> balls = ballGenerator.getBalls ();
		foreach(BallScript ball in balls){
			ball.refresh ();
		}
	}

	public bool isAllBallNotMoving(){
		bool result = true;
		List<BallScript> balls = ballGenerator.getBalls ();
		foreach(BallScript ball in balls){
			Rigidbody2D rigidbody2D = ball.gameObject.GetComponent<Rigidbody2D> ();
			if(rigidbody2D != null){
				if (Mathf.Abs (rigidbody2D.velocity.y) > 1 || Mathf.Abs (rigidbody2D.velocity.x) > 0.1 ) {
					result = false;
					return result;
				}
			}
		}
		return result;
	}

	string generateZeroes(int number, int length){
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

	public void addScore(int scoreAdded){
		score += scoreAdded;
		string scoreString = generateZeroes (score, scoreLength);
		scoreText.text = scoreString;
	}


}
