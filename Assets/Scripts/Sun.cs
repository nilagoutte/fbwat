using UnityEngine;

public class Sun : MonoBehaviour {

    void FixedUpdate() {
        //the sun continually rotates with the progress of the level (in the current implementation, time is the angle/progress)
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, Time.timeSinceLevelLoad * Mathf.Rad2Deg));
    }
}
