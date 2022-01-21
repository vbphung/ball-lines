using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class BallEditor : EditorWindow
{
    private static Ball editedBall;
    private ListView ballList;
    private Ball[] balls;

    [MenuItem("Window/Editors/Ball")]
    private static void ShowWindow()
    {
        var window = GetWindow<BallEditor>();
        window.titleContent = new GUIContent("Ball Editor");
        window.Show();
    }

    [OnOpenAssetAttribute(1)]
    private static bool OnOpenAsset(int instanceID, int line)
    {
        Ball ballInstance = EditorUtility.InstanceIDToObject(instanceID) as Ball;
        if (ballInstance != null)
        {
            editedBall = ballInstance;
            ShowWindow();
            return true;
        }
        return false;
    }

    private void OnEnable()
    {
        LoadTreeAsset();
        LoadAllBalls();

        SetupBallList();
        SetupCreateBall();
        SetupRemoveBall();

        ShowBallOnEnable();
    }

    #region Setup
    private void SetupBallList()
    {
        ballList = rootVisualElement.Query<ListView>("ball-list").First();
        ballList.makeItem = () => new Label();
        ballList.bindItem = (element, i) => (element as Label).text = balls[i].name;
        ballList.itemsSource = balls;
        ballList.itemHeight = 16;
        ballList.selectionType = SelectionType.Single;
        ballList.onSelectionChange += ShowBallInfo;
    }

    private void SetupCreateBall()
    {
        TextField newBallName = rootVisualElement.Query<TextField>("new-ball-name").First();

        var createBallButton = rootVisualElement.Query<Button>("ball-create-button").First();
        createBallButton.text = "New Ball";
        createBallButton.clicked += () =>
        {
            CreateBall(newBallName.value);
            newBallName.value = "";
        };
    }

    private void SetupRemoveBall()
    {
        var removeBallButton = rootVisualElement.Query<Button>("ball-remove-button").First();
        removeBallButton.text = "Remove Ball";
        removeBallButton.clicked += RemoveBall;
    }
    #endregion

    #region Update
    private void CreateBall(string newBallName)
    {
        if (String.IsNullOrWhiteSpace(newBallName))
            return;

        Ball newBall = ScriptableObject.CreateInstance<Ball>();
        AssetDatabase.CreateAsset(newBall, "Assets/Data/Balls/" + newBallName + ".asset");
        AssetDatabase.SaveAssets();
        ReloadAllBalls();

        editedBall = newBall;
        ballList.selectedIndex = Array.FindIndex(balls, ball => ball.Equals(editedBall));
        ShowBallInfo(new Ball[] { balls[ballList.selectedIndex] });
    }

    private void RemoveBall()
    {
        if (editedBall == null)
            return;

        string ballPath = "Assets/Data/Balls/" + editedBall.name + ".asset";
        AssetDatabase.DeleteAsset(ballPath);
        ReloadAllBalls();

        if (balls.Length > 0)
        {
            ballList.selectedIndex = 0;
            ShowBallInfo(new Ball[] { balls[ballList.selectedIndex] });
        }
        else
            ShowBallInfo(new Ball[0]);
    }
    #endregion

    #region Show
    private void ShowBallInfo(IEnumerable<object> ballObjects)
    {
        foreach (var ballObject in ballObjects)
        {
            editedBall = ballObject as Ball;
            var serializedBall = new SerializedObject(editedBall);
            SerializedProperty ballProperty = serializedBall.GetIterator();
            ballProperty.Next(true);

            var propertiesRoot = rootVisualElement.Query<ScrollView>("ball-properties").First();
            propertiesRoot.Clear();

            while (ballProperty.NextVisible(false))
            {
                var propertyField = new PropertyField(ballProperty);

                propertyField.SetEnabled(ballProperty.name != "m_Script");
                propertyField.Bind(serializedBall);
                propertiesRoot.Add(propertyField);
            }
        }
    }

    private void ShowBallOnEnable()
    {
        if (editedBall != null)
        {
            ballList.selectedIndex = Array.FindIndex(balls, ball => ball.Equals(editedBall));
            ShowBallInfo(new Ball[] { balls[ballList.selectedIndex] });
            return;
        }

        if (balls.Length > 0)
        {
            ballList.selectedIndex = 0;
            ShowBallInfo(new Ball[] { balls[ballList.selectedIndex] });
        }
    }
    #endregion

    #region Load
    private VisualElement LoadTreeAsset()
    {
        rootVisualElement.Add(Resources.Load<VisualTreeAsset>("BallEditor").CloneTree());
        rootVisualElement.styleSheets.Add(Resources.Load<StyleSheet>("BallEditor_Style"));
        return rootVisualElement;
    }

    private void LoadAllBalls()
    {
        var guids = AssetDatabase.FindAssets("t:Ball");
        balls = new Ball[guids.Length];
        for (int i = 0; i < guids.Length; ++i)
            balls[i] = AssetDatabase.LoadAssetAtPath<Ball>(AssetDatabase.GUIDToAssetPath(guids[i]));
    }

    private void ReloadAllBalls()
    {
        LoadAllBalls();
        ballList.itemsSource = balls;
        ballList.Refresh();
    }
    #endregion
}
