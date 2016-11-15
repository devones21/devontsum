using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManagerScript : MonoBehaviour {
	public Text readyText;
	public Color readyColor;
	public Color notReadyColor;
	public Color[] colors;
	public BallGeneratorScript ballGenerator;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (isAllBallNotMoving ()) {
			readyText.color = readyColor;
		} else {
			readyText.color = notReadyColor;
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
}
