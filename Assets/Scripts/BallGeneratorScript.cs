using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BallGeneratorScript : MonoBehaviour {
	public GameManagerScript gameManager; //Game Manager that is working right now
	public int totalBallsGenerated; //Total number of balls that is goinf to be generated
	public float ballGenerateTimeInSeconds; //Delay time when ball is generated each time
	public Transform ballPrefab; //Prefab of balls
	public bool isRecycling = false;
	Dictionary<int, List<BallScript>> ballDictionary; //Balls shall be putted here
	int invokedBalls = 0; //Number of balls invoked

	public void Start(){
		ballDictionary = new Dictionary<int, List<BallScript>>  ();
	}

	//Function to restart game
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

	//Retrieve all balls, used when game over
	public void RetrieveAllBalls(){
		IEnumerator enumerator = GetAllBalls ();
		while (enumerator.MoveNext ()) {
			Transform child = enumerator.Current as Transform;
			BallScript ball = child.GetComponent<BallScript> ();
			ball.Retrieved ();
		}
		invokedBalls = 0;
	}

	//Generate balls based on the number of balls that has been decided beforehand
	private IEnumerator InitiateBall(){
		while (invokedBalls < totalBallsGenerated)
		{
			GenerateBall();
			yield return new WaitForSeconds(ballGenerateTimeInSeconds);
		}
	}

	//Recycle a ball withour a delay
	private IEnumerator RecycleBallWithDelay(BallScript ball){
		RecycleBall (ball);
		yield return new WaitForSeconds(ballGenerateTimeInSeconds);
	}

	//Recycle a ball without delay
	public void RecycleBall(BallScript ball){
		if (ball != null) {
			ball.EnableRigidbody ();
			Vector3 instantiatePosition = transform.position;
			Rigidbody2D ballRigidbody = ball.GetComponent<Rigidbody2D> ();
			ballRigidbody.velocity = new Vector2 (0, 0.5f);
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
			isRecycling = false;
		} else {
			Debug.Log ("Ball is null");
		}
	}

	//Instantiate a ball
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

	//Get all balls generated
	public IEnumerator GetAllBalls(){
		return transform.GetEnumerator ();
	}

	//Get all balls based on color index
	public List<BallScript> GetBallsBasedOnIndex(int index){
		if (ballDictionary.ContainsKey (index)) {
			return ballDictionary [index];
		} else
			return null;
	}

	public void DisableBallRigibodies(){
		//if (isRecycling == false) {
//			IEnumerator enumerator = GetAllBalls ();
//			while (enumerator.MoveNext ()) {
//				Transform ballTransform = enumerator.Current as Transform;
//				BallScript ball = ballTransform.GetComponent<BallScript> ();
//				ball.DisableRigidbody ();
//			}
		//}
	}

	public void EnableBallRigibodies(){
//		isRecycling = true;
//		IEnumerator enumerator = GetAllBalls ();
//		while (enumerator.MoveNext ()) {
//			Transform ballTransform = enumerator.Current as Transform;
//			BallScript ball = ballTransform.GetComponent<BallScript> ();
//			ball.EnableRigidbody ();
//		}
	}

	public int InvokedBalls{
		get{
			return invokedBalls;
		}
		set{
			invokedBalls = value;
		}
	}
}
