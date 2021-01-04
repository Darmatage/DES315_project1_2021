using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
	public int damage = 1;
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
		}
	}
		
}
