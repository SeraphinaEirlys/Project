using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private SoundLibrary soundLibrary;
    [SerializeField] private AudioSource soundEffect2DSource;

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

    public void PlaySound3D(AudioClip clip, Vector3 position)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, position);
        }
    }

    public void PlaySound3D(string soundName, Vector3 position)
    {
        AudioClip clip = soundLibrary.GetClipFromName(soundName);
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, position);
        }
    }

    public void PlaySound2D(string soundName)
    {
        AudioClip clip = soundLibrary.GetClipFromName(soundName);
        if (clip != null)
        {
            soundEffect2DSource.PlayOneShot(clip);
        }
    }
}