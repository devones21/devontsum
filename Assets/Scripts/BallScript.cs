using UnityEngine;
using System.Collections;

public class BallScript : MonoBehaviour {
	public Transform scorePrefab;
	int index;
	public int id;
	Sprite sprite;
	Animator animator;
	Rigidbody2D theRigidbody;

	static class Constants
	{
		public const string isSelected = "isSelected";
		public const string isSelectable  = "isSelectable"; 
		public const string isRetrieved  = "isRetrieved"; 
		public const string isForceIdle  = "isForceIdle"; 
	}


	void Awake () {
		theRigidbody = GetComponent<Rigidbody2D> ();
		animator = GetComponent<Animator> ();
	}

	public void Selected(){
		SwitchAnimationToSelected ();
	}

	public void Retrieved(){
		SwitchAnimationToRetrieved ();
		StartCoroutine(Destroy (animator.GetCurrentAnimatorStateInfo(0).length));
	}

	public void Selectable(){
		SwitchAnimationToSelectable ();
	}

	public void Idle(){
		SwitchAnimationToIdle ();
	}

	public void ForceIdle(){
		animator.SetTrigger (Constants.isForceIdle);
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
		Invoke ("SwitchAnimationToIdle", 1.0f);
		
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

	public float RetrievedAddScore(int score){
		SwitchAnimationToRetrieved ();
		Vector3 position = transform.position;
		position.z -= 1;
		Transform scoreText = Instantiate (scorePrefab, position, Quaternion.identity) as Transform;
		scoreText.GetComponent<ScoreEffectScript> ().SetScore (score);
		Destroy (scoreText.gameObject, animator.GetCurrentAnimatorStateInfo (0).length);
		StartCoroutine(Destroy (animator.GetCurrentAnimatorStateInfo(0).length));
		return animator.GetCurrentAnimatorStateInfo (0).length;
	}

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

	private IEnumerator Destroy(float delayTime){
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
		//gameObject.SetActive (false);
	}


//	void OnDestroy() {
//		GameObject ballGeneratorObject = GameObject.Find ("BallGenerator");
//		if (ballGeneratorObject != null) {
//			BallGeneratorScript ballGenerator = ballGeneratorObject.GetComponent<BallGeneratorScript> ();
//			ballGenerator.GetBallsBasedOnIndex (index).Remove (this);
//		}
//	}
}
