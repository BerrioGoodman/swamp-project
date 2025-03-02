using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterAnimation : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 0.1f;
    private Renderer rend;
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float offset = scrollSpeed * Time.deltaTime;
        rend.material.mainTextureOffset = new Vector2(offset, offset);
    }
}
