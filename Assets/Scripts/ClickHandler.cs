using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickHandler : MonoBehaviour
{
    Animator animator;
    public  Action<GameObject> ObjectClicked;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnMouseUpAsButton()
    {
        VisualLogger.Log(this.transform.tag+" clicked");
        ObjectClicked?.Invoke(this.gameObject);
    }

}