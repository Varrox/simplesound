#include "application_manager.h"
#include <godot_cpp/core/class_db.hpp>

#include <godot_cpp/classes/control.hpp>
#include <godot_cpp/classes/dpi_texture.hpp>
#include <godot_cpp/classes/resource_loader.hpp>
#include <godot_cpp/classes/window.hpp>

using namespace godot;

void ApplicationManager::_bind_methods() {
}

ApplicationManager::ApplicationManager() {
    _last_windows_focused = 0;
    _windows_focused = 0;

    singleton = this;
    

    currently_focused_window = Ref(get_root());
    add_window(currently_focused_window);
}

ApplicationManager::~ApplicationManager() {
}

void ApplicationManager::_initialize() {
    Node *current_scene_node = get_current_scene();
    
    if(current_scene_node) {
        static_cast<Control *>(current_scene_node)->set_theme(theme);
    }
}

void ApplicationManager::_finalize() {
    save();
}

bool ApplicationManager::_process(double delta) {
    return false;
}

Ref<Theme> ApplicationManager::_load_theme() {
    Ref<Theme> theme_resource = memnew(Theme);

    theme_resource->set_color(StringName("selected_font_color"), _THEME_TYPE, Color(1.0, 1.0, 1.0, 1));
    theme_resource->set_color("unselected_font_color", _THEME_TYPE, Color(0.74509805, 0.74509805, 0.74509805, 1));

    theme_resource->set_color("normal_font_color", _THEME_TYPE, Color(1.0, 1.0, 1.0, 1));
    theme_resource->set_color("small_font_color", _THEME_TYPE, Color(0.6627451, 0.6627451, 0.6627451, 1));

    theme_resource->set_color("disabled_font_color", _THEME_TYPE, Color(0.4117647, 0.4117647, 0.4117647, 1));
    theme_resource->set_color("enabled_font_color", _THEME_TYPE, Color(0, 1, 0, 1));

    theme_resource->set_color("playing_font_color", _THEME_TYPE, Color::html("66ff5e"));

    theme_resource->set_color("highlight_color", _THEME_TYPE, Color::html("47474796"));

    theme_resource->set_color("icon_pressed_color", _THEME_TYPE, Color(0.4117647, 0.4117647, 0.4117647, 1));
    theme_resource->set_color("icon_hover_color", _THEME_TYPE, Color(0.827451, 0.827451, 0.827451, 1));

    // Constant

    theme_resource->set_constant("context_menu_margin_constant", _THEME_TYPE, 5);

    theme_resource->set_constant("context_button_icon_max_width_small_constant", _THEME_TYPE, 20);

    // Font

    // Font size

    // Icon

    theme_resource->set_icon("play_icon", _THEME_TYPE, static_cast<Ref<DPITexture>>(ResourceLoader::get_singleton()->load("res://Icons/SVG/play.svg", "DPITexture")));
    theme_resource->set_icon("pause_icon", _THEME_TYPE, static_cast<Ref<DPITexture>>(ResourceLoader::get_singleton()->load("res://Icons/SVG/pause.svg", "DPITexture")));
    theme_resource->set_icon("stop_icon", _THEME_TYPE, static_cast<Ref<DPITexture>>(ResourceLoader::get_singleton()->load("res://Icons/SVG/stop.svg", "DPITexture")));

    theme_resource->set_icon("no_play_icon", _THEME_TYPE, static_cast<Ref<DPITexture>>(ResourceLoader::get_singleton()->load("res://Icons/SVG/no_play.svg", "DPITexture")));

    theme_resource->set_icon("loop_icon", _THEME_TYPE, static_cast<Ref<DPITexture>>(ResourceLoader::get_singleton()->load("res://Icons/SVG/loop.svg", "DPITexture")));
    theme_resource->set_icon("shuffle_icon", _THEME_TYPE, static_cast<Ref<DPITexture>>(ResourceLoader::get_singleton()->load("res://Icons/SVG/shuffle.svg", "DPITexture")));

    theme_resource->set_icon("mute_icon", _THEME_TYPE, static_cast<Ref<DPITexture>>(ResourceLoader::get_singleton()->load("res://Icons/SVG/un_muted.svg", "DPITexture")));
    theme_resource->set_icon("unmute_icon", _THEME_TYPE, static_cast<Ref<DPITexture>>(ResourceLoader::get_singleton()->load("res://Icons/SVG/muted.svg", "DPITexture")));

    theme_resource->set_icon("forward_icon", _THEME_TYPE, static_cast<Ref<DPITexture>>(ResourceLoader::get_singleton()->load("res://Icons/SVG/forward.svg", "DPITexture")));
    theme_resource->set_icon("back_icon", _THEME_TYPE, static_cast<Ref<DPITexture>>(ResourceLoader::get_singleton()->load("res://Icons/SVG/back.svg", "DPITexture")));

    theme_resource->set_icon("forward_arrow_icon", _THEME_TYPE, static_cast<Ref<DPITexture>>(ResourceLoader::get_singleton()->load("res://Icons/SVG/forward_arrow.svg", "DPITexture")));
    theme_resource->set_icon("back_arrow_icon", _THEME_TYPE, static_cast<Ref<DPITexture>>(ResourceLoader::get_singleton()->load("res://Icons/SVG/back_arrow.svg", "DPITexture")));

    theme_resource->set_icon("playlist_icon", _THEME_TYPE, static_cast<Ref<DPITexture>>(ResourceLoader::get_singleton()->load("res://Icons/SVG/playlist.svg", "DPITexture")));

    theme_resource->set_icon("hamburger_icon", _THEME_TYPE, static_cast<Ref<DPITexture>>(ResourceLoader::get_singleton()->load("res://Icons/SVG/hamburger.svg", "DPITexture")));
    theme_resource->set_icon("meatballs_icon", _THEME_TYPE, static_cast<Ref<DPITexture>>(ResourceLoader::get_singleton()->load("res://Icons/SVG/meatballs.svg", "DPITexture")));

    theme_resource->set_icon("search_icon", _THEME_TYPE, static_cast<Ref<DPITexture>>(ResourceLoader::get_singleton()->load("res://Icons/SVG/search.svg", "DPITexture")));

    theme_resource->set_icon("plus_icon", _THEME_TYPE, static_cast<Ref<DPITexture>>(ResourceLoader::get_singleton()->load("res://Icons/SVG/plus.svg", "DPITexture")));
    theme_resource->set_icon("minus_icon", _THEME_TYPE, static_cast<Ref<DPITexture>>(ResourceLoader::get_singleton()->load("res://Icons/SVG/minus.svg", "DPITexture")));

    theme_resource->set_icon("x_icon", _THEME_TYPE, static_cast<Ref<DPITexture>>(ResourceLoader::get_singleton()->load("res://Icons/SVG/x.svg", "DPITexture")));

    theme_resource->set_icon("download_icon", _THEME_TYPE, static_cast<Ref<DPITexture>>(ResourceLoader::get_singleton()->load("res://Icons/SVG/download.svg", "DPITexture")));
    theme_resource->set_icon("upload_icon", _THEME_TYPE, static_cast<Ref<DPITexture>>(ResourceLoader::get_singleton()->load("res://Icons/SVG/upload.svg", "DPITexture")));

    theme_resource->set_icon("reload_icon", _THEME_TYPE, static_cast<Ref<DPITexture>>(ResourceLoader::get_singleton()->load("res://Icons/SVG/reload.svg", "DPITexture")));

    theme_resource->set_icon("cog_icon", _THEME_TYPE, static_cast<Ref<DPITexture>>(ResourceLoader::get_singleton()->load("res://Icons/SVG/cog.svg", "DPITexture")));
    theme_resource->set_icon("pen_icon", _THEME_TYPE, static_cast<Ref<DPITexture>>(ResourceLoader::get_singleton()->load("res://Icons/SVG/pen.svg", "DPITexture")));

    theme_resource->set_icon("time_icon", _THEME_TYPE, static_cast<Ref<DPITexture>>(ResourceLoader::get_singleton()->load("res://Icons/SVG/time.svg", "DPITexture")));

    theme_resource->set_icon("folder_icon", _THEME_TYPE, static_cast<Ref<DPITexture>>(ResourceLoader::get_singleton()->load("res://Icons/SVG/folder.svg", "DPITexture")));
    theme_resource->set_icon("file_icon", _THEME_TYPE, static_cast<Ref<DPITexture>>(ResourceLoader::get_singleton()->load("res://Icons/SVG/file.svg", "DPITexture")));
    theme_resource->set_icon("zip_icon", _THEME_TYPE, static_cast<Ref<DPITexture>>(ResourceLoader::get_singleton()->load("res://Icons/SVG/zip.svg", "DPITexture")));

    theme_resource->set_icon("default_cover_icon", _THEME_TYPE, static_cast<Ref<Texture2D>>(ResourceLoader::get_singleton()->load("res://Icons/Default Covers/DefaultCover.png", "Texture2D")));

    // Stylebox

    theme_resource->set_stylebox("color_picker_panel_stylebox", _THEME_TYPE, static_cast<Ref<StyleBox>>(ResourceLoader::get_singleton()->load("res://Styling/Color Picker/color_picker_panel.tres", "StyleBox")));

    theme_resource->set_stylebox("radio_normal_stylebox", _THEME_TYPE, static_cast<Ref<StyleBox>>(ResourceLoader::get_singleton()->load("res://Styling/Selectables/radio_normal.tres", "StyleBox")));
    theme_resource->set_stylebox("radio_hover_stylebox", _THEME_TYPE, static_cast<Ref<StyleBox>>(ResourceLoader::get_singleton()->load("res://Styling/Selectables/radio_hover.tres", "StyleBox")));
    theme_resource->set_stylebox("radio_pressed_stylebox", _THEME_TYPE, static_cast<Ref<StyleBox>>(ResourceLoader::get_singleton()->load("res://Styling/Selectables/radio_pressed.tres", "StyleBox")));
    theme_resource->set_stylebox("radio_disabled_stylebox", _THEME_TYPE, static_cast<Ref<StyleBox>>(ResourceLoader::get_singleton()->load("res://Styling/Selectables/radio_disabled.tres", "StyleBox")));

    theme_resource->set_stylebox("radio_selected_normal_stylebox", _THEME_TYPE, static_cast<Ref<StyleBox>>(ResourceLoader::get_singleton()->load("res://Styling/Selectables/Selected/radio_selected_normal.tres", "StyleBox")));
    theme_resource->set_stylebox("radio_selected_hover_stylebox", _THEME_TYPE, static_cast<Ref<StyleBox>>(ResourceLoader::get_singleton()->load("res://Styling/Selectables/Selected/radio_selected_hover.tres", "StyleBox")));
    theme_resource->set_stylebox("radio_selected_pressed_stylebox", _THEME_TYPE, static_cast<Ref<StyleBox>>(ResourceLoader::get_singleton()->load("res://Styling/Selectables/Selected/radio_selected_pressed.tres", "StyleBox")));
    theme_resource->set_stylebox("radio_selected_disabled_stylebox", _THEME_TYPE, static_cast<Ref<StyleBox>>(ResourceLoader::get_singleton()->load("res://Styling/Selectables/Selected/radio_selected_disabled.tres", "StyleBox")));

    theme_resource->set_stylebox("file_drop_normal_stylebox", _THEME_TYPE, static_cast<Ref<StyleBox>>(ResourceLoader::get_singleton()->load("res://Styling/Filedrop/file_drop_normal.tres", "StyleBox")));

    theme_resource->set_stylebox("context_menu_panel_stylebox", _THEME_TYPE, static_cast<Ref<StyleBox>>(ResourceLoader::get_singleton()->load("res://Styling/context_panel.tres", "StyleBox")));
    
    return theme_resource;
}

void ApplicationManager::save() {

}

void ApplicationManager::quit_program() {
    if (!instance.is_null()) {
        instance->quit();
    }
}

void ApplicationManager::add_window(const Ref<Window>& p_window) {
    if (_windows.has(p_window)) {
        return;
    }

    _windows.append(p_window);

    
}

void ApplicationManager::remove_window(const Ref<Window>& p_window) {
    if (!_windows.has(p_window)) {
        return;
    }

    for(const Ref<Window>& window : _windows){
        if (window == p_window) {
            int idx = _windows.find(window);
            _windows.remove_at(idx);
            break;
        }
    }
}

bool ApplicationManager::is_any_window_focused() {
    for(const Ref<Window>& window : _windows){
        if (window == nullptr) continue;;
        if(window->has_focus()) {
            return true;
        }
    }

    return false;
}