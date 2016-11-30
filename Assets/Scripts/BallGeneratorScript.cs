using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BallGeneratorScript : MonoBehaviour {
	public GameManagerScript gameManager; //Game Manager that is working right now
	public int totalBallsGenerated; //Total number of balls that is goinf to be generated
	public float ballGenerateTimeInSeconds; //Delay time when ball is generated each time
	public GameObject ballPrefab; //Prefab of balls
	public float bombProbability; //Prob of a bomb generated
	public float giantBallProbability; //Prob of a giant ball generated
	bool isRecycling = false; //Is a ball being recycled
	protected Dictionary<int, List<BallScript>> ballDictionary; //Balls shall be putted here
	protected int invokedBalls = 0; //Number of balls invoked

	public void Start(){
		ballDictionary = new Dictionary<int, List<BallScript>>  ();
	}

	//Function to restart game
	public virtual void Restart(){
		if (transform.childCount == 0) {
			IEnumerator initBalls = InitiateBall ();
			StartCoroutine (initBalls);
		} else {
			IEnumerator enumerator = GetAllBalls ();
			while (enumerator.MoveNext ()) {
				isRecycling = true;
				Transform child = enumerator.Current as Transform;
				BallScript ball = child.GetComponent<BallScript> ();
				RecycleBall (ball);
				invokedBalls++;
			}
			StartCoroutine(ResetIsRecycling (1.0f));
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
	protected virtual IEnumerator InitiateBall(){
		while (invokedBalls < totalBallsGenerated)
		{
			GenerateBall();
			yield return new WaitForSeconds(ballGenerateTimeInSeconds);
		}
	}

	//Recycle a ball withour a delay
	private IEnumerator RecycleBallWithDelay(BallScript ball){
		RecycleBall (ball);
		yield return new WaitForSeconds(0.05f);
		invokedBalls++;
	}

	private IEnumerator ResetIsRecycling(float delay){
		yield return new WaitForSeconds(delay);
		isRecycling = false;
	}

	//Recycle a ball without delay
	public virtual void RecycleBall(BallScript ball){
		if (ball != null) {
			isRecycling = true;
			ball.EnableRigidbody ();
			Vector3 instantiatePosition = transform.position;
			Rigidbody2D ballRigidbody = ball.GetComponent<Rigidbody2D> ();
			ballRigidbody.velocity = new Vector2 (0, 0.5f);
			ballRigidbody.angularVelocity = 0.0f;

			instantiatePosition.y += Random.Range (0.0f, 10.0f);
			instantiatePosition.x += Random.Range (-2.0f, 2.0f);
			ball.transform.position = instantiatePosition;

			int ballIndex = -1;
			float randomNumber = Random.Range (0.0f, 1.0f);

			if (randomNumber < giantBallProbability) {
				ball.transform.localScale = new Vector3 (2.0f, 2.0f, 2.0f);
			} else {
				ball.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
			}

			if (randomNumber < bombProbability) {
				ballIndex = BallScript.Constants.bombIndex;
				ball.Initiate (ballIndex, gameManager.bombSprite);
			} else {
				if (randomNumber < giantBallProbability + bombProbability) {
					ball.gameObject.transform.localScale = new Vector3 (2.0f, 2.0f, 2.0f);
				}
				ballIndex = Random.Range (0, gameManager.sprites.Length);
				if (!ballDictionary.ContainsKey (ballIndex)) {
					ballDictionary.Add (ballIndex, new List<BallScript> ());
				}
				ball.Initiate (ballIndex, gameManager.sprites[ballIndex]);
				ballDictionary [ballIndex].Add (ball);
			}
			ball.ForceIdle ();
		} else {
			Debug.Log ("Ball is null");
		}
	}

	//Instantiate a ball
	public virtual void GenerateBall(){
		Vector3 instantiatePosition = transform.position;
		instantiatePosition.y += Random.Range(0.0f, 10.0f);
		instantiatePosition.x += Random.Range(-2.0f, 2.0f);
		GameObject ballObject = Instantiate (ballPrefab, instantiatePosition, transform.rotation);
		ballObject.name = "Ball" + invokedBalls.ToString ();
		BallScript ball = ballObject.GetComponent<BallScript> ();
		ball.Id = invokedBalls;

		int ballIndex = -1;
		float randomNumber = Random.Range (0.0f, 1.0f);

		if (randomNumber < giantBallProbability) {
			ballObject.transform.localScale = new Vector3 (2.0f, 2.0f, 2.0f);
		} else {
			ballObject.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
		}

		if (randomNumber < bombProbability) {
			ballIndex = BallScript.Constants.bombIndex;
			ball.Initiate (ballIndex, gameManager.bombSprite);
		} else {
			ballIndex = Random.Range (0, gameManager.sprites.Length);
			if (!ballDictionary.ContainsKey (ballIndex)) {
				ballDictionary.Add (ballIndex, new List<BallScript> ());
			}
			ball.Initiate (ballIndex, gameManager.sprites[ballIndex]);
			ballDictionary [ballIndex].Add (ball);
		}
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
		} else {
			return null;
		}
	}

	public void DisableBallRigidbodies(){
//		if (isRecycling == false) {
//			IEnumerator enumerator = GetAllBalls ();
//			while (enumerator.MoveNext ()) {
//				Transform ballTransform = enumerator.Current as Transform;
//				BallScript ball = ballTransform.GetComponent<BallScript> ();
//				ball.DisableRigidbody ();
//			}
//		}
	}

	public void EnableBallRigidbodies(){
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

	public bool IsRecycling{
		get{
			return isRecycling;
		}
		set{
			isRecycling = value;
		}
	}
}
