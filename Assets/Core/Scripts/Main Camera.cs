using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MainCamera : MonoBehaviour {
	private static readonly Color ALFA  = new Color(1f, 1f, 1f, 0f);
	private static readonly Color WHITE = new Color(1f, 1f, 1f, 1f);
	private static readonly Color BLACK = new Color(0f, 0f, 0f, 1f);
	private static readonly Color RED   = new Color(1f, 0f, 0f, 1f);

	private  SpriteRenderer  sky;
	private  Light2D         lightG;
	public  List<IColorable> ArrColors = new List<IColorable>();
	
	public bool blnIuColor = true;
	public bool  blnDeath = false;

	public bool blnBackground = false;
	public GameObject water;
	public PlayerController player;
	public Gradient colors;
	public float    delay = 1f;

	private Color mainColor;
	private float[] spectrum = new float[1024];


	public Color BackColor;
	public float Lintensity;

	void Start() {
		sky  = transform.Find("Sky").GetComponent<SpriteRenderer>();
		if  (sky == null )
			Debug.LogError("Error!!! No se encontro sky in camera");

		lightG =  	gameObject.GetComponent<Light2D>();
		if  (lightG == null )
			Debug.LogError("Error!!! No se encontro lightG in camera");
	}

	void FixedUpdate() {
		if ( blnDeath ) { return; }

		if ( player.health < 0 && !blnDeath ) {
			
			blnDeath = true;
			gameObject.GetComponent<Camera>().backgroundColor = RED;
			lightG.intensity = 0f;
			player.DeadPlayer();
			Audio.Instance.PlayEffect("death");
			Audio.Instance.DeadPlayer();
			Audio.Instance.DActiveIUIngame();
			Invoke("ActiveIUInGame", 1.8f);
			return;
		} else if (player.health < 10f ) player.health += 0.001f;
		Audio.Instance.SetBars(player.health, player.stamina);
		
		if (transform.position.y - 15 < water.transform.position.y)	{
			water.SetActive(true);
			if ( player.transform.position.y < -10 ) {
				player.Respawn();
				Audio.Instance.DeadPlayer();
				return;
			}
		} else water.SetActive(false);
		if (water.activeSelf) {
			Vector3 waterPosition = water.transform.position;
			waterPosition.x = transform.position.x;
			water.transform.position = waterPosition;
		}

		Audio.Instance.music.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
		float intensity = 0f;
		for (int i = 0; i < spectrum.Length; i++) 
			intensity += spectrum[i];

		float icolor = ( (intensity)/spectrum.Length)*100;

		mainColor = Color.Lerp(mainColor, colors.Evaluate(icolor), Time.deltaTime * (delay*intensity));
		ChangeColors();
		BackColor  = gameObject.GetComponent<Camera>().backgroundColor;
		Lintensity = lightG.intensity;
	}

	public void ChangeColors () {
		foreach (var colorable  in ArrColors) {
			colorable.Color = mainColor;
		}
		if ( blnIuColor ) Audio.Instance.ChangeColors(mainColor);
		if ( blnBackground ) gameObject.GetComponent<Camera>().backgroundColor = mainColor;
	}
	void ActiveIUInGame(){
		lightG.intensity = Lintensity;
		gameObject.GetComponent<Camera>().backgroundColor = BackColor;
		Audio.Instance.ActiveIUIngame();
		blnDeath = false;
	}
}
