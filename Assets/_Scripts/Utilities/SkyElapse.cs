using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyElapse : MonoBehaviour
{
    float scrollSpeed = 2f;
    Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        float offset = Time.time * scrollSpeed/200;
        rend.material.mainTextureOffset = new Vector2(offset, 0);
    }
}
