using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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
        if (GUILayout.Button("Remove all AudioSources"))
        {
            if (soundManager.audioClips.Count < 1)
            {
                for (int i = 0; i < soundManager.audioSources.Count; i++)
                {
                    DestroyImmediate(soundManager.audioSources[i]);
                    soundManager.audioSources.RemoveAt(i);
                }
            }
        }        
        // if (soundManager.audioClips.Count < soundManager.audioSources.Count)
        // {
        //     for (int i = 0; i < soundManager.audioSources.Count; i++)
        //     {
        //         //check if there is i in audioClips list
        //         if (soundManager.audioClips.Count < i)
        //         {
        //             DestroyImmediate(soundManager.audioSources[i]);
        //             soundManager.audioSources.RemoveAt(i);
        //         }
        //         else
        //         {
        //             soundManager.audioSources[i].clip = soundManager.audioClips[i];
        //         }
        //     }
        // }
    }
}
