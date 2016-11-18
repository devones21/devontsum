using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class LineManagerScript : MonoBehaviour {
	public GameManagerScript gameManager;
	public LayerMask ballLayer;
	LineRenderer lineRenderer;
	List<BallScript> chainedBalls;
	int chosenIndex = -1;

	void Start(){
		lineRenderer = GetComponent<LineRenderer> ();
		chainedBalls = new List<BallScript> ();
	}

	void Update() {
		if (Input.GetMouseButtonDown(0)) {
			if (gameManager.IsAllBallNotMoving ()) {
				BallScript ball = GetBallTouched ();
				if (ball != null){
					chosenIndex = ball.Index;
					AddBall (ball);
					if (GetChainableBalls (ball, chainedBalls, true).Count > 0) {
//						List<BallScript> bestPathBalls = recursiveFunctionToGetBestPath (ball, null, null);
//						Debug.Log (bestPathBalls.Count);
//						drawLines (bestPathBalls);
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
				Restart ();
			}
		}
		else if (Input.GetMouseButton (0)) {
			if (gameManager.IsAllBallNotMoving () && chosenIndex != -1) {
				BallScript ball = GetBallTouched ();
				if (ball != null
					&& ball.Index == chosenIndex
					&& !chainedBalls.Contains(ball)
					&& IsThisBallChainable(chainedBalls, chainedBalls[chainedBalls.Count - 1], ball)) {

					if (chainedBalls.Count > 0) {
						BallScript lastBall = chainedBalls [chainedBalls.Count - 1];
						List<BallScript> chainableBalls = GetChainableBalls (lastBall, chainedBalls, false);
						IEnumerator enumerator = chainableBalls.GetEnumerator ();
						while (enumerator.MoveNext ()) {
							BallScript chainableBall = enumerator.Current as BallScript;
							if (chainableBall.GetInstanceID () != ball.GetInstanceID ()) {
								chainableBall.Idle ();
							}
						}
					}

					AddBall (ball);

					if (GetChainableBalls (ball, chainedBalls, true).Count == 0) {
						//retrieveBalls ();
					}
				}
			} 
			else {
				Restart ();
			}
		}
		else if (Input.GetMouseButtonUp (0)) {
			if (gameManager.IsAllBallNotMoving () && chosenIndex != -1) {
				if (chainedBalls.Count >= 3) {
					RetrieveBalls ();
				} else {
					Restart ();
				}
			}
		}
	}

	public void RetrieveBalls(){
		int score = 100;
		int lastScore = score;
		IEnumerator enumerator = chainedBalls.GetEnumerator ();
		while(enumerator.MoveNext()){
			BallScript ballScript = enumerator.Current as BallScript;
			if (ballScript != null) {
				ballScript.RetrievedAddScore (score);
				gameManager.AddScore (score);
				gameManager.ballGenerator.GenerateBall ();
				score += lastScore;
				lastScore = score - lastScore;
			}
		}
		Restart ();
	}

	public void Restart(){
		gameManager.RefreshAllBalls ();
		chosenIndex = -1;
		if (chainedBalls != null) {
			chainedBalls.Clear ();
			DrawLines (chainedBalls);
		}
	}

	public void AddBall(BallScript ball){
		chainedBalls.Add (ball);
		ball.Selected ();
		DrawLines (chainedBalls);
	}

	public void DrawLines(List<BallScript> list){
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

	//Source: http://gamedev.stackexchange.com/questions/93823/how-to-make-line-renderer-lines-stay-flat
	Vector3[] Generate_Points(Vector3[] keyPoints, int segments=100){
		Vector3[] Points = new Vector3[(keyPoints.Length - 1) * segments + keyPoints.Length];
		for(int i = 1; i < keyPoints.Length;i++){
			Points [(i - 1) * segments + i - 1] = new Vector3(keyPoints [i-1].x,keyPoints [i-1].y,0);
			for (int j = 1;j<=segments;j++){
				float x = keyPoints [i - 1].x;
				float y = keyPoints [i - 1].y;
				float z = 0;
				float dx = (keyPoints [i].x - keyPoints [i - 1].x)/segments;
				float dy = (keyPoints [i].y - keyPoints [i - 1].y)/segments;
				Points [(i - 1) * segments + j + i - 1] = new Vector3 (x+dx*j,y+dy*j,z);
			}
		}
		Points [(keyPoints.Length - 1) * segments + keyPoints.Length - 1] = new Vector3(keyPoints [keyPoints.Length-1].x,keyPoints [keyPoints.Length-1].y,0);
		return Points;
	}

	BallScript GetBallTouched(){
		BallScript ballScript = null;
		Vector2 mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		Collider2D hitCollider = Physics2D.OverlapPoint (mousePosition);
		if (hitCollider != null) {
			GameObject ball = hitCollider.gameObject;
			ballScript = ball.GetComponent<BallScript> ();
		}
		return ballScript;
	}

	List<BallScript> GetChainableBalls(BallScript ball, List<BallScript> list, bool isSelectable){
		List<BallScript> result = new List<BallScript> ();
		IEnumerator enumerator = gameManager.ballGenerator.GetBalls ();
		while (enumerator.MoveNext ()) {
			Transform checkedBallTransform = enumerator.Current as Transform;
			BallScript checkedBall = checkedBallTransform.GetComponent<BallScript> ();
			if (!list.Contains(checkedBall) 
				&& ball.Index == checkedBall.Index
				&& IsThisBallChainable (list, ball, checkedBall)) {
				result.Add (checkedBall);
				if(isSelectable)checkedBall.Selectable ();
			}
		}
		return result;
	}

	bool IsThisBallChainable(List<BallScript> list, BallScript originBall, BallScript targetBall){
		if (list.Contains (targetBall))
			return false;
		float distance = Vector2.Distance (originBall.transform.position, targetBall.transform.position);
		Quaternion angle = Quaternion.FromToRotation(originBall.transform.position, targetBall.transform.position);
		Vector3 raycastDistance = angle * new Vector2 (0, gameManager.raycastWidth / 2);
		Vector2 raycastDistance2D = new Vector2 (raycastDistance.x, raycastDistance.y);
		Vector2 topPosition = new Vector2(originBall.transform.position.x, originBall.transform.position.y) + raycastDistance2D;
		Vector2 bottomPosition = new Vector2(originBall.transform.position.x, originBall.transform.position.y) - raycastDistance2D;
		Vector2 topTargetPosition = new Vector2(targetBall.transform.position.x, targetBall.transform.position.y) + raycastDistance2D;
		Vector2 bottomTargetPosition = new Vector2(targetBall.transform.position.x, targetBall.transform.position.y) - raycastDistance2D;

		RaycastHit2D[] topHits = Physics2D.RaycastAll(topPosition, topTargetPosition - topPosition, distance, ballLayer);
		RaycastHit2D[] bottomHits = Physics2D.RaycastAll(bottomPosition, bottomTargetPosition - bottomPosition, distance, ballLayer);
		if (topHits.Length == 2 && bottomHits.Length == 2) {
			RaycastHit2D hit = topHits [1];
			if (hit.collider != null) {
				BallScript hittedBall = hit.collider.gameObject.GetComponent<BallScript> ();
				//Debug.Log (targetBall.Id + "(Target Id) - " + hittedBall.Id + "(Hitted Id)");
				if (hittedBall != null 
					&& hittedBall.Id == targetBall.Id
					&& originBall.Index == targetBall.Index) {
					Debug.DrawRay (originBall.transform.position, targetBall.transform.position - originBall.transform.position, Color.white, distance);
					return true;
				}
			}
		}
//		RaycastHit2D topHits = Physics2D.Linecast(topPosition, topTargetPosition, ballLayer);
//		RaycastHit2D bottomHits = Physics2D.Linecast(bottomPosition, bottomTargetPosition, ballLayer);
//		if (topHits.collider.gameObject == targetBall.gameObject  && topHits.collider != null && bottomHits.collider != null && topHits.collider == bottomHits.collider) {
//			BallScript hittedBall = topHits.collider.gameObject.GetComponent<BallScript> ();
//			Debug.Log (targetBall.Id + "(Target Id) - " + hittedBall.Id + "(Hitted Id)");
//			if (hittedBall != null 
//				&& originBall.Index == targetBall.Index) {
//				Debug.DrawRay (originBall.transform.position, targetBall.transform.position - originBall.transform.position, Color.white, distance);
//				return true;
//			}
//		}
		return false;
	}

	List<BallScript> RecursiveFunctionToGetBestPath(BallScript ball, List<BallScript> result, List<BallScript> bestResult){
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
		List<BallScript> chainableBalls = GetChainableBalls (ball, result, false);
		if (chainableBalls.Count > 0) {
			IEnumerator enumerator = chainableBalls.GetEnumerator();
			while (enumerator.MoveNext ()) {
				BallScript chainableBall = enumerator.Current as BallScript;
				RecursiveFunctionToGetBestPath (chainableBall, result, bestResult);
			}
		}
		return bestResult;
	}

//	List<BallScript> getBestPath(BallScript ball){
//		List<BallScript> result = new List<BallScript> ();
//		List<BallScript> chainableBalls = GetChainableBalls (ball, result, false);
//		if (chainedBalls.Count > 0) {
//			BallScript currentBall = ball;
//			result.Add (currentBall);
//			foreach (BallScript chainableBall in chainableBalls) {
//			}
//		}
//	}

	public void ShowHint(){
		if (gameManager.IsAllBallNotMoving ()) {
			List<BallScript> bestHint = new List<BallScript> ();

			IEnumerator enumerator = gameManager.ballGenerator.GetBalls ();
			while (enumerator.MoveNext ()) {
				Transform ballTransform = enumerator.Current as Transform;
				BallScript ball = ballTransform.GetComponent<BallScript> ();

				List<BallScript> hint = RecursiveFunctionToGetBestPath (ball, null, null);
				if (hint.Count > bestHint.Count) {
					bestHint = hint;
				}
			}

			//DrawLines (bestHint);

			IEnumerator enumeratorBestHint = bestHint.GetEnumerator();
			while (enumeratorBestHint.MoveNext ()) {
				BallScript ball = enumeratorBestHint.Current as BallScript;
				ball.Hint ();
			}
		}
	}
}