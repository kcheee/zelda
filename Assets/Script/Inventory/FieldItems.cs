using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldItems : MonoBehaviour
{
    public Item item;
    public SpriteRenderer image;
    

    // ������ ȹ��
    void Setltem(Item _item)
    {
        item.itemName = _item.itemName;
        item.itemImage = _item.itemImage;
        item.itemType = _item.itemType;
        item.efts = _item.efts;

        image.sprite = _item.itemImage;
    }
    public Item GetItem()
    {
        return item;
    }
    public void DestoryItem()
    {
        Destroy(gameObject);
    }
}
