using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebugBallGeneratorScript : BallGeneratorScript {


	//Fill Ball Dictionary with balls that are already attached instead of generating new balls
	public override void Restart(){
		IEnumerator enumerator = GetAllBalls ();
		while (enumerator.MoveNext ()) {
			Transform ballTransform = enumerator.Current as Transform;
			BallScript ball = ballTransform.GetComponent<BallScript> ();
			int ballIndex = ball.Index;
			if (!ballDictionary.ContainsKey (ballIndex)) {
				ballDictionary.Add (ballIndex, new List<BallScript> ());
			}
			ball.Initiate (ballIndex, gameManager.sprites [ballIndex]);
			ballDictionary [ballIndex].Add (ball);
			ball.ForceIdle ();
			InvokedBalls++;
		}
	}
}
