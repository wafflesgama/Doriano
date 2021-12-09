using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


[Serializable]
public struct Item
{
    public Sprite image;
    public string name;
    [Multiline] public string description;
}

public class Chest : MonoBehaviour, Interactable
{
    [Header("Interactable Implementation")]
    [SerializeField] public Item item;
    public Vector3 offset;


    [Header("Utils")]
    public ParticleSystem glowSystem;
    public Collider lidCollider;


    public delegate void ItemAction(Sprite image,string name, string description);

    public static System.Action OnChestOpened;
    public static ItemAction OnChestItemShow;

    PlayableDirector director;

    void Awake()
    {
        director = GetComponent<PlayableDirector>();
        lidCollider.gameObject.SetActive(false);
    }


    #region Interface Implementattion
    public Vector3 GetOffset() => offset;

    public void Interact()
    {
        director.Play();
        OnChestOpened?.Invoke();
        gameObject.tag = "Untagged";
    }

    #endregion

    #region Animation Invoked Functions
    public void ShowItemUI()
    {
        OnChestItemShow?.Invoke(item.image,item.name,item.description);
        glowSystem.Stop();
    }

    public void OpenChestAction()
    {
        OnChestOpened?.Invoke();
        lidCollider.gameObject.SetActive(true);
    }
    #endregion


}
