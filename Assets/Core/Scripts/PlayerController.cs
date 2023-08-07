using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	private Animator      anim;
	private Rigidbody2D   rigi;
	private BoxCollider2D foot, head, side, front, climb;

	private Vector3 respawn;  // Posicion de reaparecimiento
	private string  nameClip; // Nombre de clip actual de la animacion
	private bool    blnNoClimb, blnBreak;
	private float   timeJump, timeNoMove;

	private string anmsFoot = "IdleLand";
	private string anmsMove = "WalkRun cycle";

	public bool   blnBlock , blnFxdUpdt, blnInvulnerable = false;
	private float  velocity  = 12f;
	public float    health   = 10f;
	public float    stamina  = 10f;
	private LayerMask L_W;
	public int nextLvl;

	void Start () {
		L_W = LayerMask.GetMask("Default");
		anim  = GetComponent<Animator>();    // Animator
		rigi  = GetComponent<Rigidbody2D>(); // Rigidbody2D
	
		respawn = transform.position;

		foot  = GetMyBox("Collision/Foot");  // * Optenemos los box trigger del Personaje
		head  = GetMyBox("Collision/Head");
		side  = GetMyBox("Collision/Side");
		front = GetMyBox("Collision/Front");
		climb = GetMyBox("Collision/Climb");
	}

	void Update() {
		if ( blnBlock ) return; // * Si esta bloqueado no actualiza Inputs 
		AnimatorClipInfo[] nameClipInfo = anim.GetCurrentAnimatorClipInfo(0);
		if (nameClipInfo.Length > 0) nameClip = nameClipInfo[0].clip.name;
		float orientation = Input.GetAxis("Horizontal");

		if ( rigi.bodyType == RigidbodyType2D.Dynamic || blnNoClimb) { // * No esta escalando o colgado
			if ( Mathf.Abs(orientation) > 0 ) {                          // * Se esta moviendo
				
				Vector2 velocidadActual = rigi.velocity;          // ! Mover Personaje
				velocidadActual.x       = orientation * velocity;
				rigi.velocity           = velocidadActual;
				if ( (anmsFoot.Contains(nameClip) || anmsMove.Contains(nameClip) ) && !side.IsTouchingLayers(L_W) )
					anim.Play(velocity < 13 ? "Walk" : "Run cycle");
				if ( anmsMove.Contains(nameClip) && !foot.IsTouchingLayers(L_W) ) anim.Play("JumpEnd");

				transform.localScale = new Vector3(orientation < 0 ? -1 : 1, 1, 1);
			} else if ( foot.IsTouchingLayers(L_W) && anmsMove.Contains(nameClip) && timeJump == 0f ) { // * Paro de moverse
				rigi.velocity = new Vector2(0, 0); // ? no deslice en el suelo
				anim.Play("Idle");
				timeNoMove = 0f;
			} else if ( rigi.velocity.magnitude < 0.5f && timeNoMove < 0.5f ) {     // * No se mueve
				timeNoMove += Time.deltaTime;
				if ( timeNoMove > 0.5f && !blnBlock ) CheckPoint();
			}

			if ( !foot.IsTouchingLayers(L_W) ) timeJump += Time.deltaTime;  // * Se encuentra en el aire
			if ( Input.GetKey(KeyCode.UpArrow) && 0.15f > timeJump ) {         // ! Saltar, Incluimos coyote time = 0.15f
				rigi.velocity = new Vector2(rigi.velocity.x, velocity * (1.2f + Time.deltaTime) );
				anim.Play("JumpUp");
				Audio.Instance.PlayEffect("jump");
			}
			if ( nameClip.Contains("JumpUp") && rigi.velocity.y < 0 )	anim.Play("JumpMid"); // * Mitad de salto
		} else {                                           // * Esta escalando o colgado
			if ( Input.GetKeyDown(KeyCode.DownArrow) || stamina < 0.02f) {     // * Soltarse Forzado
				rigi.bodyType = RigidbodyType2D.Dynamic;
				blnNoClimb = true;
				anim.Play("JumpEnd");
			}
			if ( nameClip.Contains("Side") ) {               // * Si esta colgado a la pared
				if ( Input.GetKey( KeyCode.UpArrow ) && !head.IsTouchingLayers(L_W) ) { // * Escalar
					float vertical = Input.GetAxis("Vertical");
					Vector3 climbDirection = transform.up*vertical*8*Time.deltaTime;
					transform.Translate(climbDirection);
					anim.Play("Side Climb");
				} else anim.Play("Side");                             // * Para de Escalar
				if ( !climb.IsTouchingLayers(L_W) ) LedgeClimb();     // * Fin de escalar subir al suelo
			} else {
				if( Input.GetKeyDown(KeyCode.UpArrow) ) LedgeClimb(); // * Subir al suelo
			}
		}
	}

	void FixedUpdate(){
		if ( blnFxdUpdt ) return;

		stamina = stamina < 10f ? stamina + 0.05f : 10f;
		if ( stamina > 4f ) blnBreak = false;

		if ( rigi.bodyType == RigidbodyType2D.Dynamic || blnNoClimb) {
			if (!blnBreak && Input.GetKey( KeyCode.Q ) && stamina > 0.2f) {
				velocity = 24;
				stamina -= 0.2f;
				if (stamina < 0.2f) {
					blnBreak = true;
					Audio.Instance.PlayEffect("fatigue");
				}
			} else velocity = 12;
			Vector2 nuevaVelocidad = rigi.velocity;
			nuevaVelocidad.x = Mathf.Clamp(rigi.velocity.x, -64f, 64f); // Limitar Velocidad
			nuevaVelocidad.y = Mathf.Clamp(rigi.velocity.y, -64f, 64f); // Limitar Velocidad
			rigi.velocity    = nuevaVelocidad;
		} else {
			stamina -= 0.07f;
		}
	}
	
	private void OnTriggerEnter2D(Collider2D other) {
		AnimatorClipInfo[] nameClipInfo = anim.GetCurrentAnimatorClipInfo(0);
		if (nameClipInfo.Length > 0) nameClip = nameClipInfo[0].clip.name;
		if ( nameClip == "Death") return;
		if ( other.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast") ) return;
		if ( other.gameObject.layer == LayerMask.NameToLayer("Damage") ) {
			if ( ! blnInvulnerable ) {
				health -= 3;
				blnInvulnerable = true;
				Invoke("Vulnerable", 0.3f);
				Audio.Instance.PlayEffect("hurt");
				anim.Play("Hurt");
			}
			return;
		}
		if ( foot.IsTouchingLayers(L_W) ) {
			if ( timeJump > .6f ) anim.Play("Land");
			else                  anim.Play("Idle");
			timeJump   = 0f;
			blnNoClimb = false;
			blnNoClimb = false;
			rigi.bodyType = RigidbodyType2D.Dynamic;
		} else if ( front.IsTouchingLayers(L_W) && !blnNoClimb && !head.IsTouchingLayers(L_W)) {
			rigi.bodyType = RigidbodyType2D.Kinematic;
			if ( side.IsTouchingLayers(L_W) ) anim.Play("Side");
			else if(!climb.IsTouchingLayers(L_W)) {
				anim.Play("Ledge In");
			} else {
				rigi.bodyType = RigidbodyType2D.Dynamic;
			}
		}
		rigi.velocity = new Vector2(0,0);
	}

	public void Respawn()   {
		anim.Play("Idle");
		rigi.velocity = Vector3.zero;
		rigi.angularVelocity = 0f;
		rigi.Sleep();
		health    = 10f;
		timeJump   = 0f;
		Audio.Instance.SetBars(health, stamina);
		transform.position = respawn;
		blnInvulnerable    = true;
		Audio.Instance.PlayEffect("respawn");
		Invoke("Vulnerable", 0.8f);
	}
	public void CheckPoint(){	respawn = transform.position; }
	public void ActiveFxd() { blnFxdUpdt = true ; Audio.Instance.HealthBar.SetActive(false); }
	public void DesactFxd() { blnFxdUpdt = false; Audio.Instance.HealthBar.SetActive(true);  }

	void LedgeClimb() {
		Vector3 movement = new Vector3(transform.position.x + transform.localScale.x, transform.position.y + 2, transform.position.z);
		transform.position = movement;
		rigi.bodyType = RigidbodyType2D.Static;
		anim.Play("Ledge climb");
	}
	void LedgeClimbEnd() { rigi.bodyType = RigidbodyType2D.Dynamic;}

	public void Vulnerable(){ blnInvulnerable = false; }
	private BoxCollider2D GetMyBox(string str) {
		BoxCollider2D box  = transform.Find(str).GetComponent<BoxCollider2D>();
		if  (box == null )
			Debug.LogError("Error!!! No se encontro GetMyBox = " + str);
		return box;
	}
	public void DeadPlayer() {
		anim.Play("Death");
		health = 10f;
	}
	public void NextLevel() {
		rigi.bodyType = RigidbodyType2D.Static;
		Audio.Instance.PlayMusic("happy"); 
		anim.Play("happy");
		Invoke("End", 10f);
	}
	private void End() {
		 Audio.Instance.ChangeScena(nextLvl);
	}
}
