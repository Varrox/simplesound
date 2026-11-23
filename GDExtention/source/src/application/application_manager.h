#pragma once 

#include <godot_cpp/classes/scene_tree.hpp>
#include <godot_cpp/classes/theme.hpp>

namespace godot {

class ApplicationManager : public SceneTree {
    GDCLASS(ApplicationManager, SceneTree);

private:
    int32_t _last_windows_focused;
    int32_t _windows_focused;

    PackedInt32Array _windows;
    int32_t _currently_focused_window;

    Ref<Theme> _theme;
    
    Ref<Theme> _load_theme();
    
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
    int32_t get_currently_focused_window();
    
    void save();
    void quit_program();
    
    void add_window(const int32_t p_window);
    void remove_window(const int32_t p_window);
    bool is_any_window_focused();

    static ApplicationManager* get_singleton();
};
};