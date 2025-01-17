using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeFly : MonoBehaviour
{
	public int damage = 10;
	private GameHandler gameHandlerObj;

	void Start () {
		GameObject gameHandlerLocation = GameObject.FindWithTag ("GameHandler");
		if (gameHandlerLocation != null) {
			gameHandlerObj = gameHandlerLocation.GetComponent<GameHandler> ();
		}
	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.gameObject.tag == "Player") {
			gameHandlerObj.TakeDamage (damage);
			Destroy(gameObject);
		}
		if(other.gameObject.tag == "monsterShooter")
        {
			Destroy(other.gameObject);
			Destroy(gameObject);
        }

	}
}
