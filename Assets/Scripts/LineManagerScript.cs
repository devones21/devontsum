﻿using UnityEngine;
using System.Collections.Generic;

public class LineManagerScript : MonoBehaviour {
	public GameManagerScript gameManager;
	LineRenderer lineRenderer;
	List<BallScript> chainedBalls;
	int chosenIndex = -1;

	void Start(){
		lineRenderer = GetComponent<LineRenderer> ();
		chainedBalls = new List<BallScript> ();
	}

	void Update() {
		if (Input.GetMouseButtonDown(0)) {
			if (gameManager.isAllBallNotMoving ()) {
				BallScript ball = getBallTouched ();
				if (ball != null){
					chosenIndex = ball.getIndex ();
					if (getChainableBalls (ball, chainedBalls, true).Count > 0) {
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
					//gameManager.refreshAllBalls ();
				}
			} 
			else {
				//restart ();
			}
		}
		else if (Input.GetMouseButton (0)) {
			if (gameManager.isAllBallNotMoving () && chosenIndex != -1) {
				BallScript ball = getBallTouched ();
				if (ball != null
					&& ball.getIndex() == chosenIndex
					&& !chainedBalls.Contains(ball)
					&& isThisBallValid(chainedBalls[chainedBalls.Count - 1], ball)) {

					if (chainedBalls.Count > 0) {
						BallScript lastBall = chainedBalls [chainedBalls.Count - 1];
						foreach (BallScript chainableBall in getChainableBalls(lastBall, chainedBalls, false)) {
							if (chainableBall.GetInstanceID () != ball.GetInstanceID ()) {
								chainableBall.switchAnimationToIdle ();
							}
						}
					}

					addBall (ball);

					if (getChainableBalls (ball, chainedBalls, true).Count == 0) {
						//retrieveBalls ();
					}
				}
			} 
			else {
				//restart ();
			}
		}
		else if (Input.GetMouseButtonUp (0)) {
			if (gameManager.isAllBallNotMoving () && chosenIndex != -1) {
				gameManager.refreshAllBalls ();
				retrieveBalls ();
			}
		}
		//Debug.Log ("BallTouchedCount: " + ballScriptList.Count);
	}

	public void retrieveBalls(){
		float retrievedLength = 0;
		int score = 100;
		foreach(BallScript retrievedBalls in chainedBalls){
			retrievedLength = retrievedBalls.retrieved(score);
			gameManager.addScore (score);
			gameManager.ballGenerator.generateBall ();
			score *= 2;
		}
		Invoke ("restart", retrievedLength);
	}

	public void restart(){
		gameManager.refreshAllBalls ();
		chosenIndex = -1;
		chainedBalls.Clear ();
		drawLines (chainedBalls);
	}

	public void addBall(BallScript ball){
		chainedBalls.Add (ball);
		ball.switchAnimationToSelected ();
		drawLines (chainedBalls);
	}

	public void drawLines(List<BallScript> list){
		if (list.Count > 1) {
			Vector3[] ballPositions = new Vector3[list.Count];
			for (int i = 0; i < list.Count; i++) {
				ballPositions [i] = list [i].transform.position;
				ballPositions [i].z = 0;
			}
			ballPositions = Generate_Points (ballPositions, 100);
			lineRenderer.SetVertexCount (ballPositions.Length);
			lineRenderer.SetPositions (ballPositions);
		} else {
			lineRenderer.SetVertexCount (0);
		}
	}

	Vector3[] Generate_Points(Vector3[] keyPoints, int segments=100){
		Vector3[] Points = new Vector3[(keyPoints.Length - 1) * segments + keyPoints.Length];
		for(int i = 1; i < keyPoints.Length;i++){
			Points [(i - 1) * segments + i - 1] = new Vector3(keyPoints [i-1].x,keyPoints [i-1].y,0);
			for (int j = 1;j<=segments;j++){
				float x = keyPoints [i - 1].x;
				float y = keyPoints [i - 1].y;
				float z = 0;//keyPoints [i - 1].z;
				float dx = (keyPoints [i].x - keyPoints [i - 1].x)/segments;
				float dy = (keyPoints [i].y - keyPoints [i - 1].y)/segments;
				Points [(i - 1) * segments + j + i - 1] = new Vector3 (x+dx*j,y+dy*j,z);
			}
		}
		Points [(keyPoints.Length - 1) * segments + keyPoints.Length - 1] = new Vector3(keyPoints [keyPoints.Length-1].x,keyPoints [keyPoints.Length-1].y,0);
		return Points;
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

	List<BallScript> getChainableBalls(BallScript ball, List<BallScript> list, bool isSelectable){
		List<BallScript> result = new List<BallScript> ();
		foreach(BallScript checkedBall in gameManager.ballGenerator.getBalls()){
			if (!list.Contains(checkedBall) && checkedBall.getIndex() == ball.getIndex() && isThisBallValid (ball, checkedBall)) {
				result.Add (checkedBall);
				if(isSelectable)checkedBall.switchAnimationToSelectable ();
			}
		}
		return result;
	}

	bool isThisBallValid(BallScript originBall, BallScript targetBall){
		if (chainedBalls.Contains (targetBall))
			return false;
		bool result = false;
		float distance = Vector2.Distance (originBall.transform.position, targetBall.transform.position);
		//Debug.DrawRay (originBall.transform.position, targetBall.transform.position - originBall.transform.position, Color.black, distance);
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

	List<BallScript> recursiveFunctionToGetBestPath(BallScript ball, List<BallScript> result, List<BallScript> bestResult){
		if (result == null) {
			result = new List<BallScript> ();
		}
		if (bestResult == null) {
			bestResult = new List<BallScript> ();
		}
		result.Add (ball);
		if (result.Count > bestResult.Count) {
			bestResult = result;
		}
		List<BallScript> chainableBalls = getChainableBalls (ball, result, false);
		if (chainableBalls.Count > 0) {
			foreach (BallScript chainableBall in chainableBalls) {
				recursiveFunctionToGetBestPath (chainableBall, result, bestResult);
			}
		}
		return bestResult;
	}

	public void showHint(){
		if (gameManager.isAllBallNotMoving ()) {
			List<BallScript> bestHint = new List<BallScript> ();
			;
			foreach (BallScript ball in gameManager.ballGenerator.getBalls()) {
				List<BallScript> hint = recursiveFunctionToGetBestPath (ball, null, null);
				if (hint.Count > bestHint.Count) {
					bestHint = hint;
				}
			}

			//drawLines (bestHint);
			foreach (BallScript ball in bestHint) {
				ball.hint ();
			}
		}
	}
}
