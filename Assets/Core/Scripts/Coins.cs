using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coins : MonoBehaviour{
	public int value = 1;
	private void OnTriggerEnter2D( Collider2D collision ) {
		if ( collision.CompareTag("Player") ) {
			Audio.Instance.AddCoins(value);
			Audio.Instance.PlayEffect("coin");
			Destroy(gameObject);
		}
	}
}
