using UnityEngine;
using TMPro;

public class TextFadeInAndOut : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float minAlpha = 0.2f;
    [SerializeField] private float maxAlpha = 1f;
    [SerializeField] private float fadeSpeed = 0.5f;
    private float range;
    private float newAlpha;

    private void Start()
    {
        range = maxAlpha - minAlpha;
    }

    void Update()
    {
        newAlpha = Mathf.PingPong(Time.time * fadeSpeed, range) + minAlpha;

        text.color = new Color(1f, 1f, 1f, newAlpha);
    }
}
