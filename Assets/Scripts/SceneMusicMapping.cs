using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneMusicMapping", menuName = "Custom/Scene Music Mapping")]
public class SceneMusicMapping : ScriptableObject
{
    [Serializable]
    public class SceneMusicPair
    {
        public string sceneName;
        public AudioClip musicClip;
    }

    public SceneMusicPair[] sceneMusicPairs;
}