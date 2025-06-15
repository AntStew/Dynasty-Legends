using UnityEngine;

public class DraftGameManager : MonoBehaviour
{
    public static DraftGameManager Instance { get; private set; }
    public string TeamName { get; set; }
    public Coach SelectedCoach { get; set; }
    public Player SelectedPG { get; set; }
    public Player SelectedSG { get; set; }
    public Player SelectedSF { get; set; }
    public Player SelectedPF { get; set; }
    public Player SelectedC  { get; set; }

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else Destroy(gameObject);
    }
}
