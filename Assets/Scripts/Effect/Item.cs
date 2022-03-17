using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int id;
    public string itemName;
    public float coolTime;
    public int hpValue;
    public int mpValue;
    public Sprite icon;
    public string description;

    public Character User { get; private set; }

    public bool CanUse { get; set; }

    public void Start()
    {
        CanUse = true;
    }

    public void Use(Character character)
    {
        character.Damage(-hpValue);
        character.CostMp(-mpValue);
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.collider.CompareTag("Player"))
    //    {
    //        Player.Instance.GetItem(this);
    //    }
    //}
}
