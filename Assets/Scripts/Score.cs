using System.Collections;
using GBaaS.io;
using GBaaS.io.Objects;
using GBaaS.io.Services;
using UnityEngine;

public class Score : MonoBehaviour {
	public int score = 0;					// The player's score.
	
	private PlayerControl playerControl;	// Reference to the player control script.
	private static int previousScore = 0;			// The score in the previous frame.
	
	void Awake () {
		Debug.Log("Score Awake");
		// Setting up the reference.
		playerControl = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();

		Debug.Log("playerControl Gameplayed? : " + PlayerControl.gamePlayed.ToString());
		if(PlayerControl.gamePlayed) {
			GBaaSObject aClient = new GBaaSObject();
			aClient.Init(null);

			PlayerControl.gamePlayed = false;
			Debug.Log("Score is : " + System.Convert.ToString(previousScore));

			aClient.AddScore(new GBScoreObject {
				stage = "1",
				score = previousScore,
				unit = "point"
			});

			if(previousScore >= 2000) {
				aClient.UpdateAchievement(1);
			}

			Application.LoadLevel(3); // Go Score Page
		}
	}

	void Update () {
		// Set the score text.
		guiText.text = "Score: " + score;

		// If the score has changed...
		if(previousScore != score && playerControl != null) {
			// ... play a taunt.
			playerControl.StartCoroutine(playerControl.Taunt());
		}

		// Set the previous score to this frame's score.
		previousScore = score;
	}
}
