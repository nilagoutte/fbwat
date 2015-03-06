//Copyright Nicolas Lagoutte 2015
using UnityEngine;

public class MusicPlayer : MonoBehaviour {

    private void Awake() {
        DontDestroyOnLoad(gameObject);
        //avoid duplicates
        if (FindObjectsOfType(GetType()).Length > 1) {
            Destroy(gameObject);
        }
    }
}
