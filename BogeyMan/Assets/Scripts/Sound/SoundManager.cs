using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance = null;
    [SerializeField] internal List<AudioSource> audioSources;
    [SerializeField] internal List<AudioClip> audioClips;


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
    
    public void PlaySound(int index)
    {
        audioSources[index].PlayOneShot(audioClips[index]);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SoundManager))]
public class SoundManagerEditor : Editor
{
    private SoundManager soundManager;
    private SerializedProperty audioSources;
    private SerializedProperty audioClips;

    private void OnEnable()
    {
        soundManager = (SoundManager) target;
        audioSources = serializedObject.FindProperty("audioSources");
        audioClips = serializedObject.FindProperty("audioClips");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(audioSources, true);
        EditorGUILayout.PropertyField(audioClips, true);
        serializedObject.ApplyModifiedProperties();
        
        if (GUILayout.Button("Create AudioSources"))
        {
            if (soundManager.audioClips.Count > 0)
            {
                for (int i = 0; i < soundManager.audioClips.Count; i++)
                {
                    soundManager.audioSources.Add(soundManager.gameObject.AddComponent<AudioSource>());
                    soundManager.audioSources[i].clip = soundManager.audioClips[i];
                }
            }
        }
    }
}
#endif
