using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyTrigger : MonoBehaviour {
	private Button button;
	
	public bool SingleRun = true;
	
	void Start() {
		button = GetComponent<Button>();
	}
	
	private void OnTriggerEnter2D( Collider2D collision ) {
		if ( collision.CompareTag("Player"))  button.onClick.Invoke();
		if ( SingleRun ) Destroy(gameObject);
	}
}
