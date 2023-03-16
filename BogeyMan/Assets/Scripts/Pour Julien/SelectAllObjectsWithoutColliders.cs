using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class SelectAllObjectsWithoutColliders : MonoBehaviour
{
    public static SelectAllObjectsWithoutColliders Instance = null;
    [SerializeField] internal List<GameObject> objectsWithoutColliders = new List<GameObject>();
    [SerializeField] internal List<GameObject> objectsWithoutObstacleNavMesh = new List<GameObject>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

[CustomEditor(typeof(SelectAllObjectsWithoutColliders))]
public class SelectAllObjectsWithoutCollidersEditor : Editor
{
    private SelectAllObjectsWithoutColliders selectAllObjectsWithoutColliders;
    private SerializedProperty objectsWithoutColliders;
    private SerializedProperty objectsWithoutObstacleNavMesh;

    private void OnEnable()
    {
        selectAllObjectsWithoutColliders = (SelectAllObjectsWithoutColliders) target;
        objectsWithoutColliders = serializedObject.FindProperty("objectsWithoutColliders");
        objectsWithoutObstacleNavMesh = serializedObject.FindProperty("objectsWithoutObstacleNavMesh");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(objectsWithoutColliders, true);
        EditorGUILayout.PropertyField(objectsWithoutObstacleNavMesh, true);
        serializedObject.ApplyModifiedProperties();
        
        if (GUILayout.Button("Select All Objects Without Colliders"))
        {
            selectAllObjectsWithoutColliders.objectsWithoutColliders.Clear();
            foreach (GameObject gameObject in FindObjectsOfType<GameObject>())
            {
                if (gameObject.GetComponent<NavMeshObstacle>() == null && gameObject.transform.parent != null && 
                    gameObject.transform.parent.GetComponent<NavMeshObstacle>() == null
                    && gameObject.gameObject.GetComponent<Player>() == null
                    && gameObject.gameObject.GetComponent<NavMeshAgent>() == null
                    && gameObject.GetComponent<Light>() == null)
                {
                    selectAllObjectsWithoutColliders.objectsWithoutColliders.Add(gameObject);
                }
            }
            
            Selection.objects = selectAllObjectsWithoutColliders.objectsWithoutColliders.ToArray();
        }
        
        if (GUILayout.Button("Deselect All Objects Without Colliders"))
        {
            selectAllObjectsWithoutColliders.objectsWithoutColliders.Clear();
            
            Selection.objects = selectAllObjectsWithoutColliders.objectsWithoutColliders.ToArray();
        }
        
        if (GUILayout.Button("Select All Objects Without Obstacle NavMesh"))
        {
            selectAllObjectsWithoutColliders.objectsWithoutObstacleNavMesh.Clear();
            foreach (GameObject gameObject in FindObjectsOfType<GameObject>())
            {
                if (gameObject.GetComponent<NavMeshObstacle>() == null && gameObject.transform.parent != null && 
                    gameObject.transform.parent.GetComponent<NavMeshObstacle>() == null
                    && gameObject.gameObject.GetComponent<Player>() == null
                    && gameObject.gameObject.GetComponent<NavMeshAgent>() == null
                    && gameObject.GetComponent<Light>() == null)
                {
                    selectAllObjectsWithoutColliders.objectsWithoutObstacleNavMesh.Add(gameObject);
                }
            }
            
            Selection.objects = selectAllObjectsWithoutColliders.objectsWithoutObstacleNavMesh.ToArray();
        }
        
        if (GUILayout.Button("Deselect All Objects Without Obstacle NavMesh"))
        {
            selectAllObjectsWithoutColliders.objectsWithoutObstacleNavMesh.Clear();
            
            Selection.objects = selectAllObjectsWithoutColliders.objectsWithoutObstacleNavMesh.ToArray();
        }
    }
}