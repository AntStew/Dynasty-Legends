using UnityEngine;

[System.Serializable]
public class Coach {
    [Tooltip("Visible name for this coach")]
    public string coachName;

    [Tooltip("Portrait sprite to show in the carousel")]
    public Sprite portrait;

    [Tooltip("e.g. Defensive, Offensive, Balanced…")]
    public string playstyle;

    [Tooltip("e.g. Rebounding, Shooting, Passing…")]
    public string focus;
}