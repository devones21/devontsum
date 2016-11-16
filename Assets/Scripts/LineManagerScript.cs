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
				if (ball != null){
					chosenIndex = ball.getIndex ();
					if (getChainableBalls (ball, ballScriptList).Count > 0) {
						addBall (ball);
//						List<BallScript> bestPathBalls = recursiveFunctionToGetBestPath (ball, null, null);
//						Debug.Log (bestPathBalls.Count);
//						foreach(BallScript selectableBall in bestPathBalls){
//
//							selectableBall.switchAnimationToSelectable ();
//						}
					} 
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
			if (gameManager.isAllBallNotMoving () && chosenIndex != -1) {
				BallScript ball = getBallTouched ();
				if (ball != null
					&& ball.getIndex() == chosenIndex
					&& !ballScriptList.Contains(ball)
					&& isThisBallValid(ballScriptList[ballScriptList.Count - 1], ball)) {
					addBall (ball);

					if (getChainableBalls (ball, ballScriptList).Count == 0) {
						Debug.Log ("No more to be chained");
						retrieveBalls ();
					} else {
						Debug.Log ("More to be chained");
					}
				}
			} 
			else {
				restart ();
			}
		}
		else if (Input.GetMouseButtonUp (0)) {
			retrieveBalls ();
		}
		//Debug.Log ("BallTouchedCount: " + ballScriptList.Count);
	}

	public void retrieveBalls(){
		foreach(BallScript retrievedBalls in ballScriptList){
			retrievedBalls.retrieved();
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
		ball.switchAnimationToSelected ();
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
			//Debug.Log ("Hit Something = " + hitCollider.gameObject.name);
			GameObject ball = hitCollider.gameObject;
			ballScript = ball.GetComponent<BallScript> ();
		} else {
			//Debug.Log ("Hit Nothing");
		}

//		if (ballScript == null) {
//			Debug.Log ("Ball Script Null");
//		} else {
//			Debug.Log ("Ball Script Not Null");
//		}
		return ballScript;
	}

	bool isThisBallValid(BallScript originBall, BallScript targetBall){
		if (ballScriptList.Contains (targetBall))
			return false;
		bool result = false;
		float distance = Vector2.Distance (originBall.transform.position, targetBall.transform.position);
		//Debug.DrawRay (lastBall.transform.position, ball.transform.position - lastBall.transform.position, white, distance);
		RaycastHit2D[] hits = Physics2D.RaycastAll(originBall.transform.position, targetBall.transform.position - originBall.transform.position, distance);
		if (hits.Length == 2) {
			RaycastHit2D hit = hits [1];
			if (hit.collider != null) {
				BallScript hittedBall = hit.collider.gameObject.GetComponent<BallScript> ();
				if (hittedBall != null 
					&& hittedBall.gameObject.GetInstanceID () == targetBall.gameObject.GetInstanceID ()
					&& originBall.getIndex() == targetBall.getIndex()) {
					result = true;
				}
			}
		}
		return result;
	}

	//TEMPORARILY NOT USED
//	List<BallScript> recursiveFunctionToGetBestPath(BallScript ball, List<BallScript> result, List<BallScript> bestResult){
//		if (result == null) {
//			result = new List<BallScript> ();
//		}
//		if (bestResult == null) {
//			bestResult = new List<BallScript> ();
//		}
//		result.Add (ball);
//		if (result.Count > bestResult.Count) {
//			bestResult = result;
//		}
//		List<BallScript> chainableBalls = getChainableBalls (ball, result);
//		if (chainableBalls.Count > 0) {
//			foreach (BallScript chainableBall in chainableBalls) {
//				recursiveFunctionToGetBestPath (chainableBall, result, bestResult);
//			}
//		}
//
//		return bestResult;
//	}

	List<BallScript> getChainableBalls(BallScript ball, List<BallScript> list){
		List<BallScript> result = new List<BallScript> ();
		foreach(BallScript checkedBall in gameManager.ballGenerator.getBalls()){
			if (!list.Contains(checkedBall) && checkedBall.getIndex() == chosenIndex && isThisBallValid (ball, checkedBall)) {
				result.Add (checkedBall);
				checkedBall.switchAnimationToSelectable ();
				//closestBall.transform.GetComponentsInChildren<SpriteRenderer> ()[0].sprite = gameManager.touchableBallSprite;
			}
		}
		return result;
	}

//	int numberOfChainableBalls(BallScript ball){
//		int result = 0;
//		foreach(BallScript checkedBall in gameManager.ballGenerator.getBalls()){
//			if (checkedBall.getIndex() == chosenIndex && isThisBallValid (ball, checkedBall)) {
//				result++;
//				//checkedBall.switchAnimationToSelectable ();
//				//closestBall.transform.GetComponentsInChildren<SpriteRenderer> ()[0].sprite = gameManager.touchableBallSprite;
//			}
//		}
//		return result;
//	}


}
