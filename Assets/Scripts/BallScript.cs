using UnityEngine;
using System.Collections;

public class BallScript : MonoBehaviour {
	public Transform scorePrefab;
	int index;
	Sprite sprite;
	Animator animator;
	Rigidbody2D theRigidbody;

	static class Constants
	{
		public const string isSelected = "isSelected";
		public const string isSelectable  = "isSelectable"; 
		public const string isRetrieved  = "isRetrieved"; 
	}

	// Use this for initialization
	void Awake () {
		theRigidbody = GetComponent<Rigidbody2D> ();
		animator = GetComponent<Animator> ();
	}

	public void Selected(){
		SwitchAnimationToSelected ();
	}

	public void Retrieved(){
		SwitchAnimationToRetrieved ();
	}

	public void Selectable(){
		SwitchAnimationToSelectable ();
	}

	public void Idle(){
		SwitchAnimationToIdle ();
	}

	public void SwitchAnimationToSelected(){
		animator.SetBool (Constants.isSelectable, false);
		animator.SetBool (Constants.isSelected, true);
	}

	public void SwitchAnimationToRetrieved(){
		animator.SetTrigger (Constants.isRetrieved);
	}

	public void SwitchAnimationToSelectable(){
		animator.SetBool (Constants.isSelected, false);
		animator.SetBool (Constants.isSelectable, true);
	}

	public void SwitchAnimationToIdle(){
		animator.SetBool (Constants.isSelected, false);
		animator.SetBool (Constants.isSelectable, false);
	}

	public void Hint(){
		SwitchAnimationToSelectable ();
		Invoke ("switchAnimationToIdle", 1.0f);
		
	}

	public void Initiate(int index, Sprite sprite){
		this.index = index;
		this.sprite = sprite;
		Refresh ();
	}

	public void Refresh(){
		SpriteRenderer ballSpriteRenderer = transform.GetComponentsInChildren<SpriteRenderer>()[0];
		ballSpriteRenderer.sprite = sprite;
		Idle ();
	}

	public bool IsMoving(){
		if (theRigidbody != null) {
			if (Mathf.Abs (theRigidbody.velocity.y) > 1 || Mathf.Abs (theRigidbody.velocity.x) > 0.1f) {
				return true;
			} else
				return false;
		} else {
			return false;
		}
	}

	public float Retrieved(int score){
		Retrieved ();
		Vector3 position = transform.position;
		position.z -= 1;
		Transform scoreText = Instantiate (scorePrefab, position, Quaternion.identity) as Transform;
		scoreText.GetComponent<ScoreEffectScript> ().SetScore (score);
		Destroy (scoreText.gameObject, animator.GetCurrentAnimatorStateInfo (0).length);
		Destroy (gameObject, animator.GetCurrentAnimatorStateInfo(0).length);
		return animator.GetCurrentAnimatorStateInfo (0).length;
	}

	public int Index
	{
		get
		{
			return index;
		}
	}
}
