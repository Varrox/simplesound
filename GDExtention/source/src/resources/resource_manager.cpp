#include "resource_manager.h"

using namespace godot;

void ResourceManager::_bind_methods() {
    
}

ResourceManager::ResourceManager() {

}

ResourceManager::~ResourceManager() {

}


Ref<PackedScene> ResourceManager::get_scene(const StringName& p_resource_name) {
    if (!_scene_resources.has(p_resource_name)) return nullptr;
    return _scene_resources[p_resource_name];
}

Ref<Shader> ResourceManager::get_shader(const StringName& p_resource_name) {
    if (!_shader_resources.has(p_resource_name)) return nullptr;
    return _shader_resources[p_resource_name];
}


void ResourceManager::set_scene(const StringName& p_resource_name, const Ref<PackedScene>& p_resource) {
    _scene_resources[p_resource_name] = p_resource;
}

void ResourceManager::set_shader(const StringName& p_resource_name, const Ref<Shader>& p_resource) {
    _shader_resources[p_resource_name] = p_resource;
}
