using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Scenes;
using UnityEngine.SceneManagement;

public class SceneStorage : MonoBehaviour
{

    public static SceneStorage Instance;
    private SceneLevels sceneLevels = SceneLevels.Empty;

    [SerializeField]
    public LevelParams[] LevelParameters;

    [SerializeField]
    public LevelParams MenuLevel;

    [SerializeField]
    public LevelParams StartLevel;

    [SerializeField]
    public LevelParams EndLevel;

    private Dictionary<int, int> LevelDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(this);

        SetupDictionary();
    }

    public void LoadLevel(int level)
    {
        //Debug.Log("Loading scene Level : " + level);
        SceneManager.LoadSceneAsync(LevelDictionary[level],LoadSceneMode.Additive);
    }

    public void UnloadLevel(int level)
    {
       //Debug.Log("Scene to unload : " + level);
        if (LevelDictionary.ContainsKey(level))
        {
            SceneManager.UnloadSceneAsync(LevelDictionary[level]);
        }
        else
        {
            Debug.LogWarning("This Scene Doesn't exist!");
        }
    }
    public void LoadStartLevel()
    {
        SceneManager.LoadSceneAsync(StartLevel.BuildLevel, LoadSceneMode.Additive);
    }
    public void UnLoadStartLevel()
    {
        SceneManager.UnloadSceneAsync(StartLevel.BuildLevel);
    }

    public void LoadEndLevel()
    {
        SceneManager.LoadSceneAsync(EndLevel.BuildLevel, LoadSceneMode.Additive);
    }
    public void UnLoadEndLevel()
    {
        SceneManager.UnloadSceneAsync(EndLevel.BuildLevel);
    }
    public void LoadMenu()
    {
        SceneManager.LoadSceneAsync(MenuLevel.BuildLevel, LoadSceneMode.Additive);
    }
    public void UnLoadMenu()
    {
        SceneManager.UnloadSceneAsync(MenuLevel.BuildLevel);

    }
    
    private void SetupDictionary()
    {
        LevelDictionary = new Dictionary<int, int>();

        foreach (var item in LevelParameters)
        {
            LevelDictionary.Add(((int)item.Level), item.BuildLevel);
        }
    }

    public int SceneLength()
    {
        return LevelDictionary.Count;
    }

}


public enum SceneLevels
{
    Empty = 0,
    Level_1 = 1,
    Level_2 = 2,
    Level_3 = 3,
    Level_4 = 4
}

[System.Serializable]
public struct LevelParams
{
    public SceneLevels Level;
    public int BuildLevel;
}
