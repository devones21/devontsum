using UnityEngine;
using System.Collections.Generic;

public class BallGeneratorScript : MonoBehaviour {
	public GameManagerScript gameManager;
	public int totalBallsGenerated;
	public float ballGenerateTimeInSeconds;
	public Transform ballPrefab;
	int invokedBalls = 0;
	Color[] colors;
	// Use this for initialization
	void Start () {
		Invoke("initiateBall",ballGenerateTimeInSeconds);
	}

	void initiateBall(){
		invokedBalls++;
		if (invokedBalls < totalBallsGenerated) {
			generateBall ();
			Invoke("initiateBall",ballGenerateTimeInSeconds);
		}
	}

	public void generateBall(){
		Vector3 instantiatePosition = transform.position;
		instantiatePosition.y += Random.Range(0.0f, 10.0f);
		instantiatePosition.x += Random.Range(-5.0f, 5.0f);
		GameObject ball = Instantiate (ballPrefab,instantiatePosition, transform.rotation) as GameObject;
		BallScript ballScript = ball.GetComponent<BallScript> ();
		int ballIndex = Random.Range (0, gameManager.colors.Length);
		Color ballColor = gameManager.colors[ballIndex];
		ballScript.initiate (ballIndex, ballColor);
		ball.transform.parent = transform;
	}

	public List<BallScript> getBalls(){
		List<BallScript> balls = new List<BallScript> ();
		for (int i = 0; i < transform.childCount; i++) {
			BallScript ball = transform.GetChild (i).GetComponent<BallScript>();
			balls.Add (ball);
		}
		return balls;
	}
	
	// Update is called once per frame

}
