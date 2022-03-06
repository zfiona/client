using UnityEngine;
using EasyAlphabetArabic;


public class Arabic_TextMesh : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<TextMesh>().text = EasyArabicCore.CorrectString(GetComponent<TextMesh>().text);
	}
	
}
