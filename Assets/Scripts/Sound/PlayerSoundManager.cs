using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
    public AudioSource[] externalAudioSources;

    [Header("Fading AudioSources")]
    public float fadeDuration=2;
    public Ease fadeEase;

    [Header("Interaction Sounds")]
    public Sound interactInSound;
    public Sound interactOutSound;
    public Sound chestOpenSound;
    public Sound[] hitSounds;

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

    AudioSource mainSource;
    int typeSkipCounter = 0;
    float mainSourceVol;
    float[] externalSourcesVol;
    UEventHandler eventHandler = new UEventHandler();

    private void Awake()
    {
        typeSkipCounter = 0;
        currentManager = this;
        mainSource = GetComponent<AudioSource>();
        SaveAllVolumes();
        SetAllSources(0);
        FadeAllSources(fadeIn: true);
    }

    private void Start()
    {
        playerMovementController.OnJumped.Subscribe(eventHandler, PlayJumpSound);
        playerMovementController.OnLanded.Subscribe(eventHandler, PlayLandSound);
        InteractionHandler.OnInteractableAppeared.Subscribe(eventHandler, (x) => PlaySound(interactInSound));
        InteractionHandler.OnInteractableDisappeared.Subscribe(eventHandler, () => PlaySound(interactOutSound));
        PlayerDamageHandler.OnHit.Subscribe(eventHandler, (x) => PlaySound(PickRandomClip(hitSounds)));
        Chest.OnChestOpened.Subscribe(eventHandler, () => PlaySound(chestOpenSound));
        UIManager.OnFadeScreen.Subscribe(eventHandler, PlayFadeSound);
        PlayerCutsceneManager.OnEndingStarted.Subscribe(eventHandler, ()=> FadeAllSources(fadeIn:false));
    }

    private void OnDestroy()
    {
        if (currentManager == this)
            currentManager = null;


        eventHandler.UnsubcribeAll();
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

    private void SetAllSources(float volume)
    {
        mainSource.volume = volume;
        foreach (var externalSource in externalAudioSources)
            externalSource.volume = volume;
    }

    private void SaveAllVolumes()
    {
        mainSourceVol = mainSource.volume;
        externalSourcesVol = new float[externalAudioSources.Length];
        for (int i = 0; i < externalSourcesVol.Length; i++)
            externalSourcesVol[i] = externalAudioSources[i].volume;
    }

    private void FadeAllSources(bool fadeIn)
    {
        mainSource.DOFade(fadeIn? mainSourceVol:0,fadeDuration).SetEase(fadeEase);
        for (int i = 0; i < externalAudioSources.Length; i++)
            externalAudioSources[i].DOFade(fadeIn ? externalSourcesVol[i] : 0, fadeDuration).SetEase(fadeEase);
    }

    public void PlayDialogueTypeSound()
    {
        if (typeSkipCounter == 0)
            PlaySound(dialogueTypeSound);

        typeSkipCounter++;
        if (typeSkipCounter > numOfSkips)
            typeSkipCounter = 0;
    }


    public void PlayUIClick()
    {
        PlaySound(dialogueTypeSound);
    }

    private void PlayFadeSound(bool fadeIn)
    {
        if (fadeIn)
            PlaySound(interactInSound);
        else
            PlaySound(interactOutSound);
    }


    private void PlayJumpSound()
    {
        PlaySound(PickRandomClip(jumpSounds));

    }

    private void PlayLandSound()
    {
        PlaySound(landSound);
    }


    private void PlaySoundCustomVol(Sound sound, float volumeFactor) => mainSource.PlayOneShot(sound.clip, sound.volume * volumeFactor);
    private void PlaySound(Sound sound) => mainSource.PlayOneShot(sound.clip, sound.volume);
    private Sound PickRandomClip(Sound[] source) => source[UnityEngine.Random.Range(0, source.Length)];
}
