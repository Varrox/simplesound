using Godot;

public class AudioSettings
{
    // Reverb

	public float room_size = 0.0f;
    public float damping = 0.5f;
    public float spread = 1.0f;
    public float high_pass = 0.0f;
    public float dry = 1.0f;
    public float wet = 0.0f;

    // EQ

    public float hz_32 = 0.0f;
    public float hz_100 = 0.0f;
    public float hz_320 = 0.0f;
    public float hz_1000 = 0.0f;
    public float hz_3200 = 0.0f;
    public float hz_10000 = 0.0f;

    public AudioSettings() {}

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
}