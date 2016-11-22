using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class LineManagerScript : MonoBehaviour {
	public GameManagerScript gameManager; //GameManager to get settings
	public LayerMask ballLayer; //Layer of the balls
	LineRenderer realLineRenderer; //Line Renderer that we will use to render real line
	LineRenderer hintLineRenderer; //Line Renderer that we will use to render hint line
	List<BallScript> chainedBalls; //Currently chained balls
	List<BallScript> hintBalls; //Possible balls that can be chained from current chained ball
	int chosenIndex = -1; //Currently chosen balls index

	void Start(){
		realLineRenderer = transform.FindChild("RealLine").GetComponent<LineRenderer> ();
		hintLineRenderer = transform.FindChild("HintLine").GetComponent<LineRenderer> ();
		chainedBalls = new List<BallScript> ();
		hintBalls = new List<BallScript> ();
	}

	void Update() {
		if (Input.GetMouseButtonDown(0)) {
			if (gameManager.IsAllBallNotMoving ()) {
				BallScript ball = GetBallTouched ();
				if (ball != null){
					chosenIndex = ball.Index;
					AddBall (ball);
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
					IEnumerator enumerator = hintBalls.GetEnumerator ();
					while (enumerator.MoveNext ()) {
						BallScript chainableBall = enumerator.Current as BallScript;
						if (chainableBall.GetInstanceID () != ball.GetInstanceID () && !chainedBalls.Contains(chainableBall)) {
							chainableBall.Idle ();
						}
					}
					AddBall (ball);
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
					gameManager.ballGenerator.EnableBallRigidbodies ();
				} else {
					Restart ();
				}
			}
		}
		if (Input.GetMouseButtonDown (1)) {
			Restart ();
		}
	}

	//Retreive currently chained balls
	public void RetrieveBalls(){
		int score = 100;
		int lastScore = score;
		IEnumerator enumerator = chainedBalls.GetEnumerator ();
		while(enumerator.MoveNext()){
			BallScript ballScript = enumerator.Current as BallScript;
			if (ballScript != null) {
				ballScript.RetrievedAddScore (score);
				gameManager.AddScore (score);
				score += lastScore;
				lastScore = score - lastScore;
			}
		}
		Restart ();
	}

	//Cancel last action
	public void Restart(){
		gameManager.RefreshAllBalls ();
		chosenIndex = -1;
		if (chainedBalls != null) {
			chainedBalls.Clear ();
			DrawRealLines (chainedBalls);
		}
	}

	//Chain a ball
	public void AddBall(BallScript ball){
		if (GetChainableBalls (ball, chainedBalls, false).Count > 0) {
			hintBalls = new List<BallScript> ();
			hintBalls = RecursiveFunctionToGetBestPath (ball, new List<BallScript>(chainedBalls));
			//DrawLines (hintBalls);
			foreach(BallScript selectableBall in hintBalls){
				if(ball != selectableBall)selectableBall.Selectable ();
			}
		}
		chainedBalls.Add (ball);
		ball.Selected ();
		DrawRealLines (chainedBalls);
	}

	//Draw lines to a group of balls
	public void DrawRealLines(List<BallScript> list){
		if (list != null && list.Count > 1) {
			Vector3[] ballPositions = new Vector3[list.Count];
			for (int i = 0; i < list.Count; i++) {
				ballPositions [i] = list [i].transform.position;
				ballPositions [i].z = 0;
			}
			ballPositions = Generate_Points (ballPositions, 100);
			realLineRenderer.SetVertexCount (ballPositions.Length);
			realLineRenderer.SetPositions (ballPositions);
		} else {
			realLineRenderer.SetVertexCount (0);
		}
	}

	//Draw lines to a group of balls
	public IEnumerator DrawHintLines(List<BallScript> list){
		if (list != null && list.Count > 1) {
			Vector3[] ballPositions = new Vector3[list.Count];
			for (int i = 0; i < list.Count; i++) {
				ballPositions [i] = list [i].transform.position;
				ballPositions [i].z = 0;
			}
			ballPositions = Generate_Points (ballPositions, 100);
			hintLineRenderer.SetVertexCount (ballPositions.Length);
			hintLineRenderer.SetPositions (ballPositions);
			StartCoroutine (DrawHintLines (null));
		} else {
			yield return new WaitForSeconds (1.0f);
			hintLineRenderer.SetVertexCount (0);
		}
	}

	//Flatten the lines so it wont look folded everytime it moves to another direction
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

	//Return the ball that is touched by player
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

	//Get balls that can be chained from a ball position
	List<BallScript> GetChainableBalls(BallScript ball, List<BallScript> list, bool isSelectable){
		List<BallScript> result = new List<BallScript> ();
		IEnumerator enumerator = gameManager.ballGenerator.GetBallsBasedOnIndex (ball.Index).GetEnumerator();
		while (enumerator.MoveNext ()) {
			BallScript checkedBall = enumerator.Current as BallScript;
			if (!list.Contains(checkedBall) 
				&& ball.Index == checkedBall.Index
				&& IsThisBallChainable (list, ball, checkedBall)) {
				result.Add (checkedBall);
				if (isSelectable) {
					checkedBall.Selectable ();
				}
			}
		}
		return result;
	}

	//Check if this ball(targetBall) can be chained to currently chained ball(originBall)
	bool IsThisBallChainable(List<BallScript> list, BallScript originBall, BallScript targetBall){
		if (list.Contains (targetBall))
			return false;
		float distance = Vector2.Distance (originBall.transform.position, targetBall.transform.position);

		//Source: http://answers.unity3d.com/questions/510361/rotate-gameobject-towards-another-gameobject-in-a.html
		float difX = targetBall.gameObject.transform.position.x - originBall.transform.position.x;
		float difY = targetBall.gameObject.transform.position.y - originBall.transform.position.y;

		float angleFloat = Mathf.Atan2(difY,difX);
		Quaternion angle = Quaternion.Euler (0,0, Mathf.Rad2Deg * angleFloat);
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
				if (hittedBall != null 
					&& hittedBall.Id == targetBall.Id
					&& originBall.Index == targetBall.Index) {
					Debug.DrawRay (topPosition, topTargetPosition - topPosition, Color.white, distance);
					Debug.DrawRay (bottomPosition, bottomTargetPosition - bottomPosition, Color.white, distance);
					return true;
				}
			}
		}
		return false;
	}

	//Look for best path for a ball to chain
	List<BallScript> RecursiveFunctionToGetBestPath(BallScript ball, List<BallScript> result){
		if (result == null) {
			result = new List<BallScript> ();
		}
		if (hintBalls == null) {
			hintBalls = new List<BallScript> ();
		}
		if (!result.Contains (ball)) {
			result.Add (ball);
			List<BallScript> chainableBalls = GetChainableBalls (ball, result, false);
			if (chainableBalls.Count > 0) {
				IEnumerator enumerator = chainableBalls.GetEnumerator ();
				while (enumerator.MoveNext ()) {
					BallScript chainableBall = enumerator.Current as BallScript;
					RecursiveFunctionToGetBestPath (chainableBall, new List<BallScript>(result));
				}
			} else {
				if (result.Count > hintBalls.Count) {
					hintBalls = result;
				}
			}
		} else {
			return null;
		}

		return hintBalls;
	}

	//Show the ball that has the longest possible chain
	public void ShowHint(){
		if (gameManager.IsAllBallNotMoving ()) {
			hintBalls = new List<BallScript> ();
			IEnumerator enumerator = gameManager.ballGenerator.GetAllBalls ();
			while (enumerator.MoveNext ()) {
				Transform ballTransform = enumerator.Current as Transform;
				BallScript ball = ballTransform.GetComponent<BallScript> ();
				RecursiveFunctionToGetBestPath (ball, null);
			}
			StartCoroutine(DrawHintLines (hintBalls));
			IEnumerator enumeratorBestHint = hintBalls.GetEnumerator();
			while (enumeratorBestHint.MoveNext ()) {
				BallScript ball = enumeratorBestHint.Current as BallScript;
				ball.Hint ();
			}
		}
	}
}