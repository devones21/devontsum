using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinScript : MonoBehaviour {
	public float time = 2.0f;
	private Vector3 startPos;
	private Vector3 endPos;
	private float startTime;
	public AnimationCurve curve;
	int score = 0;
	GameManagerScript gameManager;

	public int Score{
		get{
			return score;
		}
		set{
			score = value;
		}
	}

	// Use this for initialization
	void Start () {
		gameManager = GameObject.Find ("GameManager").GetComponent<GameManagerScript> ();
		startTime = Time.time;
		startPos = transform.position;
		endPos = GameObject.Find ("ScoreText").transform.position;
	}

	void Update() {
		if (startPos != null && endPos != null) {
			StartCoroutine(Move(time));
		}
	}

	//Move the coin to score text
	//Source: http://answers.unity3d.com/questions/697665/transition-between-vector3s-using-animationcurve.html
	IEnumerator Move(float time) {
		float timer = 0.0f;
		while (timer <= time) {
			if (gameManager.IsPlaying) {
				Debug.Log (Vector3.Lerp (startPos, endPos, curve.Evaluate (timer / time)));
				transform.position = Vector3.Lerp (startPos, endPos, curve.Evaluate (timer / time));
				timer += Time.deltaTime;
				yield return null;
			} else {
				break;
			}
		}
		if (gameManager.IsPlaying) gameManager.AddScore (score);
		Destroy (gameObject);
	}
}
