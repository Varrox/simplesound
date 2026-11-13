#pragma once 

#include <godot_cpp/classes/scene_tree.hpp>
#include <godot_cpp/classes/theme.hpp>

namespace godot {

class ApplicationManager : public SceneTree {
    GDCLASS(ApplicationManager, SceneTree);

private:
    static int _last_windows_focused;
    static int _windows_focused;

    static Vector<Ref<Window>> _windows;

    Ref<Theme> _load_theme();

    const StringName _THEME_TYPE = StringName("GUI");

protected:
	static void _bind_methods();

public:
    ApplicationManager();
    ~ApplicationManager();

    void _initialize() override;
    void _finalize() override;

    bool _process(double delta) override;

    static Ref<Theme> theme;
    static ApplicationManager* singleton;
    static Ref<Window> currently_focused_window;

    static void save();
    static void quit_program();

    static void add_window(const Ref<Window>& p_window);
    static void remove_window(const Ref<Window>& p_window);
    static bool is_any_window_focused();
};

};