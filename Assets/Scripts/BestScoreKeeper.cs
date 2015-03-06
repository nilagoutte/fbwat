using UnityEngine;
using UnityEngine.UI;

public class BestScoreKeeper : MonoBehaviour {

    private int bestScore = 0;

    public void TryUpdateBestScore(int newBestScore) {
        if (newBestScore > bestScore)
            bestScore = newBestScore;
    }

    public void UpdateBestScoreDisplay() {
        //have to find the best score text component every time because when the scene reloads, references to the component are broken since a new one is made
        GameObject.Find("BestScoreText").GetComponent<Text>().text = "BEST SCORE: " + bestScore;
    }

    private void Awake() {
        //allows this to persist through level reloads and remember the best score since application launch
        DontDestroyOnLoad(this);
        //avoid duplicates
        if (FindObjectsOfType(GetType()).Length > 1) {
            Destroy(gameObject);
        }
    }
}
