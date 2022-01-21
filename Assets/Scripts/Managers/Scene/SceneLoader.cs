using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [field: SerializeField] public int StartMenuIndex { get; private set; }

    private void Awake()
    {
        if (FindObjectsOfType<SceneLoader>().Length > 1)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(gameObject);
    }

    public void LoadStartMenu()
    {
        SceneManager.LoadScene(StartMenuIndex);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }
}
