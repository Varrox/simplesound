#pragma once

#include <godot_cpp/variant/string.hpp>
#include <godot_cpp/classes/image_texture.hpp>

using namespace godot;

String seconds_to_timestamp(float p_time);

String get_media_title(const String& p_file_path);
Ref<Texture2D> get_media_cover(const String& p_file_path);

Ref<Texture2D> load_image_from_file(const String& p_file_path);

bool is_valid_audio_file(const String& p_file_path);
bool is_valid_audio_format(const String& p_file_path);

String get_proper_file_name(const String& p_text);

Vector<String> get_filters(Vector<String> p_file_extentions);
