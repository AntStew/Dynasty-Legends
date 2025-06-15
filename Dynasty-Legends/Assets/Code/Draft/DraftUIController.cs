using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DraftUIController : MonoBehaviour {
    [Header("All Panels (in order)")]
    // 0 = Name Input, 1 = Welcome, 2 = Coach, 3 = PG, 4 = SG, 5 = SF, 6 = PF, 7 = C, 8 = Agent
    public List<CanvasGroup> panels;

    [Header("Welcome Message")]
    public TMP_Text welcomeText;
    public string welcomeMessage = "Welcome to the Draft";
    public float typeSpeed = 0.05f;

    [Header("Team Name Input")]
    public TMP_InputField teamNameInput;

    [Header("Coach Selection")]
    public CoachCarousel coachCarousel;

    [Header("Player Selection Carousels")]
    public PlayerCarousel pgCarousel;
    public PlayerCarousel sgCarousel;
    public PlayerCarousel sfCarousel;
    public PlayerCarousel pfCarousel;
    public PlayerCarousel cCarousel;

    [Header("Fade Settings")]
    public float fadeDuration = 0.5f;

    int currentIndex = 0;

    void Start() {
        // Initialize so only the Name Input (panel[0]) is visible
        for (int i = 0; i < panels.Count; i++) {
            bool isActive = (i == 0);
            panels[i].alpha = isActive ? 1f : 0f;
            panels[i].interactable = isActive;
            panels[i].blocksRaycasts = isActive;
        }

        // Wire Next/Back on all panels except Welcome (index 1)
        for (int i = 0; i < panels.Count; i++) {
            if (i == 1) continue; // skip Welcome
            Transform t = panels[i].transform;
            var nb = t.Find("NextButton")?.GetComponent<Button>();
            var bb = t.Find("BackButton")?.GetComponent<Button>();
            if (nb != null) nb.onClick.AddListener(NextPanel);
            if (bb != null) bb.onClick.AddListener(PrevPanel);
        }
    }

    public void NextPanel() {
        // 0 = Name Input → go to Welcome + start typewriter
        if (currentIndex == 0) {
            string name = teamNameInput.text.Trim();
            if (string.IsNullOrEmpty(name)) return;
            DraftGameManager.Instance.TeamName = name;

            StartCoroutine(TransitionPanels(0, 1, () => StartCoroutine(RunWelcomeSequence())));
            currentIndex = 1;
            return;
        }

        // other panels: Coach (2) through Agent (8)
        // Save selections before advancing
        if (currentIndex == 2) {
            var coach = coachCarousel.GetCurrentCoach();
            if (coach != null) DraftGameManager.Instance.SelectedCoach = coach;
        } else if (currentIndex == 3) {
            var p = pgCarousel.GetCurrentPlayer();
            if (p != null) DraftGameManager.Instance.SelectedPG = p;
        } else if (currentIndex == 4) {
            var p = sgCarousel.GetCurrentPlayer();
            if (p != null) DraftGameManager.Instance.SelectedSG = p;
        } else if (currentIndex == 5) {
            var p = sfCarousel.GetCurrentPlayer();
            if (p != null) DraftGameManager.Instance.SelectedSF = p;
        } else if (currentIndex == 6) {
            var p = pfCarousel.GetCurrentPlayer();
            if (p != null) DraftGameManager.Instance.SelectedPF = p;
        } else if (currentIndex == 7) {
            var p = cCarousel.GetCurrentPlayer();
            if (p != null) DraftGameManager.Instance.SelectedC = p;
        }

        int next = currentIndex + 1;
        if (next >= panels.Count) {
            SceneManager.LoadScene("home");
            return;
        }

        StartCoroutine(TransitionPanels(currentIndex, next));
        currentIndex = next;
    }

    public void PrevPanel() {
        // allow back from Coach onward
        if (currentIndex <= 2) return;
        int prev = currentIndex - 1;
        StartCoroutine(TransitionPanels(currentIndex, prev));
        currentIndex = prev;
    }

    IEnumerator RunWelcomeSequence() {
        welcomeText.text = string.Empty;
        foreach (char c in welcomeMessage) {
            welcomeText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
        yield return new WaitForSeconds(0.5f);

        // after typewriter, auto transition to Coach
        StartCoroutine(TransitionPanels(1, 2));
        currentIndex = 2;
    }

    IEnumerator TransitionPanels(int from, int to, System.Action onComplete = null) {
        yield return Fade(panels[from], 1f, 0f);
        panels[from].interactable = panels[from].blocksRaycasts = false;

        yield return Fade(panels[to], 0f, 1f);
        panels[to].interactable = panels[to].blocksRaycasts = true;

        onComplete?.Invoke();
    }

    IEnumerator Fade(CanvasGroup cg, float start, float end) {
        float elapsed = 0f;
        cg.alpha = start;
        while (elapsed < fadeDuration) {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, elapsed / fadeDuration);
            yield return null;
        }
        cg.alpha = end;
    }
}