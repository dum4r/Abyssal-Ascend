using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Audio : MonoBehaviour {
	private static Audio instance;
	public  static Audio Instance { get { return instance; } }

	private GameObject      PlInGame;
	private Image           btnPause;
	private TextMeshProUGUI txtTitle;
	private GameObject      txtCoins;
	private GameObject      txtLifes;
	public  GameObject      HealthBar;

	public AudioSource music, efect;
	public Dictionary<string, AudioClip> MapMusic = new Dictionary<string, AudioClip>();
	public Dictionary<string, AudioClip> MapEfect = new Dictionary<string, AudioClip>();


	private Animator   animPause, animHearth;
	private GameObject PauseMenu;
	private float      pauseTime;
	public  bool       blnPause = false;

	public int         PjLifes     = 11;
	public int         PjCoins     =  0;
	public bool        blnInmortal = false;

	void Start() {
		MapMusic.Add("gameover" , Resources.Load<AudioClip>("Sounds/Music/GameOver"));
		MapMusic.Add("intro" , Resources.Load<AudioClip>("Sounds/Music/Light_Grind"));
		MapMusic.Add("happy" , Resources.Load<AudioClip>("Sounds/Music/HappyHappy"));
		MapMusic.Add("cap1" , Resources.Load<AudioClip>("Sounds/Music/Dream_About_Dreams"));
		MapMusic.Add("cap2" , Resources.Load<AudioClip>("Sounds/Music/Mary_Was_A_Spiritualist"));
		MapMusic.Add("cap3" , Resources.Load<AudioClip>("Sounds/Music/Moonlight_Sonata"));

		MapEfect.Add("coin" , Resources.Load<AudioClip>("Sounds/Effects/coin"));
		MapEfect.Add("hurt" , Resources.Load<AudioClip>("Sounds/Effects/hurt"));
		MapEfect.Add("jump" , Resources.Load<AudioClip>("Sounds/Effects/jump"));
		MapEfect.Add("death", Resources.Load<AudioClip>("Sounds/Effects/death"));
		MapEfect.Add("respawn", Resources.Load<AudioClip>("Sounds/Effects/respawn"));
		MapEfect.Add("fatigue" , Resources.Load<AudioClip>("Sounds/Effects/fatigue"));

		// TODO: Crear metodos para evitar redundancia
		music  = transform.Find("Music").GetComponent<AudioSource>();
		if  (music == null )
			Debug.LogError("Error!!! No se encontro Music in Audio");

		efect  = transform.Find("Efect").GetComponent<AudioSource>();
		if  (efect == null )
			Debug.LogError("Error!!! No se encontro Efect in Audio");

		animPause = transform.Find("Canvas/InGame/Pause").GetComponent<Animator>();
		if  (animPause == null )
			Debug.LogError("Error!!! No se encontro animPause in Audio");
		animHearth = transform.Find("Canvas/InGame/HealthBar").GetComponent<Animator>();
		if  (animHearth == null )
			Debug.LogError("Error!!! No se encontro animHearth in Audio");
		
		PauseMenu = transform.Find("Canvas/InGame/PauseMenu").gameObject;
		if  (PauseMenu == null )
			Debug.LogError("Error!!! No se encontro PauseMenu in Audio");

		btnPause = transform.Find("Canvas/InGame/Pause").GetComponent<Image>();
		if  (btnPause == null )
			Debug.LogError("Error!!! No se encontro btnPause in Audio");
		txtTitle = transform.Find("Canvas/InGame/PauseMenu/Title").GetComponent<TextMeshProUGUI>();
		if  (txtTitle == null )
			Debug.LogError("Error!!! No se encontro txtTitle in Audio");
				txtTitle = transform.Find("Canvas/InGame/PauseMenu/Title").GetComponent<TextMeshProUGUI>();
		txtCoins = transform.Find("Canvas/InGame/Coins").gameObject;
		if  (txtCoins == null )
			Debug.LogError("Error!!! No se encontro txtCoins in Audio");
		txtLifes = transform.Find("Canvas/InGame/Lifes").gameObject;
		if  (txtLifes == null )
			Debug.LogError("Error!!! No se encontro txtLifes in Audio");
		
		HealthBar = transform.Find("Canvas/InGame/HealthBar").gameObject;
		if  (txtLifes == null )
			Debug.LogError("Error!!! No se encontro HealthBar in Audio");
	}

	void Update(){
		if ( !txtCoins.activeSelf && PjCoins > 0 ) txtCoins.SetActive(true);
		if ( !txtLifes.activeSelf && PjLifes < 11 ) txtLifes.SetActive(true);
		if ( blnPause ) {
			float deltaTime = Time.realtimeSinceStartup - pauseTime;
			animPause.Play("ClickPause", 0, deltaTime);
			if ( deltaTime >= 1 ) {
				animPause.StopPlayback(); // Detener la animación
				blnPause = false;         // Desactivar la pausa
				animPause.Play("BtnRun"); // Reproducir otra animación si es necesario
			}
		}
	}

	private void Awake() {
		if (instance != null && instance != this) {
			Destroy(gameObject);
			return;
		}
		instance = this;
		DontDestroyOnLoad(gameObject);
	}

	public void PauseGame() {
		if ( Time.timeScale == 0f ) {
			Time.timeScale = 1f;
			animPause.Play("ClickRun");
			PauseMenu.SetActive(false);
		} else {
			Time.timeScale = 0f;
			blnPause = true;
			pauseTime = Time.realtimeSinceStartup;
			PauseMenu.SetActive(true);
		}
	}

	public void ChangeScena(int scena){ SceneManager.LoadScene(scena); }
	public void ChangeColors( Color color ) {
		btnPause.color = color;
		txtTitle.color = color;
	}
	public void ChangeInmortal () { blnInmortal = !blnInmortal; }
	public void DeadPlayer() {
		PjLifes -= 1;
		if ( blnInmortal ) {
			PjLifes = 11;
			return;
		}
		if (PjLifes < 0 ) GameOver();
		else txtLifes.GetComponent<TextMeshProUGUI>().text = PjLifes.ToString();
	}
	public void AddCoins(int coins) { 
		PjCoins += coins;
		txtCoins.GetComponent<TextMeshProUGUI>().text = PjCoins.ToString();
	}

	private void GameOver() {
		transform.Find("Canvas/GameOver").gameObject.SetActive(true);
		PlayMusic("gameover");
	}

	public void ActiveIUIngame()  { transform.Find("Canvas/InGame").gameObject.SetActive(true ); }
	public void DActiveIUIngame() { transform.Find("Canvas/InGame").gameObject.SetActive(false);; }
	public void SetBars(float health, float stamina) {
		HealthBar.GetComponent<Slider>().value = health;
		animHearth.speed = 11f - stamina;
	}
	public void PlayEffect(string soundName){
		if (MapEfect.ContainsKey(soundName)) {
			AudioClip soundClip = MapEfect[soundName];
			efect.PlayOneShot(soundClip);
			// efect.Stop();
			// efect.clip = soundClip;
			// efect.Play(); 
		}
		else Debug.LogWarning("El PlayEffect sonido '" + soundName + "' no se encuentra en el mapa.");
	}
	public void PlayMusic(string soundName){
		if (MapMusic.ContainsKey(soundName)) {
			AudioClip soundClip = MapMusic[soundName];
			music.Stop();
			music.clip = soundClip;
			music.Play();
		}
		else Debug.LogWarning("El PlayMusic sonido '" + soundName + "' no se encuentra en el mapa.");
	}
}