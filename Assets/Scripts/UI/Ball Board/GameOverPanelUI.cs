using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanelUI : MonoBehaviour
{
    [SerializeField] private Button startMenuButton;
    [SerializeField] private Button quitGameButton;

    private void Awake()
    {
        startMenuButton.onClick.AddListener(FindObjectOfType<SceneLoader>().LoadStartMenu);
        quitGameButton.onClick.AddListener(FindObjectOfType<SceneLoader>().ExitGame);
    }
}
