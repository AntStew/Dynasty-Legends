using UnityEngine;

public enum Position { PG, SG, SF, PF, C }

[System.Serializable]
public class Player {
    public string        playerName;
    public Position      position;
    [Range(1,100)]     public int shooting;
    [Range(1,100)]     public int dribbling;
    [Range(1,100)]     public int speed;
    [Range(1,100)]     public int vertical;
    [Range(1,100)]     public int steal;
    [Range(1,100)]     public int stamina;
    [Range(1,5)]     public int height;
    public string playstyle;   // e.g. “Defensive,” match vs. coach
    public Sprite        portrait;    // drag in your art
}
