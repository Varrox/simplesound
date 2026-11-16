#pragma once 

#include "resources/settings.h"

namespace godot {
    class ApplicationSettings : public Settings {
    public:
        ApplicationSettings();
        ~ApplicationSettings();

        void apply_settings() override;
        
        void write_json() override;
        void update_property(const String& p_property) override;
    };
};