using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    public enum ShakeAxis
    {
        Horizontal,
        Vertical
    }

    public ShakeAxis shakeAxis = ShakeAxis.Horizontal;
    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 0.3f;
    public float dampingSpeed = 1.0f;

    private Vector3 currentOffset = Vector3.zero;
    private float currentShakeDuration = 0f;
    private bool isShaking = false;
    private float strength = 0.75f;
    private float shakeCap = 3f;

    void Awake()
    {
        Instance = this;
    }

    void LateUpdate()
    {
        Vector3 basePosition = transform.localPosition - currentOffset;

        if (currentShakeDuration > 0 && shakeMagnitude > 0)
        {
            float offset = Random.Range(-1f, 1f) * shakeMagnitude * strength * (PlayerPowerUps.Instance.getIsRaged() ? 1.2f : 1f);

            if (shakeAxis == ShakeAxis.Horizontal)
            {
                currentOffset = new Vector3(offset, 0f, 0f);
            }
            else
            {
                currentOffset = new Vector3(0f, offset, 0f);
            }

            currentShakeDuration -= Time.deltaTime * dampingSpeed;
            shakeMagnitude -= Mathf.Max(1, shakeMagnitude * 2) * Time.deltaTime;
        }
        else if (isShaking)
        {
            if (shakeMagnitude > 0.3f)
            {
                ShakeFalloff(shakeMagnitude / 2f);
            }
            else
            {
                currentOffset = Vector3.zero;
                isShaking = false;
            }
        }

        transform.localPosition = basePosition + currentOffset;
    }

    public void Shake()
    {
        Shake(shakeDuration, shakeMagnitude, shakeAxis);
    }

    public void Shake(float duration, float magnitude)
    {
        Shake(duration, magnitude, shakeAxis);
    }

    public void Shake(float duration, float magnitude, ShakeAxis axis)
    {
        shakeAxis = axis;
        if (!isShaking) {
            shakeMagnitude = magnitude;
            currentShakeDuration = duration;
        }
        else
        {
            shakeMagnitude += magnitude * 0.5f;
            currentShakeDuration += duration * 0.5f;
            if (shakeMagnitude > shakeCap)
            {
                shakeMagnitude = shakeCap;
            }
        }
        isShaking = true;
    }

    private void ShakeFalloff(float magnitude)
    {
        shakeMagnitude = magnitude;
        currentShakeDuration = magnitude / 2f;
    }
}