using UnityEngine;
using System.Collections;

public class BallScript : MonoBehaviour {
	public Transform scorePrefab;
	int index;
	Sprite sprite;
	Animator animator;

	// Use this for initialization
	void Awake () {
		animator = GetComponent<Animator> ();
	}

	public void switchAnimationToSelected(){
		animator.SetBool ("isSelectable", false);
		animator.SetBool ("isSelected", true);
	}

	public void switchAnimationToRetrieved(){
		animator.SetTrigger ("isRetrieved");
	}

	public void switchAnimationToSelectable(){
		animator.SetBool ("isSelected", false);
		animator.SetBool ("isSelectable", true);
	}

	public void switchAnimationToIdle(){
		animator.SetBool ("isSelected", false);
		animator.SetBool ("isSelectable", false);
	}

	public void hint(){
		animator.SetBool ("isSelected", false);
		animator.SetBool ("isSelectable", true);
		Invoke ("switchAnimationToIdle", 1.0f);
		
	}

	public void initiate(int index, Sprite sprite){
		this.index = index;
		this.sprite = sprite;
		refresh ();
	}

	public void refresh(){
		SpriteRenderer ballSpriteRenderer = transform.GetComponentsInChildren<SpriteRenderer>()[0];
		ballSpriteRenderer.sprite = sprite;
		switchAnimationToIdle ();
	}

	public float retrieved(int score){
		switchAnimationToRetrieved ();
		Vector3 position = transform.position;
		position.z -= 1;
		Transform scoreText = Instantiate (scorePrefab, position, Quaternion.identity) as Transform;
		scoreText.GetComponent<ScoreEffectScript> ().setScore (score);
		Destroy (scoreText.gameObject, animator.GetCurrentAnimatorStateInfo (0).length);
		Destroy (gameObject, animator.GetCurrentAnimatorStateInfo(0).length);
		return animator.GetCurrentAnimatorStateInfo (0).length;
	}

	public int getIndex(){
		return this.index;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
