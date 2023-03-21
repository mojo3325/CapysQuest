using UnityEngine;

public class ObjectRotate : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera camera;
    void Start()
    {
        Application.targetFrameRate = 60;
    }

    private void Update()
    {
        if (camera != null && gameObject != null)
        {
            Vector3 viewPos = camera.WorldToViewportPoint(transform.position);
            if (viewPos.x >= -1.5 && viewPos.x <= 1.5 && viewPos.y >= -1.5 && viewPos.y <= 1.5)
            {
                transform.Rotate(0, 0, 120 * Time.fixedDeltaTime);
            }
        }
    }
}
