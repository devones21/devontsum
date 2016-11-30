using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ComboTextScript : MonoBehaviour {
	Animator animator;
	int combo = 0;
	Text comboText;
	bool isCombo = false;

	//Constants for animations
	public static class Constants
	{
		public const string triggerCombo = "triggerCombo";
		public const string comboAnimation = "ComboText_Combo";
		public const string idleAnimation = "ComboText_Idle";
	}

	public int Combo
	{
		get{
			return combo;
		}
		set{
			combo = value;
			SetCombo (combo);
		}
	}

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator> ();
		comboText = GetComponent<Text> ();
	}


	// Update is called once per frame
	void Update () {
		if (isCombo) {
			isCombo = animator.GetCurrentAnimatorStateInfo (0).IsName (Constants.comboAnimation);
		}
	}

	public void Idle(){
		SwitchAnimationToIdle ();
	}

	void SwitchAnimationToCombo(){
		animator.Play(Constants.comboAnimation, -1, 0f);
	}

	void SwitchAnimationToIdle(){
		animator.Play(Constants.idleAnimation, -1, 0f);
	}

	void SetCombo(int combo){
		if (combo != 0) {
			if (!isCombo) {
				//if idle restart from 1
				this.combo = 1;
			} else {
				//if not continue combo
				this.combo = combo;
			}
			comboText.text = this.combo.ToString () + " combo";

			SwitchAnimationToCombo ();
			isCombo = true;
		}
	}
}
