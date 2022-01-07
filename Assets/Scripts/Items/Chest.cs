using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Uevents;

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


    public static Uevent OnChestOpened= new Uevent();
    public static Uevent<Sprite,string,string> OnChestItemShow= new Uevent<Sprite, string, string>();

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
        OnChestOpened.TryInvoke();
        GameManager.currentGameManager.ItemCollected(item.name);
        gameObject.tag = "Uninteractable";
    }

    #endregion

    #region Animation Invoked Functions
    public void ShowItemUI()
    {
        OnChestItemShow.TryInvoke(item.image,item.name,item.description);
        glowSystem.Stop();
    }

    public void OpenChestAction()
    {
        OnChestOpened.TryInvoke();
        lidCollider.gameObject.SetActive(true);
    }
    #endregion


}
