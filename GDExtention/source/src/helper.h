#pragma once

namespace godot {
    double get_smooth_value(double s, double dt);
        
    // lerp smoothing, but not fps dependent
    double smooth(double a, double b, double s, double dt);
}