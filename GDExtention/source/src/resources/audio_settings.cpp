#include "audio_settings.h"

#include <godot_cpp/classes/audio_server.hpp>
#include <godot_cpp/classes/audio_effect_reverb.hpp>
#include <godot_cpp/classes/audio_effect_eq.hpp>

#include "constants.h"

using namespace godot;

AudioSettings::AudioSettings() {
    room_size = 0.0f;
    damping = 0.5f;
    spread = 1.0f;
    high_pass = 0.0f;
    dry = 1.0f;
    wet = 0.0f;

    hz_32 = 0.0f;
    hz_100 = 0.0f;
    hz_320 = 0.0f;
    hz_1000 = 0.0f;
    hz_3200 = 0.0f;
    hz_10000 = 0.0f;
}

void AudioSettings::apply_settings() {
    Ref<AudioEffectReverb> reverb = AudioServer::get_singleton()->get_bus_effect(0, AudioEffects::REVERB);

    reverb->set_room_size(room_size);
    reverb->set_damping(damping);
    reverb->set_spread(spread);
    reverb->set_hpf(high_pass);
    reverb->set_dry(dry);
    reverb->set_wet(wet);

    Ref<AudioEffectEQ> eq = AudioServer::get_singleton()->get_bus_effect(0, AudioEffects::EQ);

    eq->set_band_gain_db(0, hz_32);
    eq->set_band_gain_db(1, hz_100);
    eq->set_band_gain_db(2, hz_320);
    eq->set_band_gain_db(3, hz_1000);
    eq->set_band_gain_db(4, hz_3200);
    eq->set_band_gain_db(5, hz_10000);
}