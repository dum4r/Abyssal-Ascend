using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour {
	void Start() {
		Audio.Instance.DActiveIUIngame();
		Audio.Instance.PlayMusic("intro");
	}
	public void Play(){
		Audio.Instance.ChangeScena(2);
	}
	public void Salir() {
		#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
		#endif
		Application.Quit();
	}
}
