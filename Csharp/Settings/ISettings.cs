using System.Text.Json;

public interface ISettings {
    public void ApplySettings();

    public void WriteJson(in Utf8JsonWriter writer);

    public void UpdateProperty(string property_name, ref Utf8JsonReader reader);
}