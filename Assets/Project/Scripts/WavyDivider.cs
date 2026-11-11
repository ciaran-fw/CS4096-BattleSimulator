using UnityEngine;


public class WavyDivider : MonoBehaviour
{
    [Header("Line Renderers")]
    [SerializeField] private LineRenderer leftLineRenderer;
    [SerializeField] private LineRenderer rightLineRenderer;

    [Header("Colours and Transparency")]
    [SerializeField] private Color leftLineColour = Color.blue;
    [SerializeField] private Color rightLineColour = Color.red;
    [Range(0, 255)]
    [SerializeField] private int transparency = 200;

    [Header("Line Settings")]
    [SerializeField] private float lineHeight = 200f;
    [SerializeField] private float lineWidth = 0.1f;

    [Header("Divider Position")]
    [SerializeField] private float lineSpacing = 0.5f;

    private void Start()
    {
        SetupLine(leftLineRenderer, leftLineColour, -lineSpacing);
        SetupLine(rightLineRenderer, rightLineColour, lineSpacing);
    }

    private void SetupLine(LineRenderer line, Color colour, float xPosition)
    {
        line.useWorldSpace = true;

        line.startWidth = lineWidth;
        line.endWidth = lineWidth;

        // unity colour alpha between 0 and 1 not 0 and 255.
        colour.a = transparency / 255f;

        line.startColor = colour;
        line.endColor = colour;

        line.positionCount = 2;

        line.SetPosition(0, new Vector3(xPosition, lineHeight / 2f, 0));
        line.SetPosition(1, new Vector3(xPosition, -lineHeight / 2f, 0));
    }
}
