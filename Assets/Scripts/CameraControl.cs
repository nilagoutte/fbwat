using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

    public Game game;
    //when stopMoving is true (which should be only when the player dies), the camera will stop moving along the corridor
    private bool stopMoving = false;
    //remember original camera position to restore after the random screen shaking
    private Vector3 originalCameraPos;

    //stops following the corridor and shakes the screen for the death impact
    public void LaunchDeathCamera() {
        stopMoving = true;
        originalCameraPos = transform.position;
        StartCoroutine(ScreenShake(Time.timeSinceLevelLoad, 0.25f));
    }

    //coroutine to shake the screen
    private IEnumerator ScreenShake(float shakeStartTime, float duration) {
        while (Time.timeSinceLevelLoad - shakeStartTime <= duration) {
            float xAmount = Random.Range(-0.25f, 0.25f);
            float yAmount = Random.Range(-0.25f, 0.25f);
            transform.position += new Vector3(xAmount, yAmount, 0);
            yield return null;
        }
        transform.position = originalCameraPos;
    }

    private void FixedUpdate() {
        if (!stopMoving) {
            Vector2 pos = game.currentLayout.GetCoord(Time.timeSinceLevelLoad - game.levelConstructionDelay);
            transform.position = new Vector3(pos.x, pos.y, transform.position.z);
        }
    }
}
