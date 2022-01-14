using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

public class PhysicsSimulationEditor : EditorWindow
{

    public List<GameObject> objectsToSimulate = new List<GameObject>(); 

    private static PhysicsSimulationData data;

    private bool simulate;
    private int physicsTime;
    private bool randomForce;
    private int force;
    private bool notSetup;

    private SerializedObject serializedObject;
    private SerializedProperty sceneObjProp;

    private Vector2 scrollPosition = Vector2.zero;

    [InitializeOnLoadMethod]
    private static void OnLoad()
    {
        if (!data)
        {
            data = AssetDatabase.LoadAssetAtPath<PhysicsSimulationData>("Assets/CustomEditor/PhysicsSimulationData.asset");

            if(data) return;

            data = CreateInstance<PhysicsSimulationData>();

            AssetDatabase.CreateAsset(data, "Assets/CustomEditor/PhysicsSimulationData.asset");
            AssetDatabase.Refresh();
        }
    }

     [MenuItem("Custom/PhysicsSimulationEditor")]
    private static void Init()
    {
        var window = (PhysicsSimulationEditor)EditorWindow.GetWindow(typeof(PhysicsSimulationEditor));
        window.minSize = new Vector2(300, 280);
        window.Show();
    }

    private void OnEnable() {

        serializedObject = new SerializedObject(this);
        sceneObjProp = serializedObject.FindProperty("objectsToSimulate");


        objectsToSimulate.Clear();
        objectsToSimulate.AddRange(data.objects);
        

        physicsTime = data.physicsTime;

        Repaint();
        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();

        EditorSceneManager.sceneOpened   -= SceneClosing;
        EditorSceneManager.sceneOpened   += SceneClosing;
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }
    private void OnGUI()
    {
        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();

        EditorGUILayout.Space(10);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, false, false);

            EditorGUILayout.PropertyField(sceneObjProp,true);

            EditorGUILayout.Space(5);
            EditorGUILayout.BeginHorizontal(GUILayout.Width (290));
                GUILayout.Label ("Physics Time", EditorStyles.boldLabel);
                physicsTime = EditorGUILayout.IntSlider(physicsTime, 1, 20);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(1);
            EditorGUILayout.BeginHorizontal(GUILayout.Width (290));
                GUILayout.Label ("Simulate With Random Force", EditorStyles.boldLabel);
                randomForce = EditorGUILayout.Toggle(randomForce);
                GUILayout.Label ("Force", EditorStyles.boldLabel);
                force = EditorGUILayout.IntField(force);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);
            if (GUILayout.Button("Reset"))
            {
                data.ResetState();
            }
            EditorGUILayout.Space(1);
            if (GUILayout.Button("Clear"))
            {
                ClearData();
            }
            EditorGUILayout.Space(1);

            if (GUILayout.Button("Simulate Physics"))
            {
                if(objectsToSimulate.Count <= 0){
                    notSetup = true;
                }else{
                    notSetup = false;
                    SimSetup();
                    data.SimulatePhysics();
                }

            }

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();

            EditorGUILayout.Space(1);
            if (GUILayout.Button("Close Window"))
            {
                ClearData();
                this.Close();
            }

            if(notSetup){
                EditorGUILayout.HelpBox("Add the objects you want to simulate!", MessageType.Warning);
            }

        EditorGUILayout.EndScrollView();


    }

    private void Update() {
        data.UpdatePhysics();
    }

    private void SimSetup()
    {
        data.objects.Clear();
        data.objects.AddRange(objectsToSimulate);

        data.Init();

        data.physicsTime = physicsTime;
        data.force = force;
        data.randomForce = randomForce;
    }

    private void SceneClosing(UnityEngine.SceneManagement.Scene scene, OpenSceneMode mode)
    {
        ClearData();
        Debug.Log("Scene Changed");
    }

    private void ClearData()
    {
        objectsToSimulate.Clear();
        data.Clear();
    }
}