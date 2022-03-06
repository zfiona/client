using UnityEngine;
using System.Collections;
using EasyAlphabetArabic;

public class oldGUI : MonoBehaviour
{
	public string textFieldString;
    
    void OnGUI()
	{
		// Make a background box
		GUI.Box (new Rect (10, 10, 100, 90), EasyArabicCore.CorrectString ("القائمة الرئيسية"));

		// Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
		if (GUI.Button (new Rect (20, 40, 80, 20), EasyArabicCore.CorrectString ("ابدأ"))){}

		// Make the second button.
		if (GUI.Button (new Rect (20, 70, 80, 20), EasyArabicCore.CorrectString ("الخروج"))){}

		GUI.TextField (new Rect (150, 10, 180, 70), EasyArabicCore.CorrectString (textFieldString));
    
	}
}