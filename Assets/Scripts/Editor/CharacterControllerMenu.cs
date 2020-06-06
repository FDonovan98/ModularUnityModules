using UnityEngine;
using UnityEditor;
using System.IO;

public class CharacterController : EditorWindow
{
    string[] activeCommands;

    [MenuItem("Window/Character Controller")]
    public static void ShowWindow()
    {
        GetWindow<CharacterController>("Character Controller");
    }

    void Awake()
    {
        activeCommands = Directory.GetFiles("Assets/Scripts/Commands/Agents/Active", "*.cs");

        for (int i = 0; i < activeCommands.Length; i++)
        {
            activeCommands[i] = Path.GetFileNameWithoutExtension(activeCommands[i]);
        }

        foreach (string element in activeCommands)
        {
            Debug.Log(element);
        }
    }
}
