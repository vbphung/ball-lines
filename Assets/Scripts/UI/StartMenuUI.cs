using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuUI : MonoBehaviour
{
    [SerializeField] private Button levelButtonPrefab;
    [SerializeField] private Transform levelButtonContainer;
    [SerializeField] private Button quitGameButton;
    [SerializeField] private LoadingLevel[] loadingLevels;

    [System.Serializable]
    public struct LoadingLevel
    {
        public Sprite background;
        public int index;
    }

    private void Awake()
    {
        foreach (Transform child in levelButtonContainer)
            Destroy(child.gameObject);

        foreach (var level in loadingLevels)
        {
            var levelButton = Instantiate(levelButtonPrefab, levelButtonContainer);
            levelButton.GetComponentsInChildren<Image>()[1].sprite = level.background;
            levelButton.onClick.AddListener(delegate { FindObjectOfType<SceneLoader>().LoadScene(level.index); });
        }

        quitGameButton.onClick.AddListener(delegate { FindObjectOfType<SceneLoader>().ExitGame(); });
    }
}
