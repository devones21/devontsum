using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BallGeneratorScript : MonoBehaviour {
	public GameManagerScript gameManager;
	public int totalBallsGenerated;
	public float ballGenerateTimeInSeconds;
	public Transform ballPrefab;
	Dictionary<int, List<BallScript>> ballDictionary;
	int invokedBalls = 0;
	Color[] colors;

	public void Start(){
		ballDictionary = new Dictionary<int, List<BallScript>>  ();
	}

	public void Restart(){
		if (transform.childCount == 0) {
			IEnumerator initBalls = InitiateBall ();
			StartCoroutine (initBalls);
		} else {
			IEnumerator enumerator = GetAllBalls ();
			while (enumerator.MoveNext ()) {
				Transform child = enumerator.Current as Transform;
				BallScript ball = child.GetComponent<BallScript> ();
				StartCoroutine(RecycleBallWithDelay (ball));
			}
		}
	}

	public void RetrieveAllBalls(){
		IEnumerator enumerator = GetAllBalls ();
		while (enumerator.MoveNext ()) {
			Transform child = enumerator.Current as Transform;
			BallScript ball = child.GetComponent<BallScript> ();
			ball.Retrieved ();
		}
		invokedBalls = 0;
	}

	private IEnumerator InitiateBall(){
		while (invokedBalls < totalBallsGenerated)
		{
			GenerateBall();
			yield return new WaitForSeconds(ballGenerateTimeInSeconds);
		}
	}

	private IEnumerator RecycleBallWithDelay(BallScript ball){
		RecycleBall (ball);
		yield return new WaitForSeconds(ballGenerateTimeInSeconds);
	}

	public void RecycleBall(BallScript ball){
		if (ball != null) {
			Vector3 instantiatePosition = transform.position;
			Rigidbody2D ballRigidbody = ball.GetComponent<Rigidbody2D> ();
			ballRigidbody.velocity = new Vector2 (0, 0);
			ballRigidbody.angularVelocity = 0.0f;

			instantiatePosition.y += Random.Range (0.0f, 10.0f);
			instantiatePosition.x += Random.Range (-4.0f, 4.0f);
			ball.transform.position = instantiatePosition;
			int ballIndex = Random.Range (0, gameManager.sprites.Length);
			if (!ballDictionary.ContainsKey (ballIndex)) {
				ballDictionary.Add (ballIndex, new List<BallScript> ());
			}
			ball.Initiate (ballIndex, gameManager.sprites [ballIndex]);
			ballDictionary [ballIndex].Add (ball);
			ball.ForceIdle ();
			ball.Refresh ();
		} else {
			Debug.Log ("Ball is null");
		}
	}

	public void GenerateBall(){
		Vector3 instantiatePosition = transform.position;
		instantiatePosition.y += Random.Range(0.0f, 10.0f);
		instantiatePosition.x += Random.Range(-4.0f, 4.0f);
		GameObject ballObject = Instantiate (ballPrefab,instantiatePosition, transform.rotation) as GameObject;
		BallScript ball = ballObject.GetComponent<BallScript> ();
		ball.Id = invokedBalls;
		int ballIndex = Random.Range (0, gameManager.sprites.Length);
		if (!ballDictionary.ContainsKey(ballIndex)) {
			ballDictionary.Add (ballIndex, new List<BallScript> ());
		}
		ball.Initiate (ballIndex, gameManager.sprites[ballIndex]);
		ballDictionary [ballIndex].Add (ball);
		ball.transform.parent = transform;
		invokedBalls++;
	}


	public IEnumerator GetAllBalls(){
		return transform.GetEnumerator ();
	}

	public List<BallScript> GetBallsBasedOnIndex(int index){
		if (ballDictionary.ContainsKey (index)) {
			return ballDictionary [index];
		} else
			return null;
	}
}
