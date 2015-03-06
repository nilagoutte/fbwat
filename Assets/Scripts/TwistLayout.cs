using UnityEngine;

public class TwistLayout {

    //maxProgress specifies the time at which the spiral twist stabilises into a circle
    private float maxProgress = 20f;

    //deltaHeight: relative height inside the spiral corridor. 0f is exactly in the middle
    public Vector2 GetCoord(float progress, float deltaHeight = 0f) {
        float x = Mathf.Cos(progress) * (TwistControl(progress) + deltaHeight);
        float y = Mathf.Sin(progress) * (TwistControl(progress) + deltaHeight);
        return new Vector2(x, y);
    }

    //returns the progress, capped at maxProgress
    private float TwistControl(float progress) {
        if (progress < maxProgress) return progress;
        return maxProgress;
    }

}