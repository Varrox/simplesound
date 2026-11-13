#include "tools.h"

#include <godot_cpp/classes/file_access.hpp>

#include "constants.h"

using namespace godot;

String seconds_to_timestamp(float p_time) {
    int total_seconds = (int)(p_time + 0.5f);
    int total_mins = total_seconds / 60;
    bool hours = total_seconds / 3600 != 0;

    String output = "";

    if(hours) output += String::num(total_mins / 60) + ":";

    int mins = total_mins % 60;
    int seconds = total_seconds % 60;

    output += (mins < 10 ? "0" : "") + String::num(mins);
    output += ":";
    output += (seconds < 10 ? "0" : "") + String::num(seconds);

    return output;
}

/*String get_media_title(const String& p_file_path) {
    String* meta_name = Metadata.GetName(p_file_path);
    return meta_name == nullptr ? p_file_path.get_basename().right(p_file_path.get_base_dir().length()) : *meta_name;
}*/

/*Ref<Texture2D> get_media_cover(const String& p_file_path) {
    byte[] picture_data = Metadata.GetCover(p_file_path, out string type);
    
    if (picture_data == null) return nullptr;

    Ref<Image> image = memnew(Image);
    Error error = Error::FAILED;

    if(type == "image/jpg" || type == "image/jpeg") {
        error = image->load_jpg_from_buffer(picture_data);
    } else if (type == "image/png") {
        error = image->load_png_from_buffer(picture_data);
    } else if (type == "image/webp") {
        error = image->load_webp_from_buffer(picture_data);
    } else if (type == "image/bmp") {
        error = image->load_bmp_from_buffer(picture_data);
    } else if (type == "image/svg+xml") {
        error = image->load_svg_from_buffer(picture_data);
    } else if (type == "image/vnd.ms.dds") {
        error = image->load_dds_from_buffer(picture_data);
    } else if (type == "image/x-targa" || type == "image/x-tga") {
        error = image->load_tga_from_buffer(picture_data);
    }

    return error == Error::OK ? ImageTexture::create_from_image(image) : nullptr;
}*/

Ref<Texture2D> load_image_from_file(const String& p_file_path) {
    if (p_file_path.is_empty() || !FileAccess::file_exists(p_file_path)) return nullptr;
    Ref<Image> image = memnew(Image);
    if (image->load(p_file_path) == Error::OK) return ImageTexture::create_from_image(image);
    return nullptr;
}

bool is_valid_audio_format(const String& p_file_path) {
    for(const String& extention : PLAYABLE_FORMATS) {
        if(p_file_path.get_extension() == extention) return true;
    }

    return false;
}

bool is_valid_audio_file(const String& p_file_path) {
    if(!FileAccess::file_exists(p_file_path)) return false;

    return is_valid_audio_format(p_file_path);
}

String get_proper_file_name(const String& p_text) {
    return p_text.replace_char('\\', '-').replace_char('/', '-').replace_char(':', '-');
}

Vector<String> get_filters(Vector<String> p_file_extentions) {
    for(String& extention : p_file_extentions) {
        extention.insert(0, String("*."));
    }

    return p_file_extentions;
}