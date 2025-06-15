// PlayerCarousel.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCarousel : MonoBehaviour {
    [Tooltip("Drag in your Player data (portrait + stats) here in the same order as your items.")]
    public List<Player> players;

    [Tooltip("Drag in your player UI RectTransforms here in the same order as your players list.")]
    public List<RectTransform> items;

    [Tooltip("Horizontal offset of side previews.")]
    public float xOffset = 300f;

    [Tooltip("Scale of the side previews.")]
    public float sideScale = 0.8f;

    [Tooltip("Seconds to animate between positions.")]
    public float transitionDuration = 0.3f;

    [Tooltip("Minimum swipe distance (px) to trigger.")]
    public float swipeThreshold = 50f;

    private int currentIndex = 0;
    private bool isTransitioning = false;
    private Vector2 touchStart;

    /// <summary> True while the carousel is animating between slots. </summary>
    public bool IsTransitioning => isTransitioning;
    /// <summary> Index (0=left,1=center,2=right,...) of the currently centered slot. </summary>
    public int CurrentIndex => currentIndex;

    void Start() {
        if (players.Count != items.Count) {
            Debug.LogError($"PlayerCarousel: players.Count ({players.Count}) != items.Count ({items.Count})");
        }
        UpdatePositions(instant: true);
    }

    void Update() {
        if (isTransitioning) return;

        if (Input.touchCount == 1) {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) {
                touchStart = touch.position;
            } else if (touch.phase == TouchPhase.Ended) {
                float deltaX = touch.position.x - touchStart.x;
                if (Mathf.Abs(deltaX) >= swipeThreshold) {
                    if (deltaX < 0) ShowNext();
                    else             ShowPrevious();
                }
            }
        }
    }

    public void ShowNext() {
        if (isTransitioning) return;
        currentIndex = (currentIndex + 1) % items.Count;
        StartTransition();
    }

    public void ShowPrevious() {
        if (isTransitioning) return;
        currentIndex = (currentIndex - 1 + items.Count) % items.Count;
        StartTransition();
    }

    private void StartTransition() {
        isTransitioning = true;
        UpdatePositions(instant: false);
        StartCoroutine(EndTransitionAfterDelay());
    }

    private IEnumerator EndTransitionAfterDelay() {
        yield return new WaitForSeconds(transitionDuration);
        isTransitioning = false;
    }

    private void UpdatePositions(bool instant) {
        for (int i = 0; i < items.Count; i++) {
            int rawOffset = i - currentIndex;
            if (rawOffset > 1) rawOffset -= items.Count;
            if (rawOffset < -1) rawOffset += items.Count;

            float targetX = rawOffset * xOffset;
            float targetScale = (rawOffset == 0) ? 1f : sideScale;

            if (instant) {
                items[i].anchoredPosition = new Vector2(targetX, 0f);
                items[i].localScale       = Vector3.one * targetScale;
            } else {
                StartCoroutine(AnimateTransform(
                    items[i],
                    new Vector2(targetX, 0f),
                    Vector3.one * targetScale
                ));
            }
        }
    }

    private IEnumerator AnimateTransform(RectTransform rt, Vector2 toPos, Vector3 toScale) {
        Vector2 fromPos   = rt.anchoredPosition;
        Vector3 fromScale = rt.localScale;
        float elapsed = 0f;

        while (elapsed < transitionDuration) {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / transitionDuration);
            rt.anchoredPosition = Vector2.Lerp(fromPos, toPos, t);
            rt.localScale       = Vector3.Lerp(fromScale, toScale, t);
            yield return null;
        }

        rt.anchoredPosition = toPos;
        rt.localScale       = toScale;
    }

    /// <summary>
    /// Retrieves the currently-centered player's data (portrait + stats).
    /// </summary>
    public Player GetCurrentPlayer() {
        if (players == null || players.Count == 0) return null;
        return players[currentIndex];
    }
}
