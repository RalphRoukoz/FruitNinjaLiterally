using System.Collections;
using UnityEngine;

public class PlayerHitGlow : MonoBehaviour
{
    [SerializeField] private Material glowMaterial; // Assign your material with the glow shader
    [SerializeField] private float glowDuration = 1f; // How long the glow fades out

    private Coroutine glowCoroutine;

    // Call this when the player gets hit
    public void OnHit()
    {
        if (glowCoroutine != null)
            StopCoroutine(glowCoroutine);

        glowCoroutine = StartCoroutine(GlowEffect());
    }

    private IEnumerator GlowEffect()
    {
        float time = 0f;

        // Immediately set glow to full
        glowMaterial.SetFloat("_HitEffect", 1f);

        while (time < glowDuration)
        {
            time += Time.deltaTime;
            float t = 1f - (time / glowDuration); // Lerp from 1 to 0
            glowMaterial.SetFloat("_HitEffect", t);
            yield return null;
        }

        glowMaterial.SetFloat("_HitEffect", 0f);
        glowCoroutine = null;
    }
}