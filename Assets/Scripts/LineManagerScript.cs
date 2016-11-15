using UnityEngine;
using System.Collections.Generic;

public class LineManagerScript : MonoBehaviour {
	public GameManagerScript gameManager;
	LineRenderer lineRenderer;
	List<BallScript> ballScriptList;
	int chosenIndex = -1;

	void Start(){
		lineRenderer = GetComponent<LineRenderer> ();
		ballScriptList = new List<BallScript> ();
	}

	void Update() {
		if (Input.GetMouseButtonDown(0)) {
			if (gameManager.isAllBallNotMoving ()) {
				BallScript ball = getBallTouched ();
				chosenIndex = ball.getIndex ();
				if (ball != null && canThisBallStillChain (ball)) {
					addBall (ball);
				} 
				else {
					gameManager.refreshAllBalls ();
				}
			} 
			else {
				restart ();
			}
		}
		else if (Input.GetMouseButton (0)) {
			if (gameManager.isAllBallNotMoving ()) {
				BallScript ball = getBallTouched ();
				if (ball != null
					&& ball.getIndex() == chosenIndex
					&& !ballScriptList.Contains(ball)
					&& isThisBallValid(ballScriptList[ballScriptList.Count - 1], ball)) {
					addBall (ball);

					if (!canThisBallStillChain (ball)) {
						retrieveBalls ();
					}
				}
			} 
			else {
				restart ();
			}
		}
		else if (Input.GetMouseButtonUp (0)) {
			restart ();
		}
		//Debug.Log ("BallTouchedCount: " + ballScriptList.Count);
	}

	public void retrieveBalls(){
		foreach(BallScript destroyedBall in ballScriptList){
			destroyedBall.retrieved();
			gameManager.ballGenerator.generateBall ();
		}
		restart ();
	}

	public void restart(){
		gameManager.refreshAllBalls ();
		chosenIndex = -1;
		ballScriptList.Clear ();
		refreshLines ();
	}

	public void addBall(BallScript ball){
		ballScriptList.Add (ball);
		refreshLines ();
	}

	public void refreshLines(){
		Vector3[] ballPositions = new Vector3[ballScriptList.Count];
		for(int i = 0; i < ballScriptList.Count; i++){
			ballPositions [i] = ballScriptList [i].transform.position;
			ballPositions [i].z = 0;
		}
		lineRenderer.SetVertexCount(ballPositions.Length);
		lineRenderer.SetPositions (ballPositions);
	}

	BallScript getBallTouched(){
		BallScript ballScript = null;
		Vector2 mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		Collider2D hitCollider = Physics2D.OverlapPoint (mousePosition);
		if (hitCollider != null) {
			GameObject ball = hitCollider.gameObject;
			ballScript = ball.GetComponent<BallScript> ();
		}
		return ballScript;
	}

	bool isThisBallValid(BallScript originBall, BallScript targetBall){
		bool result = false;
		float distance = Vector2.Distance (originBall.transform.position, targetBall.transform.position);
		//Debug.DrawRay (lastBall.transform.position, ball.transform.position - lastBall.transform.position, white, distance);
		RaycastHit2D[] hits = Physics2D.RaycastAll(originBall.transform.position, targetBall.transform.position - originBall.transform.position, distance);
		if (hits.Length == 2) {
			RaycastHit2D hit = hits [1];
			if (hit.collider != null) {
				BallScript hittedBall = hit.collider.gameObject.GetComponent<BallScript> ();
				if (hittedBall != null 
					&& hittedBall.gameObject.GetInstanceID () == targetBall.gameObject.GetInstanceID ()) {
					result = true;
				}
			}
		}
		return result;
	}

	bool canThisBallStillChain(BallScript ball){
		bool result = false;
		gameManager.refreshAllBalls ();
		List<BallScript> closestBalls = new List<BallScript> ();
		Color white = new Color (1.0f,1.0f,1.0f, 1.0f);
		for (float x = -2.0f; x <= 2.0f; x += 0.1f) {
			for (float y = -2.0f; y <= 2.0f; y += 0.1f) {
				Vector2 direction = new Vector2 (x, y);
				//Debug.DrawRay (ball.transform.position, direction, white, Mathf.Infinity);
				
				RaycastHit2D[] hits = Physics2D.RaycastAll(ball.transform.position, direction, Mathf.Infinity);
				if (hits.Length > 1) {
					RaycastHit2D hit = hits [1];
					if (hit.collider != null) {
						BallScript hittedBall = hit.collider.gameObject.GetComponent<BallScript> ();
						if (hittedBall != null
						   && hittedBall.getIndex () == chosenIndex
						   && !ballScriptList.Contains (hittedBall)) {
							closestBalls.Add (hittedBall);
						}
					}
				}
			}
		}

		foreach(BallScript closestBall in closestBalls){
			if (isThisBallValid (ball, closestBall)) {
				result = true;
				closestBall.gameObject.GetComponent<SpriteRenderer> ().color = white;
			}
		}
		return result;
	}


}
