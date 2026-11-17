#include "graphics_settings.h"

#include <godot_cpp/classes/project_settings.hpp>
#include <godot_cpp/classes/rendering_server.hpp>
#include <godot_cpp/classes/display_server.hpp>
#include <godot_cpp/classes/engine.hpp>

using namespace godot;

GraphicsSettings::GraphicsSettings() {
    blur_quality = QualityLevel::MEDIUM;

    vsync = true;
    reduce_fps_on_lose_focus = true;
    max_fps = 60;

    int width = ProjectSettings::get_singleton()->get_setting("display/window/size/viewport_width");
    int height = ProjectSettings::get_singleton()->get_setting("display/window/size/viewport_height");

    main_display_size = Vector2i(width, height);
}

GraphicsSettings::~GraphicsSettings() {}

void GraphicsSettings::apply_settings() {
    RenderingServer::get_singleton()->global_shader_parameter_set(StringName("blur_quality"), blur_quality + 1);

    DisplayServer::get_singleton()->window_set_vsync_mode(DisplayServer::VSyncMode(vsync));
    
    Engine::get_singleton()->set_max_fps(max_fps);
}