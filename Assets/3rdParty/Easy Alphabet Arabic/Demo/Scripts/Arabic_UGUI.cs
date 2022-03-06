using UnityEngine;
using UnityEngine.UI;
using EasyAlphabetArabic;


public class Arabic_UGUI : MonoBehaviour {

    private Text textComponent = null;

	// Use this for initialization
	void Start () {
        textComponent = GetComponent<Text>();
        GetComponent<Text>().text = EasyArabicCore.CorrectWithLineWrapping( textComponent);
    }
    
}