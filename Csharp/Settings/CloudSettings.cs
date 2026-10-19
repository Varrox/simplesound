using System.Linq;
using System.Text.Json;

public class CloudSettings : ISettings {
    public bool sync_application_settings;

    // SORT THESE BY AGE. DO NOT REMOVE OLD NAMES UNTIL ALPHA IS OVER, OR ALPHA v1.4.

    public static readonly string[] PROPERTY_SYNC_APPLICATION_SETTINGS = ["sync_application_settings"];

    public CloudSettings() {
        sync_application_settings = false;
    }

    public void ApplySettings() {
        
    }

    public void WriteJson(in Utf8JsonWriter writer) {
        writer.WriteBoolean(PROPERTY_SYNC_APPLICATION_SETTINGS[0], sync_application_settings);
    }

    public void UpdateProperty(string property_name, ref Utf8JsonReader reader) {
        if (PROPERTY_SYNC_APPLICATION_SETTINGS.Contains(property_name)) {
            if (reader.TokenType == JsonTokenType.True || reader.TokenType == JsonTokenType.False) sync_application_settings = reader.GetBoolean();
        }
        
        reader.Skip();
    }
}