using System.Collections.Generic;
using Godot;

public record ResourceManager {
    private Dictionary<StringName, Resource> resources;

    public Resource GetResource(StringName resource_name) {
        if (resources == null || !resources.ContainsKey(resource_name)) return null;
        return resources[resource_name];
    }

    public PackedScene GetSceneResource(StringName resource_name) {
        if (resources == null || !resources.ContainsKey(resource_name) || resources[resource_name] is not PackedScene) return null;
        return resources[resource_name] as PackedScene;
    }

    public Shader GetShaderResource(StringName resource_name) {
        if (resources == null || !resources.ContainsKey(resource_name) || resources[resource_name] is not Shader) return null;
        return resources[resource_name] as Shader;
    }

    public void SetResource(StringName name, Resource resource) {
        resources.Add(name, resource);
    }

    public ResourceManager() {
        resources = new Dictionary<StringName, Resource>();
    }
}