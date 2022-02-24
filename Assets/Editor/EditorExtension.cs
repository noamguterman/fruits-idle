using UnityEngine;

public class EditorExtension : MonoBehaviour
{
    [UnityEditor.MenuItem("Tools/Clear PlayerPrefs")]
    public static void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}