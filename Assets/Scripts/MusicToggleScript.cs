using UnityEngine;
using System.Collections;

public class MusicToggleScript : MonoBehaviour
{
	public Vector2 sizeFindScale;
	public float scaleX;
	public float scaleY;

	public Texture buttonTex;
	public Texture buttonTexFalse;

	public Vector2 buttonPosition;
	public Vector2 buttonSize;

	public GUIStyle buttonStyle;

	public bool musicOn = true;

	void Update()
	{
		if(Input.GetKeyDown("m"))
		{
			ToggleMusic();
		}
	}

	void OnGUI()
	{
		scaleX = Screen.width / sizeFindScale.x;
		scaleY = Screen.height / sizeFindScale.y;

		if(GameObject.FindGameObjectWithTag("GUIObject").GetComponent<DefenceGUIScript>().tDefenceOn == false && GameObject.FindGameObjectWithTag("SpawnObject").GetComponent<AlienSpawnScript>().createRoundGUIs == false)
		{
			Texture textureToUse = null;
			if(musicOn == true)
			{
				textureToUse = buttonTex;
			}
			else
			{
				textureToUse = buttonTexFalse;
			}
			GUI.Label(new Rect(buttonPosition.x * scaleX, buttonPosition.y * scaleY, buttonSize.x * scaleX, buttonSize.y * scaleY), textureToUse, buttonStyle);
		}
	}

	void ToggleMusic()
	{
		if(musicOn == true)
		{
			musicOn = false;
			audio.Pause();
		}
		else
		{
			musicOn = true;
			audio.Play();
		}
	}
}
