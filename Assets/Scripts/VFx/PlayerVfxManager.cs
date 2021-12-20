using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerVfxManager : MonoBehaviour
{
    public PlayerMovementController movementController;

    [Header("Dust effect")]
    public VisualEffect dustVisualEffect;
    public float speedStartThreshold = 8;
    public float particleRate = 8;

    [Header("Hit effect")]
    public VisualEffect hitVisualEffect;

    [Header("Splash effect")]
    public VisualEffect splashVisualEffect;

    float rate;
    bool followedByJump, isLanding;

    UEventHandler eventHandler = new UEventHandler();

    private void Awake()
    {
        dustVisualEffect.gameObject.SetActive(true);
    }

    void Start()
    {
        dustVisualEffect.SetFloat("Rate", 0);

        movementController.OnJumped.Subscribe(eventHandler, JumpParticles);
        movementController.OnLanded.Subscribe(eventHandler, LandParticles);
        PlayerDamageHandler.OnHit.Subscribe(eventHandler, HitParticles);
        InteractionHandler.OnSplash.Subscribe(eventHandler, SplashParticles);
    }

    private void OnDestroy()
    {
        eventHandler.UnsubcribeAll();
    }

    void Update()
    {
        if (rate != 0 && movementController.playerHorizontalVelocity.magnitude <= speedStartThreshold)
        {
            rate = 0;
            dustVisualEffect.SetFloat("Rate", rate);
        }
        else if (rate == 0 && movementController.playerHorizontalVelocity.magnitude > speedStartThreshold)
        {
            rate = particleRate;
            dustVisualEffect.SetFloat("Rate", rate);
        }
    }

    private async void LandParticles()
    {
        isLanding = true;
        await Task.Delay(100);
        dustVisualEffect.SendEvent("OnLand");
        await Task.Delay(200);

        if (followedByJump)
            followedByJump = false;
        else
            dustVisualEffect.Play();

        isLanding = false;
    }

    private async void JumpParticles()
    {
        if (isLanding)
            followedByJump = true;

        dustVisualEffect.Stop();
        await Task.Delay(50);
        dustVisualEffect.SendEvent("OnJump");
    }

    private void HitParticles(Vector3 pos)
    {
        hitVisualEffect.SetVector3("SourcePos", pos);
        hitVisualEffect.SendEvent("OnHit");

    }

    private void SplashParticles(Vector3 pos)
    {
        splashVisualEffect.SetVector3("SourcePos", pos);
        splashVisualEffect.SendEvent("OnSplash");
    }
}
