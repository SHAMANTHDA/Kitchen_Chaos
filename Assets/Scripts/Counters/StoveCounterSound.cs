using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterSound : MonoBehaviour
{
    [SerializeField] private StoveCounter stovecounter;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        stovecounter.OnStateChanged += Stovecounter_OnStateChanged;
    }

    private void Stovecounter_OnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e)
    {
        bool playsound = e.state == StoveCounter.State.Frying || e.state == StoveCounter.State.Fried;
        if (playsound)
        {
            audioSource.Play();
        }
        else
        {
            audioSource.Pause();
        }
    }
}
