using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerSoundManager : MonoBehaviour
{
    [Serializable]
    public struct Sound
    {
        [Range(0, 3)] public float volume;
        public AudioClip clip;
    }

    public static PlayerSoundManager currentManager { get; private set; }

    public PlayerMovementController playerMovementController;
    public CheckTerrainTextures checkTerrainTextures;

    [Header("Dialogue Sounds")]
    public int numOfSkips = 1;
    public Sound dialogueTypeSound;

    [Header("Footstep Sounds")]
    public Sound[] grassFootstepSounds;
    public Sound[] sandFootstepSounds;
    public Sound[] stoneFootstepSounds;

    [Header("Other Movement Sounds")]
    public Sound[] jumpSounds;
    public Sound landSound;

    AudioSource audioSource;
    int typeSkipCounter = 0;

    private void Awake()
    {
        typeSkipCounter = 0;
        currentManager = this;
        audioSource = GetComponent<AudioSource>();
        playerMovementController.onJumped += PlayJumpSound;
        playerMovementController.onLanded += PlayLandSound;
    }

    private void OnDestroy()
    {
        if (currentManager == this)
            currentManager = null;

        playerMovementController.onJumped -= PlayJumpSound;
        playerMovementController.onLanded -= PlayLandSound;
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void PlayFootStep()
    {
        var result = checkTerrainTextures.CalculateTerrainValues();
        if (result.Length < 0) return;

        if (result[6] > 0)                                                                     //If terrain has some sand texture...
            PlaySoundCustomVol(PickRandomClip(sandFootstepSounds), result[6]);
         if (result[2] > 0)                                                                //If terrain has some stone texture...
            PlaySoundCustomVol(PickRandomClip(stoneFootstepSounds), result[2]);
         if (result[0] > 0 || result[1] > 0 || result[4] > 0 || result[5] > 0)             //If terrain has some grass/dirt/flower/mix texture play grass footstep
        {
            var maxValue = result[0] > result[4] ? result[0] : result[4];
            maxValue = maxValue < result[1] ? result[1] : maxValue;
            maxValue = maxValue < result[5] ? result[5] : maxValue;
            PlaySoundCustomVol(PickRandomClip(grassFootstepSounds), maxValue);
        }


    }

    public void PlayDialogueTypeSound()
    {
        if (typeSkipCounter == 0)
            audioSource.PlayOneShot(dialogueTypeSound.clip, dialogueTypeSound.volume);

        typeSkipCounter++;
        if (typeSkipCounter > numOfSkips)
            typeSkipCounter = 0;
    }

    private void PlayJumpSound()
    {
        PlaySound(PickRandomClip(jumpSounds));

    }

    private void PlayLandSound()
    {
        PlaySound(landSound);
    }


    private void PlaySoundCustomVol(Sound sound, float volumeFactor) => audioSource.PlayOneShot(sound.clip, sound.volume * volumeFactor);
    private void PlaySound(Sound sound) => audioSource.PlayOneShot(sound.clip, sound.volume);
    private Sound PickRandomClip(Sound[] source) => source[UnityEngine.Random.Range(0, source.Length)];
}
