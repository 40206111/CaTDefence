using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class LevelEditorWindow : EditorWindow
{
    static string Path = Application.streamingAssetsPath + "/LevelData.json";
    static AllLevels Levels;

    [MenuItem("Cat Defense/Level Editor")]
    static void Init()
    {
        //LevelEditorWindow window = (LevelEditorWindow)EditorWindow.GetWindow(typeof(LevelEditorWindow));
        LevelEditorWindow window = EditorWindow.GetWindow<LevelEditorWindow>(typeof(SceneView));

        window.Show();
    }

    void LoadLevels()
    {
        if (File.Exists(Path))
        {
            Debug.Log($"Loading from: {Path}");

            var levelData = File.ReadAllText(Path);

            Levels = JsonUtility.FromJson<AllLevels>(levelData);

            if (Levels == null)
            {
                Levels = new AllLevels();
            }
        }
        else
        {
            Levels = new AllLevels();
        }

        Levels.Init();
    }

    private void OnGUI()
    {
        if (Levels == null || Levels.Levels == null)
        {
            LoadLevels();
        }

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Save"))
        {
            SaveLevels();
        }

        if (GUILayout.Button("Reload"))
        {
            LoadLevels();
        }

        if (GUILayout.Button("New Level"))
        {
            NewLevel();
        }

        GUILayout.EndHorizontal();

        foreach (var level in Levels.Levels)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(level.width.ToString() + "," + level.height.ToString()) ;
            GUILayout.EndHorizontal();
        }
    }
    static void NewLevel()
    {
        Levels.AddLevel();
    }

    static void SaveLevels()
    {
        Debug.Log($"saving to: {Path}");

        if (!File.Exists(Path))
        {
            Debug.Log("creating path");
            Directory.CreateDirectory(Application.streamingAssetsPath);
        }
        File.WriteAllText(Path, JsonUtility.ToJson(Levels));
        Debug.Log("saved");
    }

}
