using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class GameManagerScript : MonoBehaviour {
	public ComboTextScript comboText; //Text for combo
	public ScoreTextScript scoreText; //Text for ongoing score
	public Text resultScoreText; //Text of score on result pane
	public CountdownScript countdown; //Game ongoing coundown
	public StartCountdownScrpt startCountdown; //Countdown before game starts
	public Image resultPanel; //Result panel to be shwon when game ends
	public Color readyColor; //Color of text when game is ready to be played
	public Color notReadyColor; //Color of text when game is not ready to be played
	public Sprite[] sprites; //Sprites of the balls
	public Sprite bombSprite; //Sprite for bomb
	public BallGeneratorScript ballGenerator; //Ball Generator is needed to get the balls and their conditions
	public LineManagerScript lineManagerScript; //Line Manager that manage user interface
	public float raycastWidth = 1.0f; //Width of raycast used for chain
	public float time; //Game time length
	public float hintTime; //Auto hint time
	public int minChainForBomb = 7; //Min of ball need to be chained to make a bomb
	public Transform explosionPoint; //Center point for explosion that is used for shake
	public float explosionRadius; //Explosion radius of shake
	public float explosionPower; //Explosion power for shake
	public LayerMask ballLayer; //Layer Mask of balls
	public CoinScript coinPrefab; //Prefab of coins
	bool isPlaying; //Bool to check if game is playing or not
	float hintTimeLeft; //Auto hint time left
	Button shakeButton; //Shake Button
	Button pauseButton; //Pause Button
	GameObject canvas; //Canvas Game Object
	RectTransform canvasRect; //Rect Transform of canvas
	Camera UICamera; //The camera for UIs

	public float HintTimeLeft{
		get{
			return hintTimeLeft;
		}
		set{
			hintTimeLeft = value;
		}
	}

	public bool IsPlaying{
		get{
			return isPlaying;
		}
		set{
			isPlaying = value;
		}
	}

	// Use this for initialization
	void Start () {
		Screen.fullScreen = false;
		UICamera = GameObject.Find ("UI Camera").GetComponent<Camera> ();
		canvas = GameObject.Find ("Canvas");
		canvasRect= canvas.GetComponent<RectTransform>();
		shakeButton = GameObject.Find ("ShakeButton").GetComponent<Button> ();
		pauseButton = GameObject.Find ("PauseButton").GetComponent<Button> ();
		countdown.CountdownTime = time;
		lineManagerScript.MinChainForBomb = minChainForBomb;
		StartCountdown ();
	}

	// Update is called once per frame
	void Update () {
		if (isPlaying) {
			if(shakeButton != null && !shakeButton.interactable) shakeButton.interactable = true;
			if(pauseButton != null && !pauseButton.interactable) pauseButton.interactable = true;
			hintTimeLeft -= Time.deltaTime;
			if (hintTimeLeft < 0.0f && !lineManagerScript.IsChaining ()) {
				lineManagerScript.ShowHint ();
				ResetHintCountdown ();
			}
		} else {
			if (shakeButton != null && shakeButton.interactable) shakeButton.interactable = false;
			if (pauseButton != null && pauseButton.interactable) pauseButton.interactable = false;
		}
	}


	//Reset hint time left
	public void ResetHintCountdown(){
		hintTimeLeft = hintTime;
	}

	//Called everytime start countdown needs to be started , GameOver need to be called before this
	public void StartCountdown(){
		IsPlaying = false;
		scoreText.Score = 0;
		resultPanel.gameObject.SetActive (false);
		startCountdown.Restart ();
		startCountdown.gameObject.SetActive (true);
	}

	//Called when start coundown ends
	public void Restart(){
		IsPlaying = true;
		hintTimeLeft = hintTime;
		lineManagerScript.enabled = true;
		lineManagerScript.Restart ();
		ballGenerator.Restart ();
		countdown.Restart ();
		startCountdown.gameObject.SetActive (false);
	}

	//Called when game ends
	public void GameOver(){
		IsPlaying = false;
		RetrieveRemainingCoin ();
		resultScoreText.text = scoreText.Score.ToString ("000000");
		lineManagerScript.Restart ();
		lineManagerScript.enabled = false;
		comboText.Idle ();
		comboText.Combo = 0;
		scoreText.Idle ();
		ballGenerator.RetrieveAllBalls ();
		resultPanel.gameObject.SetActive (true);
	}

	//Reload the balls and set their animation to Idle
	public void RefreshAllBalls(){
		IEnumerator enumerator = ballGenerator.transform.GetEnumerator ();
		while (enumerator.MoveNext ()) {
			Transform ballTransform = enumerator.Current as Transform;
			ballTransform.GetComponent<BallScript> ().Idle();
		}
	}

	//Check if all balls is moving or not
	public bool IsAllBallNotMoving(){
		IEnumerator enumerator = ballGenerator.GetAllBalls();
		while(enumerator.MoveNext ()){
			Transform child = enumerator.Current as Transform;
			BallScript ball = child.GetComponent<BallScript>();
			if(ball.IsMoving()){
				return false;
			}
		};
		return true;
	}

	//Add score and put it into ScoreText
	public void AddScore(int scoreAdded){
		scoreText.Score += scoreAdded;
	}


	//Add 1 combo point
	public void AddCombo(){
		comboText.Combo++;
	}

	//Shake the balls
	public void Shake(){
		Vector2 explosionPos = new Vector2 (explosionPoint.transform.position.x, explosionPoint.transform.position.y);
		Collider2D[] colliders = Physics2D.OverlapCircleAll(explosionPos, explosionRadius, ballLayer);
		Debug.Log ("Shake: " + colliders.Length);
		foreach (Collider2D hit in colliders) {
			Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
			rb.AddForce(Vector2.up * explosionPower);
		}
	}

	//Pause the game
	public void Pause(){
		if (Time.timeScale > 0.0f) {
			Time.timeScale = 0.0f;
		} else {
			Time.timeScale = 1.0f;
		}
	}

	public void GenerateCoin(Vector3 generatePoint, int score){
		CoinScript coin = Instantiate (coinPrefab, Vector3.zero, Quaternion.identity);
		coin.gameObject.name = "Coin";
		coin.Score = score;
		coin.transform.SetParent(canvas.transform);
		coin.transform.position = ConvertWorldPositionToCanvasPosition (generatePoint);
	}

	//Function to convert position from main camera to positon in canvas
	//Source: http://answers.unity3d.com/questions/799616/unity-46-beta-19-how-to-convert-from-world-space-t.html
	public Vector2 ConvertWorldPositionToCanvasPosition(Vector3 worldPoint){
		Vector2 ViewportPosition=UICamera.WorldToViewportPoint(worldPoint);
		Vector2 screenPosition = new Vector2
		(
			ViewportPosition.x * canvasRect.sizeDelta.x,
			ViewportPosition.y * canvasRect.sizeDelta.y
		);

		return screenPosition;
	}

	public void RetrieveRemainingCoin(){
		IEnumerator enumerator = canvas.transform.GetEnumerator ();
		while (enumerator.MoveNext ()) {
			Transform transform = enumerator.Current as Transform;
			CoinScript coinScript = transform.GetComponent<CoinScript> ();
			if (coinScript != null) {
				scoreText.Score += coinScript.Score;
			}
		}
	}
}
