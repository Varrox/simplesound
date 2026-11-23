#pragma once 

#include <godot_cpp/classes/scene_tree.hpp>
#include <godot_cpp/classes/theme.hpp>

#include "resources/resource_manager.h"

namespace godot {

class ApplicationManager : public SceneTree {
    GDCLASS(ApplicationManager, SceneTree);

private:
    int32_t _last_windows_focused;
    int32_t _windows_focused;

    PackedInt32Array _windows;
    int32_t _currently_focused_window;

    Ref<Theme> _theme;
    Ref<ResourceManager> _resource_manager;
    
    Ref<Theme> _load_theme();
    Ref<ResourceManager> _load_resource_manager();
    
    static ApplicationManager* _singleton;
    
    const StringName _THEME_TYPE = StringName("GUI");

protected:
	static void _bind_methods();

public:
    ApplicationManager();
    ~ApplicationManager();

    void _initialize() override;
    void _finalize() override;

    bool _process(double delta) override;
    
    Ref<Theme> get_theme();
    Ref<ResourceManager> get_resource_manager();

    int32_t get_currently_focused_window();
    
    void save();
    void quit_program();
    
    void add_window(const int32_t p_window);
    void remove_window(const int32_t p_window);
    bool is_any_window_focused();

    Ref<PackedScene> get_scene_resource(const StringName& p_name);
    Ref<Shader> get_shader_resource(const StringName& p_name);

    Color get_theme_color(const StringName& p_name);
    int get_theme_constant(const StringName& p_name);
    Ref<Font> get_theme_font(const StringName& p_name);
    int get_theme_font_size(const StringName& p_name);
    Ref<Texture2D> get_theme_icon(const StringName& p_name);
    Ref<StyleBox> get_theme_stylebox(const StringName& p_name);

    static ApplicationManager* get_singleton();
};
};