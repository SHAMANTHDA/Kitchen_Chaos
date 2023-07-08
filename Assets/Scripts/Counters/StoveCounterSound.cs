using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterSound : MonoBehaviour
{
    [SerializeField] private StoveCounter stovecounter;
    private AudioSource audioSource;
    private float warningSoundTimer;
    private bool playWarningSound;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        stovecounter.OnStateChanged += Stovecounter_OnStateChanged;
        stovecounter.OnProgressChanged += Stovecounter_OnProgressChanged;
    }

    private void Stovecounter_OnProgressChanged(object sender, IHasProgress.OnProgreessChangedEventArgs e)
    {
        float burnShowProgressAmount = 0.5f;
        playWarningSound = stovecounter.IsFried() && e.progressNormalized >= burnShowProgressAmount;
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

    private void Update()
    {
        if (playWarningSound)
        {
            warningSoundTimer -= Time.deltaTime;
            if (warningSoundTimer <= 0f)
            {
                float warningSoundTimerMax = .2f;
                warningSoundTimer = warningSoundTimerMax;

                SoundManager.Instance.PlayWarningSound(stovecounter.transform.position);
            }
        }
    }
}
