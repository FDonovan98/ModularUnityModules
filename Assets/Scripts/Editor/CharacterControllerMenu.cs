using UnityEngine;
using UnityEditor;
using System.IO;

public class CharacterController : EditorWindow
{
    string[] activeCommands;
    string[] passiveCommands;
    GUIStyle customStyle;

    public AgentInputHandler agent;

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
        customStyle = new GUIStyle(GUI.skin.button);
        customStyle.active.background = Texture2D.redTexture;

        agent = (AgentInputHandler)EditorGUILayout.ObjectField(agent, typeof(AgentInputHandler), true);
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        ArrayToLabelList(activeCommands, customStyle);
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        ArrayToLabelList(passiveCommands, customStyle);
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }

    void ArrayToLabelList(string[] array, string labelStyle)
    {
        foreach (string element in array)
        {
            GUILayout.Button(element, labelStyle);
        }
    }
    
    void ArrayToLabelList(string[] array, GUIStyle style)
    {
        foreach (string element in array)
        {
            if (GUILayout.Button(element, style))
            {
                if (array == activeCommands)
                {
                    agent.activeCommands = AddElementToArray<ActiveCommandObject>(agent.activeCommands, (ActiveCommandObject)ScriptableObject.CreateInstance(element));
                }
                else
                {
                    agent.passiveCommands = AddElementToArray<PassiveCommandObject>(agent.passiveCommands, (PassiveCommandObject)ScriptableObject.CreateInstance(element));
                }
            }
        }
    }

    static T[] AddElementToArray<T>(T[] arrayToAddTo, T elementToAdd)
    {
        T[] temp = arrayToAddTo;
        arrayToAddTo = new T[arrayToAddTo.Length + 1];
        int i;
        for (i = 0; i < temp.Length; i++)
        {
            arrayToAddTo[i] = temp[i];
        }
        arrayToAddTo[i] = elementToAdd;
        return arrayToAddTo;
    }
}