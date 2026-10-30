using Godot;

[GlobalClass]
public partial class Background : BlitEffectRect {
    private float _transition_value;

    private const int DEFAULT_TRANSITON_PRIORITY = 0, DEFAULT_BLUR_PRIORITY = 1;
    private int _transition_effect, _blur_effect;

    private bool _default_effects_enabled = true;

    public void SetDefaultEffectsVisible(bool is_visible) {
        _default_effects_enabled = is_visible;

        SetEffectVisible(_transition_effect, _default_effects_enabled);
        SetEffectVisible(_blur_effect, _default_effects_enabled);
    }

    public bool GetDefaultEffectsVisible() {
        return _default_effects_enabled;
    }

    public override void _Ready() {
        base._Ready();

        BlitEffect[] default_effects = CreateDefaultEffects();
        
        _transition_effect = AppendEffect(default_effects[DEFAULT_TRANSITON_PRIORITY], DEFAULT_TRANSITON_PRIORITY, _default_effects_enabled);
        _blur_effect = AppendEffect(default_effects[DEFAULT_BLUR_PRIORITY], DEFAULT_BLUR_PRIORITY, _default_effects_enabled);
    }

    public override void _Process(double delta)
    {
        if (_default_effects_enabled) {
            if(!Mathf.IsEqualApprox(_transition_value, 1.0f)) {
                _transition_value = Helper.Smooth(_transition_value, 1.0f, 4.0f);

                ((TransitionBlitEffect)GetEffect(_transition_effect)).t_value = _transition_value;

                QueueRedrawBlits();
            }
        }

        base._Process(delta);
    }

    public BlitEffect[] CreateDefaultEffects() {
        BlitEffect[] effects = new BlitEffect[2];

        effects[DEFAULT_TRANSITON_PRIORITY] = new TransitionBlitEffect{
            t_value = 0.0f,
            last_texture = null,
            new_texture = null
        };
        
        effects[DEFAULT_BLUR_PRIORITY] = new BlurBlitEffect();

        return effects;
    }

    public void SetDefaultBackgroundTexture(Texture2D new_texture) {
        TransitionBlitEffect transition_blit_effect = (TransitionBlitEffect)GetEffect(_transition_effect);

        Texture2D last_texture = transition_blit_effect.new_texture;

        transition_blit_effect.new_texture = new_texture;
        transition_blit_effect.last_texture = last_texture;

        _transition_value = 0.0f;

        QueueRedraw();
    }
}