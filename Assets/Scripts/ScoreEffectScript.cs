using UnityEngine;
using System.Collections;

public class ScoreEffectScript : MonoBehaviour {
	public Color[] colors = new Color[5];

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		transform.Translate(Vector3.up * Time.deltaTime, Camera.main.transform);
	}

	public void setScore(int score){
		TextMesh textMesh = GetComponent<TextMesh> ();
		textMesh.text = score.ToString ();
		if (score > 12800) {
			textMesh.color = colors [4];
			textMesh.characterSize = 2.0f;
		} else if (score > 3200) {
			textMesh.color = colors [3];
			textMesh.characterSize = 1.75f;
		} else if (score > 800) {
			textMesh.color = colors [2];
			textMesh.characterSize = 1.5f;
		} else if (score > 200) {
			textMesh.color = colors [1];
			textMesh.characterSize = 1.25f;
		} else {
			textMesh.color = colors [0];
			textMesh.characterSize = 1.0f;
		}

	}
}
