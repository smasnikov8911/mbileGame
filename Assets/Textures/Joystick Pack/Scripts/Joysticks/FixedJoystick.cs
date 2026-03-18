using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedJoystick : Joystick
{
#if UNITY_EDITOR || UNITY_STANDALONE
    private void Update()
    {
        Vector2 keyboardInput = Vector2.zero;

        if (Input.GetKey(KeyCode.W)) keyboardInput.y += 1;
        if (Input.GetKey(KeyCode.S)) keyboardInput.y -= 1;
        if (Input.GetKey(KeyCode.A)) keyboardInput.x -= 1;
        if (Input.GetKey(KeyCode.D)) keyboardInput.x += 1;

        if (keyboardInput != Vector2.zero)
        {
            input = keyboardInput.normalized;
            FormatInput();
            handle.anchoredPosition = input * (background.sizeDelta / 2f) * handleRange;
        }
        else if (!Input.anyKey)
        {
            input = Vector2.zero;
            handle.anchoredPosition = Vector2.zero;
        }
    }
#endif
}