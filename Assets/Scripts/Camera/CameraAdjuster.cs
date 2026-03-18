using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraAdjuster : MonoBehaviour
{
    public enum ScaleMode
    {
        ConstantPixelSize,
        ScaleWithScreenSize,
        ConstantPhysicalSize
    }

    public enum ScreenMatchMode
    {
        MatchWidthOrHeight = 0,
        Expand = 1,
        Shrink = 2
    }

    [Header("Canvas Scaling Options")]
    public ScaleMode uiScaleMode = ScaleMode.ScaleWithScreenSize;
    public ScreenMatchMode screenMatchMode = ScreenMatchMode.MatchWidthOrHeight;
    public Vector2 referenceResolution = new Vector2(800, 600);
    public float matchWidthOrHeight = 0;

    [Header("Physical Size Settings")]
    public float fallbackScreenDPI = 96;
    public enum Unit { Centimeters, Millimeters, Inches, Points, Picas }
    public Unit physicalUnit = Unit.Points;

    [Header("Dynamic Pixels Per Unit")]
    public float dynamicPixelsPerUnit = 1;

    private Camera m_Camera;
    private const float kLogBase = 2;

    void Start()
    {
        m_Camera = GetComponent<Camera>();
        AdjustCamera();
    }

    void AdjustCamera()
    {
        switch (uiScaleMode)
        {
            case ScaleMode.ConstantPixelSize:
                HandleConstantPixelSize();
                break;
            case ScaleMode.ScaleWithScreenSize:
                HandleScaleWithScreenSize();
                break;
            case ScaleMode.ConstantPhysicalSize:
                HandleConstantPhysicalSize();
                break;
        }
    }

    void HandleConstantPixelSize()
    {
        // Здесь можно применить масштабирование камеры для постоянного размера пикселей
        m_Camera.orthographicSize = 1f;  // Пример, зависит от требований
    }

    void HandleScaleWithScreenSize()
    {
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        float scaleFactor = 0;

        switch (screenMatchMode)
        {
            case ScreenMatchMode.MatchWidthOrHeight:
                {
                    float logWidth = Mathf.Log(screenSize.x / referenceResolution.x, kLogBase);
                    float logHeight = Mathf.Log(screenSize.y / referenceResolution.y, kLogBase);
                    float logWeightedAverage = Mathf.Lerp(logWidth, logHeight, matchWidthOrHeight);
                    scaleFactor = Mathf.Pow(kLogBase, logWeightedAverage);
                    break;
                }
            case ScreenMatchMode.Expand:
                scaleFactor = Mathf.Min(screenSize.x / referenceResolution.x, screenSize.y / referenceResolution.y);
                break;
            case ScreenMatchMode.Shrink:
                scaleFactor = Mathf.Max(screenSize.x / referenceResolution.x, screenSize.y / referenceResolution.y);
                break;
        }

        m_Camera.orthographicSize = scaleFactor * 5f;  // Пример, адаптировать под нужды
    }

    void HandleConstantPhysicalSize()
    {
        float currentDpi = Screen.dpi;
        float dpi = (currentDpi == 0 ? fallbackScreenDPI : currentDpi);
        float targetDPI = 1;

        switch (physicalUnit)
        {
            case Unit.Centimeters: targetDPI = 2.54f; break;
            case Unit.Millimeters: targetDPI = 25.4f; break;
            case Unit.Inches: targetDPI = 1; break;
            case Unit.Points: targetDPI = 72; break;
            case Unit.Picas: targetDPI = 6; break;
        }

        m_Camera.orthographicSize = dpi / targetDPI; // Пример, адаптировать под нужды
    }
}
