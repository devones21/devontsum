using UnityEngine;
using System.Collections;

public class ScoreEffectScript : MonoBehaviour {
	public Color[] colors = new Color[5];

	// Update is called once per frame
	// Move to above until disappear
	void FixedUpdate () {
		transform.Translate(Vector3.up * Time.deltaTime, Camera.main.transform);
	}

	// Set the score text
	public void SetScore(int score){
		TextMesh textMesh = GetComponent<TextMesh> ();
		textMesh.text = score.ToString ();
		if (score > 10000) {
			textMesh.color = colors [4];
			textMesh.characterSize = 2.0f;
		} else if (score > 3000) {
			textMesh.color = colors [3];
			textMesh.characterSize = 1.75f;
		} else if (score > 1000) {
			textMesh.color = colors [2];
			textMesh.characterSize = 1.5f;
		} else if (score > 500) {
			textMesh.color = colors [1];
			textMesh.characterSize = 1.25f;
		} else {
			textMesh.color = colors [0];
			textMesh.characterSize = 1.0f;
		}

	}
}
