using UnityEngine;
using TMPro;
using System.Collections;

public class TMPTypewriter : MonoBehaviour
{
    [Header("Type-writer Settings")]
    [SerializeField] private TMP_Text tmpText;
    [SerializeField] private float delay        = 0.05f;  // time between characters
    
    [Header("Post-typing Settings")]
    [SerializeField] private float pauseDuration = 1f;    // how long to wait after typing
    [SerializeField] private float fadeDuration  = 1f;    // how long the fade-out takes

    private void Awake()
    {
        // ensure full alpha at start
        Color c = tmpText.color;
        c.a = 1f;
        tmpText.color = c;

        tmpText.ForceMeshUpdate();
        StartCoroutine(PlayText());
    }

    private IEnumerator PlayText()
    {
        int totalChars = tmpText.textInfo.characterCount;
        tmpText.maxVisibleCharacters = 0;

        // type-writer effect
        for (int i = 1; i <= totalChars; i++)
        {
            tmpText.maxVisibleCharacters = i;
            yield return new WaitForSeconds(delay);
        }

        // pause when done
        yield return new WaitForSeconds(pauseDuration);

        // fade out
        float elapsed = 0f;
        Color startCol = tmpText.color;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            tmpText.color = new Color(startCol.r, startCol.g, startCol.b, alpha);
            yield return null;
        }
        // ensure fully invisible
        tmpText.color = new Color(startCol.r, startCol.g, startCol.b, 0f);
    }
}
