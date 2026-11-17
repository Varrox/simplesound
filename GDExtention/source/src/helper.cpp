#include "helper.h"

#include "application/application_manager.h"

#include <godot_cpp/core/math.hpp>

using namespace godot;

double get_smooth_value(double s, double dt) {
    return CLAMP(1.0 - Math::exp(-dt * s), 0.0, 1.0);
}
        
double smooth(double a, double b, double s, double dt) { // lerp smoothing, but not fps dependent
    return Math::lerp(a, b, godot::get_smooth_value(s, dt));
}