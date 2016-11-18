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
		IEnumerator initBalls = InitiateBall ();
		StartCoroutine (initBalls);
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

	public void RecycleBall(BallScript ball){
		if (ball != null ){
			Vector3 instantiatePosition = transform.position;
			instantiatePosition.y += Random.Range(0.0f, 10.0f);
			instantiatePosition.x += Random.Range(-4.0f, 4.0f);
			ball.transform.position = instantiatePosition;
			int ballIndex = Random.Range (0, gameManager.sprites.Length);
			if (!ballDictionary.ContainsKey(ballIndex)) {
				ballDictionary.Add (ballIndex, new List<BallScript> ());
			}
			ball.Initiate (ballIndex, gameManager.sprites[ballIndex]);
			ball.ForceIdle ();
			ball.Refresh ();
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
		if (!ballDictionary.ContainsKey(ballIndex)) {
			ballDictionary.Add (ballIndex, new List<BallScript> ());
		}
		ballScript.Initiate (ballIndex, gameManager.sprites[ballIndex]);
		ballDictionary [ballIndex].Add (ballScript);
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
