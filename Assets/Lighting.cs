using UnityEngine;

public class Lightning : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform startTransform;
    public Transform endTransform;
    public int segments = 10;

    void Start()
    {
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        GenerateLightning();
    }

    void GenerateLightning()
    {
        lineRenderer.positionCount = segments + 1;
        lineRenderer.useWorldSpace = true;

        Vector3[] positions = new Vector3[segments + 1];
        positions[0] = startTransform.position;
        positions[segments] = endTransform.position;

        for (int i = 1; i < segments; i++)
        {
            float t = i / (float)segments;
            positions[i] = Vector3.Lerp(startTransform.position, endTransform.position, t) +
                           new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        }

        lineRenderer.SetPositions(positions);
    }
}
