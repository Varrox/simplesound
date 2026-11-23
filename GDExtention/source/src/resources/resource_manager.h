#pragma once 

#include <godot_cpp/classes/resource.hpp>
#include <godot_cpp/classes/packed_scene.hpp>
#include <godot_cpp/classes/shader.hpp>

#include <godot_cpp/variant/typed_dictionary.hpp>

namespace godot {
    class ResourceManager : public Resource {
    private:
        GDCLASS(ResourceManager, Resource);

        TypedDictionary<StringName, Ref<PackedScene>> _scene_resources;
        TypedDictionary<StringName, Ref<Shader>> _shader_resources;
    protected:
        static void _bind_methods();
    public:
        ResourceManager();
        ~ResourceManager();

        Ref<PackedScene> get_scene(const StringName& p_resource_name);
        Ref<Shader> get_shader(const StringName& p_resource_name);

        void set_scene(const StringName& p_resource_name, const Ref<PackedScene>& p_resource);
        void set_shader(const StringName& p_resource_name, const Ref<Shader>& p_resource);
    };
};