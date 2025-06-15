using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoachCarousel : MonoBehaviour {
    [Tooltip("Drag in your Coach data (portrait + stats) here in the same order as your coachItems.")]
    public List<Coach> coaches;

    [Tooltip("Drag in your coach UI RectTransforms here in the same order as your coaches list.")]
    public List<RectTransform> coachItems;

    [Tooltip("Horizontal offset of side previews.")]
    public float xOffset = 300f;
    [Tooltip("Scale of the side previews.")]
    public float sideScale = 0.8f;
    [Tooltip("Seconds to animate between positions.")]
    public float transitionDuration = 0.3f;
    [Tooltip("Minimum swipe distance (px) to trigger.")]
    public float swipeThreshold = 50f;

    int currentIndex = 0;
    bool isTransitioning = false;
    Vector2 touchStart;

    void Start() {
        // sanity check
        if (coaches.Count != coachItems.Count) {
            Debug.LogError($"CoachCarousel: coaches.Count ({coaches.Count}) != coachItems.Count ({coachItems.Count})");
        }

        // Position & scale immediately on start
        UpdatePositions(instant: true);
    }

    void Update() {
        if (isTransitioning) return;

        if (Input.touchCount == 1) {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) {
                touchStart = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended) {
                float deltaX = touch.position.x - touchStart.x;
                if (Mathf.Abs(deltaX) >= swipeThreshold) {
                    if (deltaX < 0) ShowNext();
                    else          ShowPrevious();
                }
            }
        }
    }

    public void ShowNext() {
        if (isTransitioning) return;
        currentIndex = (currentIndex + 1) % coachItems.Count;
        StartTransition();
    }

    public void ShowPrevious() {
        if (isTransitioning) return;
        currentIndex = (currentIndex - 1 + coachItems.Count) % coachItems.Count;
        StartTransition();
    }

    void StartTransition() {
        isTransitioning = true;
        UpdatePositions(instant: false);
        StartCoroutine(EndTransitionAfterDelay());
    }

    IEnumerator EndTransitionAfterDelay() {
        yield return new WaitForSeconds(transitionDuration);
        isTransitioning = false;
    }

    void UpdatePositions(bool instant) {
        for (int i = 0; i < coachItems.Count; i++) {
            // compute offset slot (-1, 0, +1)
            int rawOffset = i - currentIndex;
            if (rawOffset > 1) rawOffset -= coachItems.Count;
            if (rawOffset < -1) rawOffset += coachItems.Count;

            float targetX     = rawOffset * xOffset;
            float targetScale = (rawOffset == 0) ? 1f : sideScale;

            if (instant) {
                coachItems[i].anchoredPosition = new Vector2(targetX, 0f);
                coachItems[i].localScale       = Vector3.one * targetScale;
            }
            else {
                StartCoroutine(AnimateTransform(
                    coachItems[i],
                    new Vector2(targetX, 0f),
                    Vector3.one * targetScale
                ));
            }
        }
    }

    IEnumerator AnimateTransform(RectTransform rt, Vector2 toPos, Vector3 toScale) {
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
    /// Call this to retrieve the currently‐centered coach’s data (portrait + stats).
    /// </summary>
    public Coach GetCurrentCoach() {
        if (coaches == null || coaches.Count == 0) return null;
        return coaches[currentIndex];
    }
}