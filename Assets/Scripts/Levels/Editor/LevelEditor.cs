using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class LevelEditor : EditorWindow
{
    private static LevelPack editedLevel;
    private ListView levelList;
    private LevelPack[] levels;

    [MenuItem("Window/Editors/Level")]
    private static void ShowWindow()
    {
        var window = GetWindow<LevelEditor>();
        window.titleContent = new GUIContent("Level Editor");
        window.Show();
    }

    [OnOpenAssetAttribute(1)]
    private static bool OnOpenAsset(int instanceID, int line)
    {
        LevelPack levelInstance = EditorUtility.InstanceIDToObject(instanceID) as LevelPack;
        if (levelInstance != null)
        {
            editedLevel = levelInstance;
            ShowWindow();
            return true;
        }
        return false;
    }

    private void OnEnable()
    {
        LoadTreeAsset();
        LoadAllLevels();

        SetupLevelList();
        SetupCreateLevel();
        SetupRemoveLevel();

        ShowLevelOnEnable();
    }

    #region Setup
    private void SetupLevelList()
    {
        levelList = rootVisualElement.Query<ListView>("level-list").First();
        levelList.makeItem = () => new Label();
        levelList.bindItem = (element, i) => (element as Label).text = levels[i].name;
        levelList.itemsSource = levels;
        levelList.itemHeight = 16;
        levelList.selectionType = SelectionType.Single;
        levelList.onSelectionChange += ShowLevelInfo;
    }

    private void SetupCreateLevel()
    {
        TextField newLevelName = rootVisualElement.Query<TextField>("new-level-name").First();

        var createLevelButton = rootVisualElement.Query<Button>("level-create-button").First();
        createLevelButton.text = "New Level";
        createLevelButton.clicked += () =>
        {
            CreateLevel(newLevelName.value);
            newLevelName.value = "";
        };
    }

    private void SetupRemoveLevel()
    {
        var removeLevelButton = rootVisualElement.Query<Button>("level-remove-button").First();
        removeLevelButton.text = "Remove Level";
        removeLevelButton.clicked += RemoveLevel;
    }
    #endregion

    #region Update
    private void CreateLevel(string newLevelName)
    {
        if (String.IsNullOrWhiteSpace(newLevelName))
            return;

        LevelPack newLevel = ScriptableObject.CreateInstance<LevelPack>();
        AssetDatabase.CreateAsset(newLevel, "Assets/Data/Levels/" + newLevelName + ".asset");
        AssetDatabase.SaveAssets();
        ReloadAllLevels();

        editedLevel = newLevel;
        levelList.selectedIndex = Array.FindIndex(levels, level => level.Equals(editedLevel));
        ShowLevelInfo(new LevelPack[] { levels[levelList.selectedIndex] });
    }

    private void RemoveLevel()
    {
        if (editedLevel == null)
            return;

        string levelPath = "Assets/Data/Levels/" + editedLevel.LevelName + ".asset";
        AssetDatabase.DeleteAsset(levelPath);
        ReloadAllLevels();

        if (levels.Length > 0)
        {
            levelList.selectedIndex = 0;
            ShowLevelInfo(new LevelPack[] { levels[levelList.selectedIndex] });
        }
        else
            ShowLevelInfo(new LevelPack[0]);
    }
    #endregion

    #region Show
    private void ShowLevelInfo(IEnumerable<object> levelObjects)
    {
        foreach (var levelObject in levelObjects)
        {
            editedLevel = levelObject as LevelPack;
            var serializedLevel = new SerializedObject(editedLevel);
            SerializedProperty levelProperty = serializedLevel.GetIterator();
            levelProperty.Next(true);

            var propertiesRoot = rootVisualElement.Query<ScrollView>("level-properties").First();
            propertiesRoot.Clear();

            while (levelProperty.NextVisible(false))
            {
                var propertyField = new PropertyField(levelProperty);

                propertyField.SetEnabled(levelProperty.name != "m_Script");
                propertyField.Bind(serializedLevel);
                propertiesRoot.Add(propertyField);

                if (levelProperty.name == "<BackgroundTexture>k__BackingField")
                    propertyField.RegisterCallback<ChangeEvent<UnityEngine.Object>>((changeEvent) => ShowLevelBackground(editedLevel.BackgroundTexture));
            }
        }
    }

    private void ShowLevelBackground(Texture2D levelBackground)
    {
        var icon = rootVisualElement.Query<Image>("level-background").First();
        icon.image = levelBackground;
    }

    private void ShowLevelOnEnable()
    {
        if (editedLevel != null)
        {
            levelList.selectedIndex = Array.FindIndex(levels, level => level.Equals(editedLevel));
            ShowLevelInfo(new LevelPack[] { levels[levelList.selectedIndex] });
            return;
        }

        if (levels.Length > 0)
        {
            levelList.selectedIndex = 0;
            ShowLevelInfo(new LevelPack[] { levels[levelList.selectedIndex] });
        }
    }
    #endregion

    #region Load
    private VisualElement LoadTreeAsset()
    {
        rootVisualElement.Add(Resources.Load<VisualTreeAsset>("LevelEditor").CloneTree());
        rootVisualElement.styleSheets.Add(Resources.Load<StyleSheet>("LevelEditor_Style"));
        return rootVisualElement;
    }

    private void LoadAllLevels()
    {
        var guids = AssetDatabase.FindAssets("t:LevelPack");
        levels = new LevelPack[guids.Length];
        for (int i = 0; i < guids.Length; ++i)
            levels[i] = AssetDatabase.LoadAssetAtPath<LevelPack>(AssetDatabase.GUIDToAssetPath(guids[i]));
    }

    private void ReloadAllLevels()
    {
        LoadAllLevels();
        levelList.itemsSource = levels;
        levelList.Refresh();
    }
    #endregion
}
