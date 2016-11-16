using UnityEngine;
using System.Collections;

public class BallScript : MonoBehaviour {
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

	public void switchAnimationToSelectable(){
		animator.SetBool ("isSelected", false);
		animator.SetBool ("isSelectable", true);
	}

	public void switchAnimationToIdle(){
		animator.SetBool ("isSelected", false);
		animator.SetBool ("isSelectable", false);
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

	public void retrieved(){
		Destroy (gameObject);
	}

	public int getIndex(){
		return this.index;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
