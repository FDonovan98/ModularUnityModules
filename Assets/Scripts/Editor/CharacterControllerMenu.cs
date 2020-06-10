using UnityEngine;
using UnityEditor;
using System.IO;

public class CharacterController : EditorWindow
{
    string[] activeCommands;
    string[] passiveCommands;
    GUIStyle customStyle;

    [MenuItem("Window/Character Controller")]
    public static void ShowWindow()
    {
        GetWindow<CharacterController>("Character Controller");
    }

    void Awake()
    {
        activeCommands = GetFileNamesFromDirectory("Assets/Scripts/Commands/Agents/Active", "*.cs");
        
        passiveCommands = GetFileNamesFromDirectory("Assets/Scripts/Commands/Agents/Passive", "*.cs");
    }

    string[] GetFileNamesFromDirectory(string filePath, string extension)
    {
        string[] fileNames = Directory.GetFiles(filePath, extension);

        for (int i = 0; i < fileNames.Length; i++)
        {
            fileNames[i] = Path.GetFileNameWithoutExtension(fileNames[i]);
        }

        return fileNames;
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        ArrayToLabelList(activeCommands, "textField");
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        ArrayToLabelList(passiveCommands, "textField");
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }

    void ArrayToLabelList(string[] array, string labelStyle)
    {
        foreach (string element in array)
        {
            GUILayout.Label(element, labelStyle);
        }
    }
}