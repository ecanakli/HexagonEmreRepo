using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioSource _audioS;

    [Header ("GAME SOUNDS")]
    [SerializeField] AudioClip _diamondSound;
    [SerializeField] AudioClip _clickSound;
    [SerializeField] AudioClip _rotationSound;
    [SerializeField] AudioClip _clusterSound;
    [SerializeField] AudioClip _bombTimerSound;
    [SerializeField] AudioClip _bombRemoveSound;
    [SerializeField] AudioClip _bombExplodeSound;

    public static SoundManager _instance;

    //Singleton
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    private void Start()
    {
        _audioS = gameObject.GetComponent<AudioSource>();
    }

    public void ReplacementS()
    {
        _audioS.PlayOneShot(_diamondSound);
    }

    public void BombClickS()
    {
        _audioS.PlayOneShot(_clickSound);
    }

    public void BombRotationS()
    {
        _audioS.PlayOneShot(_rotationSound);
    }

    public void ClusterS()
    {
        _audioS.PlayOneShot(_clusterSound);
    }

    public void BombTimerS()
    {
        _audioS.PlayOneShot(_bombTimerSound);
    }

    public void BombRemoveS()
    {
        _audioS.PlayOneShot(_bombRemoveSound);
    }

    public void BombExplodeS()
    {
        _audioS.PlayOneShot(_bombExplodeSound);
    }
}
