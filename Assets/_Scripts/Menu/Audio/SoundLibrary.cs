using UnityEngine;

[System.Serializable]
public struct SoundEffect
{
    public string groupID;
    public AudioClip[] clips;
}

public class SoundLibrary : MonoBehaviour
{
    public SoundEffect[] soundEffects;

    public AudioClip GetClipFromName(string name)
    {
        foreach (var soundEffect in soundEffects)
        {
            if (soundEffect.groupID == name)
            {
                if (soundEffect.clips.Length > 0)
                {
                    int randomIndex = Random.Range(0, soundEffect.clips.Length);
                    return soundEffect.clips[randomIndex];
                }
            }
        }
        return null;
    }
}