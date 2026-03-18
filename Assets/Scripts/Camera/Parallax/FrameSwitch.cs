using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AdaptivePerformance.VisualScripting;

public class FrameSwitch : MonoBehaviour
{
    public GameObject activeFrame;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && activeFrame != null)
        {
            activeFrame.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && activeFrame != null)
        {
            activeFrame.SetActive(false);
        }
    }
}
