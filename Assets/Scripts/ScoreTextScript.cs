using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreTextScript : MonoBehaviour {
	int score = 0;
	Text scoreText;
	Animator animator;

	public int Score{
		get{
			return score;
		}
		set{
			score = value;
			SetScore (score);
		}
	}

	//Constants for animations
	public static class Constants
	{
		public const string triggerScoreAdded = "triggerScoreAdded";
		public const string scoreAddedAnimation = "ScoreText_ScoreAdded";
		public const string idleAnimation = "ScoreText_Idle";
	}

	void Start(){
		animator = GetComponent<Animator> ();
		scoreText = GetComponent<Text> ();
	}

	public void Idle(){
		SwitchAnimationToIdle ();
	}

	void SwitchAnimationToIdle(){
		animator.Play(Constants.idleAnimation, -1, 0f);
	}

	void SwitchAnimationToScoreAdded(){
		animator.SetTrigger (Constants.triggerScoreAdded);
	}


	//Set score and put it into ScoreText
	public void SetScore(int score){
		this.score = score;
		if (score > 0) {
			if(!animator.GetCurrentAnimatorStateInfo(0).IsName(Constants.scoreAddedAnimation)){
				SwitchAnimationToScoreAdded ();
			}
		}
		string scoreString = score.ToString ("0000000");
		if (scoreText == null) {
			scoreText = GetComponent<Text> ();
		}
		scoreText.text = scoreString;
	}
}
