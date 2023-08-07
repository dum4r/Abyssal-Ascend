
// using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering.Universal;

public interface IColorable { Color Color { get; set; } } // * Interfaz para manejar el Color

// ! Init es un objeto temporal pasa asignar todos los objetos que queremos que interactuen con el color
public class Init : MonoBehaviour {
	private class ColorableSprite : IColorable {                             // * Clase para Sprites
		private SpriteRenderer sprite;                                         // SpriteRenderer
		public Color Color {                                                   // Implementamos Interfaz
			get { return sprite.color; }                                         // Get
			set { sprite.color = value; }                                        // Set
		}
		public ColorableSprite(SpriteRenderer sprite){ this.sprite = sprite; } // Constructor Convertimos a Interfaz
	}

	private class ColorableLight : IColorable {                             // * Clase para Light
		private Light2D light;                                                // Light2D
		public Color Color {                                                  // Implementamos Interfaz
			get { return light.color; }                                         // Get
			set { light.color = value; }                                        // Set
		}
		public ColorableLight(Light2D light){ this.light = light; }           // Constructor Convertimos a Interfaz
	}

	public class ColorableTilemap : IColorable {                             // * Clase para Tilemaps
		private Tilemap tilemap;                                               // Tilemap
		public Color Color {                                                   // Implementamos Interfaz
			get { return tilemap.color; }                                        // Get
			set { tilemap.color = value; }                                       // Set
		}
		public ColorableTilemap(Tilemap tilemap) { this.tilemap = tilemap; }   // Constructor Convertimos a Interfaz
	}

	public string music;

	public List<SpriteRenderer> ArrSprites = new List<SpriteRenderer>();     // Array de Sprites a convertir
	public List<Light2D>        ArrLights  = new List<Light2D>();            // Array de Light a convertir
	public List<Tilemap>       ArrTileMaps = new List<Tilemap>();            // Array de Tilemaps a convertir

	public MainCamera main;                                                  // Destino y Controlador del color

	void Start() {
		Audio.Instance.ActiveIUIngame();
		Audio.Instance.PlayMusic(music);
		List<IColorable> colorElements = new List<IColorable>();
		foreach (var sprite in ArrSprites) {
			IColorable colorableSprite = new ColorableSprite(sprite);
			if (colorableSprite != null) colorElements.Add(colorableSprite);
		}

		foreach (var light in ArrLights) {
			IColorable colorableLight = new ColorableLight(light);
			if (colorableLight != null)	colorElements.Add(colorableLight);
		}

		foreach (var tilemap in ArrTileMaps) {
			IColorable colorableTilemap = new ColorableTilemap(tilemap);
			if (colorableTilemap != null)	colorElements.Add(colorableTilemap);
		}

		// Debug.Log("Sprites Cargados a color ="+colorElements.Count);
		main.ArrColors = colorElements;
		Destroy(gameObject);
	}
}