using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ButtonManager : MonoBehaviour
{
    [SerializeField] public float alphaThreshold = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        // Ensures that click radius is the image and not based on rectangle around mouse cursor (checks for alpha fields)
        // If transparent, does not click on anything -- even if the image is there
        this.GetComponent<Image>().alphaHitTestMinimumThreshold = alphaThreshold;   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
