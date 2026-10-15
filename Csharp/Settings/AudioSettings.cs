using Godot;
using System.Linq;
using System.Text.Json;

public class AudioSettings : ISettings
{
    // Reverb

	public float room_size;
    public float damping;
    public float spread;
    public float high_pass;
    public float dry;
    public float wet;

    // EQ

    public float hz_32;
    public float hz_100;
    public float hz_320;
    public float hz_1000;
    public float hz_3200;
    public float hz_10000;

    // SORT THESE BY AGE. DO NOT REMOVE OLD NAMES UNTIL ALPHA IS OVER, OR ALPHA v1.4.

	public static readonly string[] PROPERTY_ROOM_SIZE = ["room_size"];
    public static readonly string[] PROPERTY_DAMPING = ["damping"];
    public static readonly string[] PROPERTY_SPREAD = ["spread"];
    public static readonly string[] PROPERTY_HIGH_PASS = ["high_pass"];
    public static readonly string[] PROPERTY_DRY = ["dry"];
    public static readonly string[] PROPERTY_WET = ["wet"];

    public static readonly string[] PROPERTY_HZ_32 = ["hz_32"];
    public static readonly string[] PROPERTY_HZ_100 = ["hz_100"];
    public static readonly string[] PROPERTY_HZ_320 = ["hz_320"];
    public static readonly string[] PROPERTY_HZ_1000 = ["hz_1000"];
    public static readonly string[] PROPERTY_HZ_3200 = ["hz_3200"];
    public static readonly string[] PROPERTY_HZ_10000 = ["hz_10000"];

    public AudioSettings() {
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

	public void ApplySettings() {
		AudioEffectReverb reverb = AudioServer.GetBusEffect(0, Constants.REVERB_IDX) as AudioEffectReverb;

        reverb.RoomSize = room_size;
        reverb.Damping = damping;
        reverb.Spread = spread;
        reverb.Hipass = high_pass;
        reverb.Dry = dry;
        reverb.Wet = wet;

        AudioEffectEQ eq = AudioServer.GetBusEffect(0, Constants.EQ_IDX) as AudioEffectEQ;

        eq.SetBandGainDb(0, hz_32);
        eq.SetBandGainDb(1, hz_100);
        eq.SetBandGainDb(2, hz_320);
        eq.SetBandGainDb(3, hz_1000);
        eq.SetBandGainDb(4, hz_3200);
        eq.SetBandGainDb(5, hz_10000);
	}

    public void WriteJson(in Utf8JsonWriter writer) {
        writer.WriteNumber(PROPERTY_ROOM_SIZE[0], room_size);
        writer.WriteNumber(PROPERTY_DAMPING[0], damping);
        writer.WriteNumber(PROPERTY_SPREAD[0], spread);
        writer.WriteNumber(PROPERTY_HIGH_PASS[0], high_pass);
        writer.WriteNumber(PROPERTY_DRY[0], dry);
        writer.WriteNumber(PROPERTY_WET[0], wet);

        writer.WriteNumber(PROPERTY_HZ_32[0], hz_32);
        writer.WriteNumber(PROPERTY_HZ_100[0], hz_100);
        writer.WriteNumber(PROPERTY_HZ_320[0], hz_320);
        writer.WriteNumber(PROPERTY_HZ_1000[0], hz_1000);
        writer.WriteNumber(PROPERTY_HZ_3200[0], hz_3200);
        writer.WriteNumber(PROPERTY_HZ_10000[0], hz_10000);
    }

    public void UpdateProperty(string property_name, ref Utf8JsonReader reader) {
        if (PROPERTY_ROOM_SIZE.Contains(property_name)) {
            if (reader.TokenType == JsonTokenType.String) room_size = (float)reader.GetDouble();
        }
        else if (PROPERTY_DAMPING.Contains(property_name)) {
            if (reader.TokenType == JsonTokenType.String) damping = (float)reader.GetDouble();
        }
        else if (PROPERTY_SPREAD.Contains(property_name)) {
            if (reader.TokenType == JsonTokenType.String) spread = (float)reader.GetDouble();
        }
        else if (PROPERTY_HIGH_PASS.Contains(property_name)) {
            if (reader.TokenType == JsonTokenType.String) high_pass = (float)reader.GetDouble();
        }
        else if (PROPERTY_DRY.Contains(property_name)) {
            if (reader.TokenType == JsonTokenType.String) dry = (float)reader.GetDouble();
        }
        else if (PROPERTY_WET.Contains(property_name)) {
            if (reader.TokenType == JsonTokenType.String) wet = (float)reader.GetDouble();
        }
        else if (PROPERTY_HZ_32.Contains(property_name)) {
            if (reader.TokenType == JsonTokenType.String) hz_32 = (float)reader.GetDouble();
        }
        else if (PROPERTY_HZ_100.Contains(property_name)) {
            if (reader.TokenType == JsonTokenType.String) hz_100 = (float)reader.GetDouble();
        }
        else if (PROPERTY_HZ_320.Contains(property_name)) {
            if (reader.TokenType == JsonTokenType.String) hz_320 = (float)reader.GetDouble();
        }
        else if (PROPERTY_HZ_1000.Contains(property_name)) {
            if (reader.TokenType == JsonTokenType.String) hz_1000 = (float)reader.GetDouble();
        }
        else if (PROPERTY_HZ_3200.Contains(property_name)) {
            if (reader.TokenType == JsonTokenType.String) hz_3200 = (float)reader.GetDouble();
        }
        else if (PROPERTY_HZ_10000.Contains(property_name)) {
            if (reader.TokenType == JsonTokenType.String) hz_10000 = (float)reader.GetDouble();
        }
        
        reader.Skip();
    }
}