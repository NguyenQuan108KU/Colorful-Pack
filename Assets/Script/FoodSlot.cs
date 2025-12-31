
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodSlot : MonoBehaviour
{
    private Image _imgFood;

    private void Awake()
    {
        _imgFood = this.transform.GetChild(0).GetComponent<Image>();
    }
    public void OnSetSlot(Sprite spr)
    {
        _imgFood.gameObject.SetActive(true);
        _imgFood.sprite = spr;
        //_imgFood.SetNativeSize();
    } 
    public void OnActiveFood(bool active)
    {
        _imgFood?.gameObject.SetActive(active);
    }
    public void Clear()
    {
        _imgFood.gameObject.SetActive(false);
        _imgFood.sprite = null;
    }
    public bool HasFood => _imgFood.gameObject.activeInHierarchy;
    public Sprite GetSpriteFood => _imgFood.sprite;
}
