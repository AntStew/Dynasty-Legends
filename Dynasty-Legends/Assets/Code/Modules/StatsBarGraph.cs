using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatsBarGraph : MonoBehaviour {
    [Header("UI References")]
    public RectTransform container;   // StatsContainer for bars
    public GameObject     barPrefab;  // StatBar prefab (root must have CanvasGroup)
    public TMP_Text       headerText; // Text component to fade in/out

    [Header("Player Carousel")]
    public PlayerCarousel playerCarousel;  // provides CurrentIndex & IsTransitioning

    [Header("Settings")]
    public int   maxValue         = 100;
    public float barFadeDuration  = 0.3f;
    public float delayBetweenBars = 0.1f;

    [Tooltip("Which carousel slot this graph belongs to (0=left,1=center,2=right)")]
    public int slotIndex;

    private Player lastPlayer;
    private CanvasGroup headerGroup;

    void Start() {
        if (container == null || barPrefab == null || playerCarousel == null || headerText == null) {
            Debug.LogError("StatsBarGraph: assign container, barPrefab, playerCarousel, and headerText in the Inspector.");
            enabled = false;
            return;
        }
        lastPlayer = null;
        container.gameObject.SetActive(false);
        headerGroup = headerText.GetComponent<CanvasGroup>();
        if (headerGroup == null) headerGroup = headerText.gameObject.AddComponent<CanvasGroup>();
        headerGroup.alpha = 0f;
    }

    void Update() {
        bool shouldShow =
            !playerCarousel.IsTransitioning &&
            playerCarousel.CurrentIndex == slotIndex;

        if (!shouldShow) {
            // Hide bars and header
            container.gameObject.SetActive(false);
            headerGroup.alpha = 0f;
            lastPlayer = null;
            return;
        }

        Player current = playerCarousel.GetCurrentPlayer();
        if (current != lastPlayer) {
            lastPlayer = current;
            StopAllCoroutines();
            StartCoroutine(ShowStatsWithFade(current));
        }
    }

    private IEnumerator ShowStatsWithFade(Player p) {
        // Clear old bars
        foreach (Transform child in container) Destroy(child.gameObject);
        container.gameObject.SetActive(false);
        headerGroup.alpha = 0f;
        if (p == null) yield break;

        // Prepare stats list
        var stats = new List<(string name, int value)> {
            ("Shooting",  p.shooting),
            ("Dribbling", p.dribbling),
            ("Speed",     p.speed),
            ("Vertical",  p.vertical),
            ("Steal",     p.steal),
            ("Stamina",   p.stamina)
        };

        // Instantiate bars invisible, collect CanvasGroups
        var barGroups = new List<CanvasGroup>();
        foreach (var stat in stats) {
            var go = Instantiate(barPrefab, container);
            var cg = go.GetComponent<CanvasGroup>();
            if (cg == null) cg = go.AddComponent<CanvasGroup>();
            cg.alpha = 0f;

            var label = go.GetComponentInChildren<TMP_Text>();
            if (label != null) label.text = stat.name;

            var fill = go.transform.Find("BarBG/Fill")?.GetComponent<Image>();
            if (fill != null) fill.fillAmount = Mathf.Clamp01((float)stat.value / maxValue);

            barGroups.Add(cg);
        }

        // Activate container
        container.gameObject.SetActive(true);

        // Fade in header text
        yield return FadeCanvasGroup(headerGroup, 0f, 1f, barFadeDuration);

        // Fade in each bar sequentially
        foreach (var cg in barGroups) {
            yield return FadeCanvasGroup(cg, 0f, 1f, barFadeDuration);
            yield return new WaitForSeconds(delayBetweenBars);
        }
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration) {
        float elapsed = 0f;
        cg.alpha = from;
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        cg.alpha = to;
    }
}