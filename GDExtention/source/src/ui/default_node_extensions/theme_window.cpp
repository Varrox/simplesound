#include "theme_window.h"

#include "application/application_manager.h"

using namespace godot;

void ThemeWindow::_bind_methods() {
    ADD_SIGNAL(MethodInfo("on_close"));
}

ThemeWindow::ThemeWindow() {
    ApplicationManager::get_singleton()->add_window(get_window_id());
    
    set_theme(ApplicationManager::get_singleton()->get_theme());
}

ThemeWindow::~ThemeWindow() {
    ApplicationManager::get_singleton()->remove_window(get_window_id());
}