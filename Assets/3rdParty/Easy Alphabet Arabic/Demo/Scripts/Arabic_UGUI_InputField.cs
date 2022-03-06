using UnityEngine;
using UnityEngine.UI;
using EasyAlphabetArabic;

// This class provides and example to correct InputField text.
// all operations are performed when the user finishes typing and clicking or touching other place.
// we first copy the entered text and after that correct the text and assign it back.
// we use the OnEndEdit() callback since we can't correct from OnValueChanged() which will call 
// GetComponent<>() several times per frame resulting in an immediate stack overflow i will try to
// figure out a solution for it and hopefuly will be added in the next version
public class Arabic_UGUI_InputField : MonoBehaviour {

    private string text = "";

	public void CopyText()
    {
        text = GetComponent< InputField>().text;
    }

    public void CorrectText()
    {
        GetComponent< InputField>().text = EasyArabicCore.CorrectString(text, 1);
    }
}
