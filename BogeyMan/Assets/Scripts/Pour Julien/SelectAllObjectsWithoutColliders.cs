using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SelectAllObjectsWithoutColliders : MonoBehaviour
{
    public static SelectAllObjectsWithoutColliders Instance = null;
    [SerializeField] internal List<GameObject> objectsWithoutColliders = new List<GameObject>();
    
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

    private void OnEnable()
    {
        selectAllObjectsWithoutColliders = (SelectAllObjectsWithoutColliders) target;
        objectsWithoutColliders = serializedObject.FindProperty("objectsWithoutColliders");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(objectsWithoutColliders, true);
        serializedObject.ApplyModifiedProperties();
        
        if (GUILayout.Button("Select All Objects Without Colliders"))
        {
            selectAllObjectsWithoutColliders.objectsWithoutColliders.Clear();
            foreach (GameObject gameObject in FindObjectsOfType<GameObject>())
            {
                if (gameObject.GetComponent<Collider>() == null && gameObject.transform.parent != null && gameObject.transform.parent.GetComponent<Collider>() == null)
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
    }
}