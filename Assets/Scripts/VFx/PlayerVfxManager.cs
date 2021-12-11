using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerVfxManager : MonoBehaviour
{
    public PlayerMovementController movementController;

    [Header("Dust trail")]
    public VisualEffect dustVisualEffect;
    public float speedStartThreshold = 8;
    public float particleRate = 8;

    [Header("Hit trail")]
    public VisualEffect hitVisualEffect;

    float rate;
    bool followedByJump, isLanding;

    private void Awake()
    {
        dustVisualEffect.gameObject.SetActive(true);
    }

    void Start()
    {
        dustVisualEffect.SetFloat("Rate", 0);
        movementController.onJumped += JumpParticles;
        movementController.onLanded += LandParticles;
        PlayerDamageHandler.OnHit += HitParticles;
    }

    private void OnDestroy()
    {
        movementController.onJumped -= JumpParticles;
        movementController.onLanded -= LandParticles;
        PlayerDamageHandler.OnHit -= HitParticles;
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
        hitVisualEffect.SetVector3("SourcePos",pos);
        hitVisualEffect.SendEvent("OnHit");
    }

    
}
