using UnityEngine;
using System.Collections;

public class BallScript : MonoBehaviour {
	int index;
	Color color;

	public void initiate(int index, Color color){
		this.index = index;
		this.color = color;
		refresh ();
	}

	public void refresh(){
		SpriteRenderer ballSpriteRenderer = GetComponent <SpriteRenderer>();
		ballSpriteRenderer.color = color;
	}

	public void retrieved(){
		Destroy (gameObject);
	}

	public int getIndex(){
		return this.index;
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
