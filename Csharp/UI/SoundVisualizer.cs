using Godot;
using Godot.Collections;

public partial class SoundVisualizer : Control {
    [Export] int bar_count = 6;
    [Export] float max_height = 4 * 7, min_height = 4;

    [Export] Array<Control> bars;

    public const int MAX_FREQ = 700;

    private static AudioEffectSpectrumAnalyzerInstance _spectrum_instance;

    public override void _Ready() {
        if (_spectrum_instance == null) {
            _spectrum_instance = (AudioEffectSpectrumAnalyzerInstance)AudioServer.GetBusEffectInstance(0, Constants.SPECTRUM_ANALIZER_IDX);
        }
    }

    public override void _Process(double delta) {
        if (_spectrum_instance == null || !Visible) return;
        
        for (int i = 0; i < bar_count; i++) {
            float freq_start = (float)(i * MAX_FREQ) / bar_count;
            float freq_end = (float)((i + 1) * MAX_FREQ) / bar_count;

            Vector2 magnitude_stereo = _spectrum_instance.GetMagnitudeForFrequencyRange(freq_start, freq_end);
            
            float magnitude = Mathf.Lerp(magnitude_stereo.X, magnitude_stereo.Y, 0.5f) * ((i + 1) * 0.5f);
            float height = (magnitude * 75.0f) / Main.instance.audio_player.VolumeLinear;
            
            bars[i].Size = new Vector2(bars[i].Size.X, Helper.Smooth(bars[i].Size.Y, Mathf.Min(min_height + height, max_height), 40));
            bars[i].Position = new Vector2(bars[i].Position.X, -bars[i].Size.Y / 2);
        }
    }
}
