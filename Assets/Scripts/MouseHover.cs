using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHover : MonoBehaviour {

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        FollowMouse();
	}

    private void FollowMouse()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    public void SetHoverIcon(Sprite icon)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.sprite = icon;
    }
}
