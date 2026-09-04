using UnityEngine;
using TMPro;

public class FPSViewer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmpText = null;

    private int frameCount;
    private float totalFrameTime;

    private void Update()
    {
        frameCount++;
        totalFrameTime += Time.unscaledDeltaTime;

        if (frameCount >= 1000)
        {
            float averageFPS = frameCount / totalFrameTime;
            string logText = $"Average FPS: {averageFPS:F2}";

            //Debug.Log(logText);
            tmpText.text = logText;

            frameCount = 0;
            totalFrameTime = 0f;
        }
    }
}
