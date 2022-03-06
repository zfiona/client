
/***********************************************************************************************
 * Easy Alphabet Arabic v3.0
 * Fayyad Sufyan (c) 2017
 *
 * Description:
 * Editor Window of the tool.
 *
 ***********************************************************************************************/


using UnityEngine;
using UnityEditor;
using EasyAlphabetArabic;


class EasyArabicEditor : EditorWindow
{
    private enum NumeralsEnum { Latin = 0, Arabic = 1, Persian = 2 };

    [SerializeField] string inputTextField      = string.Empty;
    [SerializeField] NumeralsEnum numsFormat    = NumeralsEnum.Latin;
    
 
    [MenuItem("Window/Easy Alphabet Arabic")]
    
    static void Init()
    {
        EasyArabicEditor mWindow = (EasyArabicEditor)GetWindow(typeof(EasyArabicEditor), false, "Easy Arabic");
        mWindow.Show();
    }

    
    void OnGUI()
    {
        EditorGUILayout.BeginVertical();

        EditorGUILayout.LabelField("Easely Correct Arabic Text");
        EditorGUILayout.Separator();
        
        inputTextField = EditorGUILayout.TextField("Input Arabic Text", inputTextField);
        EditorGUILayout.Separator();
        
        numsFormat = (NumeralsEnum)EditorGUILayout.EnumPopup("Numbers Format: ", numsFormat);
        EditorGUILayout.Separator();

        EditorGUILayout.TextField("Corrected Arabic Text", EasyArabicCore.CorrectString(inputTextField, (int)numsFormat));

        EditorGUILayout.EndVertical();

        // window undo/redo
        if (focusedWindow != null) Undo.RecordObject(focusedWindow, "EasyArabic text");

    }
}
