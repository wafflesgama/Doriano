using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Uevents;
public class StrechnSquach : MonoBehaviour
{
    public float jumpStrechtAmount=1.2f;
    public float jumpStrechtDuration=1.2f;
    public float landStrechtDuration=1.2f;
    public Ease jumpStrechtEase;
    public Ease landStrechtEase;
    public PlayerMovementController playerMovementController;
    Vector3 baseScale;
    Vector3 jumpStretchDirection=new Vector3();
    UeventHandler eventHandler = new UeventHandler();

    void Start()
    {
        baseScale = transform.localScale;
        playerMovementController.OnJumped.Subscribe(eventHandler, Jump_Strech);
        playerMovementController.OnLanded.Subscribe(eventHandler, Land_Squach);
    }
    private void OnDestroy()
    {
        eventHandler.UnsubcribeAll();

    }

    private void Land_Squach()
    {
        transform.DOScale(baseScale, landStrechtDuration).SetEase(landStrechtEase);
    }

  

    private void Jump_Strech()
    {
        transform.DOScale(Vector3.Scale(baseScale, new Vector3(1,jumpStrechtAmount, 1)), jumpStrechtDuration).SetEase(jumpStrechtEase);
    }

}
