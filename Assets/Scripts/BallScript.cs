using UnityEngine;
using System.Collections;

public class BallScript : MonoBehaviour {
	public Transform scorePrefab;
	public int index;
	int id;
	Sprite sprite;
	Animator animator;
	Rigidbody2D theRigidbody;

	public int Index
	{
		get
		{
			return index;
		}
	}

	public int Id
	{
		get
		{
			return id;
		}
		set {
			id = value;
		}
	}

	//Constants for animations
	static class Constants
	{
		public const string isSelected = "isSelected";
		public const string isSelectable  = "isSelectable"; 
		public const string isRetrieved  = "isRetrieved"; 
		public const string isForceIdle  = "isForceIdle"; 
	}
		
	void Start () {
		theRigidbody = GetComponent<Rigidbody2D> ();
		animator = GetComponent<Animator> ();
	}

	//Enter Chained condtion
	public void Selected(){
		SwitchAnimationToSelected ();
	}

	//Retrieved 
	public void Retrieved(){
		SwitchAnimationToRetrieved ();
		StartCoroutine(Recycle (animator.GetCurrentAnimatorStateInfo(0).length));
	}

	//Enter Chainable condition
	public void Selectable(){
		SwitchAnimationToSelectable ();
	}

	//Return to idle condition
	public void Idle(){
		SwitchAnimationToIdle ();
	}

	//Force ball to enter idle immediately
	public void ForceIdle(){
		animator.SetTrigger (Constants.isForceIdle);
	}

	//Switch animation to selected
	public void SwitchAnimationToSelected(){
		if (animator == null) {
			animator = GetComponent<Animator> ();
		}
		animator.SetBool (Constants.isSelectable, false);
		animator.SetBool (Constants.isSelected, true);
	}

	//Switch animation to retrieved
	public void SwitchAnimationToRetrieved(){
		if (animator == null) {
			animator = GetComponent<Animator> ();
		}
		animator.SetTrigger (Constants.isRetrieved);
	}

	//Switch animation to selectable
	public void SwitchAnimationToSelectable(){
		if (animator == null) {
			animator = GetComponent<Animator> ();
		}
		animator.SetBool (Constants.isSelected, false);
		animator.SetBool (Constants.isSelectable, true);
	}

	//Switch animation to idle
	public void SwitchAnimationToIdle(){
		if (animator == null) {
			animator = GetComponent<Animator> ();
		}
		animator.SetBool (Constants.isSelected, false);
		animator.SetBool (Constants.isSelectable, false);
	}

	//Temporarily enter chainable animation as hint
	public void Hint(){
		SwitchAnimationToSelectable ();
		Invoke ("SwitchAnimationToIdle", 1.0f);
		
	}

	//Initiate/Restart ball
	public void Initiate(int index, Sprite sprite){
		this.index = index;
		this.sprite = sprite;

		SpriteRenderer ballSpriteRenderer = transform.GetComponentsInChildren<SpriteRenderer>()[0];
		if(ballSpriteRenderer != null) ballSpriteRenderer.sprite = sprite;
		Idle ();
	}

	//Check if baal is moving
	public bool IsMoving(){
		if (theRigidbody != null && GetComponent<Rigidbody2D>() != null) {
			if (Mathf.Abs (theRigidbody.velocity.y) > 1 || Mathf.Abs (theRigidbody.velocity.x) > 1) {
				return true;
			} else
				return false;
		} else {
			return false;
		}
	}

	//Retrieve and add score
	public float RetrievedAddScore(int score){
		SwitchAnimationToRetrieved ();
		Vector3 position = transform.position;
		position.z -= 1;
		Transform scoreText = Instantiate (scorePrefab, position, Quaternion.identity) as Transform;
		scoreText.GetComponent<ScoreEffectScript> ().SetScore (score);
		Destroy (scoreText.gameObject, animator.GetCurrentAnimatorStateInfo (0).length);
		StartCoroutine(Recycle (animator.GetCurrentAnimatorStateInfo(0).length));
		return animator.GetCurrentAnimatorStateInfo (0).length;
	}

	//Recycle the ball
	private IEnumerator Recycle(float delayTime){
		yield return new WaitForSeconds(delayTime);
		GameObject ballGeneratorObject = GameObject.Find ("BallGenerator");
		GameObject gameManagerObject = GameObject.Find ("GameManager");
		if (ballGeneratorObject != null && gameManagerObject  != null) {
			GameManagerScript gameManager = gameManagerObject.GetComponent<GameManagerScript>();
			BallGeneratorScript ballGenerator = ballGeneratorObject.GetComponent<BallGeneratorScript> ();
			ballGenerator.GetBallsBasedOnIndex (index).Remove (this);

			if (gameManager.IsPlaying) {
				ballGenerator.RecycleBall (this);
			}
		}
	}

	public void DisableRigidbody(){
		theRigidbody.isKinematic = true;
	}


	public void EnableRigidbody(){
		theRigidbody.isKinematic = false;
	}
}
