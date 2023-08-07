using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class panel : MonoBehaviour {
	void Start() {
		Audio.Instance.PjLifes = 11;
		Audio.Instance.PjCoins =  0;
		Audio.Instance.DActiveIUIngame();
	}
	void LoadScene() {
		SceneManager.LoadScene(1);
	}
}
