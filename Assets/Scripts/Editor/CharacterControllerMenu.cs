using UnityEngine;
using UnityEditor;
using System.IO;

public class CharacterController : EditorWindow
{
    string[] activeCommands;
    bool[] activeCommandsExpanded;
    string[] passiveCommands;
    bool[] passiveCommandsExpanded;

    string[] foundAssets;

    public AgentInputHandler agent;

    [MenuItem("Window/Character Controller")]
    public static void ShowWindow()
    {
        GetWindow<CharacterController>("Character Controller");
    }

    void Awake()
    {
        activeCommands = GetFileNamesFromDirectory("Assets/Scripts/Commands/Agents/Active", "*.cs");
        activeCommandsExpanded = new bool[activeCommands.Length];
        
        passiveCommands = GetFileNamesFromDirectory("Assets/Scripts/Commands/Agents/Passive", "*.cs");
        passiveCommandsExpanded = new bool[passiveCommands.Length];
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
        agent = (AgentInputHandler)EditorGUILayout.ObjectField(agent, typeof(AgentInputHandler), true);

        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        ArrayToExpandableButtonList(activeCommands, activeCommandsExpanded);
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        ArrayToExpandableButtonList(passiveCommands, passiveCommandsExpanded);
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        // Debug.Log(ScriptableObject.CreateInstance(activeCommands[0]).GetType().ToString());
    }

    // Perfomance can be improved by searching for existing assets once in Awake.
    // A class would be needed to organise this containing command name, if it's expanded, and existing objects of that type.
    void ArrayToExpandableButtonList(string[] array, bool[] foldoutTracker)
    {
        for (int i = 0; i < array.Length; i++)
        {
            foldoutTracker[i] = EditorGUILayout.Foldout(foldoutTracker[i], array[i]);
            
            if (foldoutTracker[i])
            {
                string commandType = ScriptableObject.CreateInstance(array[i]).GetType().ToString();

                string[] foundAssets = AssetDatabase.FindAssets("t: " + commandType);

                for (int j = 0; j < foundAssets.Length; j++)
                { 
                    foundAssets[j] = Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(foundAssets[j]));
                }

                if (array == activeCommands)
                {
                    ArrayToButtonList(array, foundAssets, commandType);
                }
            }
        }
    }
    
    void ArrayToButtonList(string[] parentArray, string[] array, string commandType)
    {
        foreach (string element in array)
        {
            if (GUILayout.Button(element))
            {
                if (parentArray == activeCommands)
                {
                    AddUniqueElementTypeToArray<ActiveCommandObject>(ref agent.activeCommands, (ActiveCommandObject)ScriptableObject.CreateInstance(commandType));
                }
                else
                {
                    AddUniqueElementTypeToArray<PassiveCommandObject>(ref agent.passiveCommands, (PassiveCommandObject)ScriptableObject.CreateInstance(commandType));
                }
            }
        }
    }

    static void AddElementToArray<T>(ref T[] arrayToAddTo, T elementToAdd)
    {
        T[] temp = arrayToAddTo;
        arrayToAddTo = new T[arrayToAddTo.Length + 1];

        int i;
        for (i = 0; i < temp.Length; i++)
        {
            arrayToAddTo[i] = temp[i];
        }

        arrayToAddTo[i] = elementToAdd;
    }

    static void AddUniqueElementTypeToArray<T>(ref T[] arrayToAddTo, T elementToAdd)
    {
        foreach (T element in arrayToAddTo)
        {
            if (element.GetType() == elementToAdd.GetType())
            {
                Debug.Log("Array already has an element of this type");
                return;
            }
        }

        AddElementToArray<T>(ref arrayToAddTo, elementToAdd);
    }
}