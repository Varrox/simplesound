using Godot;

public struct Helper {
    public static float GetSmoothValue(float s, float delta) {
        return Mathf.Clamp(1.0f - Mathf.Exp(-delta * s), 0.0f, 1.0f);
    }
        
    public static float Smooth(float a, float b, float s) { // lerp smoothing, but not fps dependent
        return Mathf.Lerp(a, b, GetSmoothValue(s, GetProcessDeltaTime()));
    }

    public static float GetProcessDeltaTime() {
        return (float)ApplicationManager.instance.Root.GetProcessDeltaTime();
    }
}