using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StrechnSquach : MonoBehaviour
{
    public float jumpStrechtAmount=1.2f;
    public float jumpStrechtDuration=1.2f;
     public Ease jumpStrechtEase;
    public PlayerMovementController playerMovementController;
    Vector3 baseScale;
    Vector3 jumpStretchDirection=new Vector3();
    UEventHandler eventHandler = new UEventHandler();

    void Start()
    {
        baseScale = transform.localScale;
        playerMovementController.OnJumped.Subscribe(eventHandler, Jump_Strech);
        playerMovementController.OnLanded.Subscribe(eventHandler, Jump_Squach);
    }
    private void OnDestroy()
    {
        eventHandler.UnsubcribeAll();

    }

    private void Jump_Squach()
    {
        transform.DOScale(baseScale, jumpStrechtDuration).SetEase(jumpStrechtEase);
    }

  

    private void Jump_Strech()
    {
        transform.DOScale(Vector3.Scale(baseScale, new Vector3(1,jumpStrechtAmount, 1)), jumpStrechtDuration).SetEase(jumpStrechtEase);
    }

}
