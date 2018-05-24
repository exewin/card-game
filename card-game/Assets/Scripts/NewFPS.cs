using UnityEngine;
using System.Collections;
 
public class NewFPS : MonoBehaviour
{
	float deltaTime = 0.0f;
 
	void Update()
	{
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
	}
 
	void OnGUI()
	{
		GUIStyle style = new GUIStyle();
 
		Rect rect = new Rect(4,Screen.height-19,56,28);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = Screen.height * 2 / 100;
		style.normal.textColor = new Color (1.0f, 0.0f, 0.1f, 1.0f);
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
		GUI.Label(rect, text, style);
	}
}