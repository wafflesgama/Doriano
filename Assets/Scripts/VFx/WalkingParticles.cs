using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;

public class WalkingParticles : MonoBehaviour
{
    public PlayerMovementController movementController;
    public float speedStartThreshold = .5f;
    public float particleRate = 10;
    VisualEffect visualEffect;

    public UEventHandler eventHandler = new UEventHandler();

    float rate;
    bool followedByJump, isLanding;

    void Start()
    {
        visualEffect = GetComponent<VisualEffect>();
        visualEffect.SetFloat("Rate", 0);

        movementController.OnJumped.Subscribe(eventHandler,JumpParticles);
        movementController.OnLanded.Subscribe(eventHandler,LandParticles);
        //movementController.onLanded += LandParticles;
    }

    private void OnDestroy()
    {
        //movementController.OnJumped -= JumpParticles;
        //movementController.onLanded -= LandParticles;
    }

    private async void LandParticles()
    {
        isLanding = true;
        await Task.Delay(100);
        visualEffect.SendEvent("OnLand");
        await Task.Delay(200);

        if (followedByJump)
            followedByJump = false;
        else
            visualEffect.Play();

        isLanding = false;
    }

    private async void JumpParticles()
    {
        if (isLanding)
            followedByJump = true;

        visualEffect.Stop();
        await Task.Delay(50);
        visualEffect.SendEvent("OnJump");
    }

    // Update is called once per frame
    void Update()
    {
        if (rate != 0 && movementController.playerHorizontalVelocity.magnitude <= speedStartThreshold)
        {
            rate = 0;
            visualEffect.SetFloat("Rate", rate);
        }
        else if (rate == 0 && movementController.playerHorizontalVelocity.magnitude > speedStartThreshold)
        {
            rate = particleRate;
            visualEffect.SetFloat("Rate", rate);
        }
    }
}
