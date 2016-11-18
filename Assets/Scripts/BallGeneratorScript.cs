using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BallGeneratorScript : MonoBehaviour {
	public GameManagerScript gameManager;
	public int totalBallsGenerated;
	public float ballGenerateTimeInSeconds;
	public Transform ballPrefab;
	int invokedBalls = 0;
	Color[] colors;

	public void Restart(){
		IEnumerator initBalls = initiateBall ();
		StartCoroutine (initBalls);
	}

	public void RetrieveAllBalls(){
		IEnumerator enumerator = GetBalls ();
		while (enumerator.MoveNext ()) {
			Transform child = enumerator.Current as Transform;
			BallScript ball = child.GetComponent<BallScript> ();
			ball.Retrieved ();
		}
		invokedBalls = 0;
	}

	private IEnumerator initiateBall(){
		while (invokedBalls < totalBallsGenerated)
		{
			GenerateBall();
			yield return new WaitForSeconds(ballGenerateTimeInSeconds);
		}
	}

	public void GenerateBall(){
		Vector3 instantiatePosition = transform.position;
		instantiatePosition.y += Random.Range(0.0f, 10.0f);
		instantiatePosition.x += Random.Range(-4.0f, 4.0f);
		GameObject ball = Instantiate (ballPrefab,instantiatePosition, transform.rotation) as GameObject;
		BallScript ballScript = ball.GetComponent<BallScript> ();
		ballScript.Id = invokedBalls;
		int ballIndex = Random.Range (0, gameManager.sprites.Length);
		ballScript.Initiate (ballIndex, gameManager.sprites[ballIndex]);
		ball.transform.parent = transform;
		invokedBalls++;
	}

	public IEnumerator GetBalls(){
		return transform.GetEnumerator ();
	}
}
