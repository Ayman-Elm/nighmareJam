using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class LightMod : MonoBehaviour
{
    [Header("Light Settings")]
    public Color lightColor = Color.white;
    [Range(0, 5)] public float intensity = 1f;

    [Header("Spot Settings (for 2D Spot Light)")]
    [Range(0, 360)] public float innerAngle = 70.588f;
    [Range(0, 360)] public float outerAngle = 70.588f;
    [Range(0, 10)]  public float innerRadius = 1f;
    [Range(0, 10)]  public float outerRadius = 5.59f;
    [Range(0, 1)]   public float falloffStrength = 0.109f;

    [Header("Light Toggle")]
    // When true, the light is only enabled while the left mouse button is held down.
    public bool onlyOnLeftMouse = false;
    
    private Light2D _light2D;

    private void Awake()
    {
        // Grab the attached Light2D component
        _light2D = GetComponent<Light2D>();
    }

    private void Update()
    {
        // Update the light properties in case they're changed in the Inspector
        _light2D.color                 = lightColor;
        _light2D.intensity             = intensity;
        _light2D.pointLightInnerAngle  = innerAngle;
        _light2D.pointLightOuterAngle  = outerAngle;
        _light2D.pointLightInnerRadius = innerRadius;
        _light2D.pointLightOuterRadius = outerRadius;
        _light2D.falloffIntensity      = falloffStrength;
        
        // If onlyOnLeftMouse = true, light is on only while left mouse is pressed
        if (onlyOnLeftMouse)
        {
            _light2D.enabled = Input.GetMouseButton(0);
        }
        else
        {
            _light2D.enabled = true;
        }
    }
}
