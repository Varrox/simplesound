using Godot;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class BlitEffectRect : TextureRect {
    private Dictionary<int, BlitEffectLayer> _effect_layers;
    private DrawableTexture2D _drawable_texture;

    private bool _queued_for_redraw;

    private Vector2 _last_size;
    private bool _last_visible;

    private int[] _sorted_effects;

    public const DrawableTexture2D.DrawableFormat FORMAT = DrawableTexture2D.DrawableFormat.Rgbah;

    private record BlitEffectLayer {
        public int priority;
        public bool visible;
        public BlitEffect effect;

        public bool is_shader { get { return effect != null && effect is ShaderBlitEffect; } }

        public void TryRecompile() {
            if (is_shader) {
                (effect as ShaderBlitEffect).Recompile();
            }
        }
    }

    public override void _Ready() {
        _effect_layers = new Dictionary<int, BlitEffectLayer>();
        _drawable_texture = new DrawableTexture2D();

        TransitionBlitEffect gup = new TransitionBlitEffect
        {
            last_texture = GD.Load<Texture2D>("res://Icons/Default Covers/DefaultCover.png"),
            new_texture = GD.Load<Texture2D>("res://Icons/Logos/logochrimis.png"),
            time = 0.0f
        };

        AppendEffect(gup, 0);
        AppendEffect(new BlurBlitEffect(), 1);

        QueueRedrawBlits();

        _last_size = -Vector2.One;
        _last_visible = false;
    }

    public override void _Process(double delta) {
        if((!_last_visible || Size != _last_size) && Visible) {
            QueueRedrawBlits();

            _last_visible = Visible;
            _last_size = Size;
        }

        if (_queued_for_redraw) {
            _queued_for_redraw = false;
            RedrawBlits();
        }
    }

    public int AppendEffect(BlitEffect effect, int priority = 0, bool visible = true) {
        int idx = _effect_layers.Count;

        while (_effect_layers.ContainsKey(idx)) idx++;

        _effect_layers.Add(idx, new BlitEffectLayer { priority = priority, visible = visible, effect = effect});

        QueuePriorityResort();
        RedrawBlits();

        return idx;
    }

    public void RemoveEffect(int effect_idx) {
        if (!_effect_layers.ContainsKey(effect_idx))
            return;

        _effect_layers.Remove(effect_idx);

        QueuePriorityResort();
        RedrawBlits();
    }

    public void ClearEffects() {
        _effect_layers.Clear();
        
        QueuePriorityResort();
        RedrawBlits();
    }

    public void SetEffectVisible(int effect_idx, bool is_visible) {
        if (_effect_layers.ContainsKey(effect_idx))
            _effect_layers[effect_idx].visible = is_visible;
    }

    public void SetEffectPriority(int effect_idx, int priority) {
        if (_effect_layers.ContainsKey(effect_idx))
            _effect_layers[effect_idx].priority = priority;
    }

    private void QueuePriorityResort() {
        _sorted_effects = null;
    }

    public void QueueRedrawBlits() {
        _queued_for_redraw = true;
    }

    public void RedrawBlits() {
        Rect2I full_rect = new Rect2I(0, 0, (Vector2I)Size);

        _drawable_texture.Setup(full_rect.Size.X, full_rect.Size.Y, FORMAT);

        if (_sorted_effects == null) {
            _sorted_effects = _effect_layers.OrderBy(k => k.Value.priority).Select(v => v.Key).ToArray();
        }

        foreach(int idx in _sorted_effects) {
            if (_effect_layers[idx].visible) {
                _effect_layers[idx].effect.Blit(ref _drawable_texture, full_rect);
            }
        }

        Texture = _drawable_texture;
    }
}

public interface BlitEffect {
    public void Blit(ref DrawableTexture2D drawable_texture, Rect2I full_rect);
}

public interface ShaderBlitEffect : BlitEffect {
    public void Recompile();
}

public class ImageBlitEffect : BlitEffect {
    public Texture2D texture;

    public void Blit(ref DrawableTexture2D drawable_texture, Rect2I full_rect) {
        drawable_texture.BlitRect(full_rect, texture);
    }
}

public class TransitionBlitEffect : ShaderBlitEffect {
    private static Shader _blit_transition_shader;
    private static ShaderMaterial _blit_transition_material;

    public float time = 0.0f;

    public Texture2D last_texture, new_texture;

    public TransitionBlitEffect() {
        _blit_transition_shader = ApplicationManager.GetShaderResource("transition_blit");

        _blit_transition_material = new ShaderMaterial {
            Shader = _blit_transition_shader
        };
    }

    public void Recompile() {
        _blit_transition_shader = GD.Load<Shader>(_blit_transition_shader.ResourcePath);
    }

    public void Blit(ref DrawableTexture2D drawable_texture, Rect2I full_rect) {
        _blit_transition_material.SetShaderParameter("last_texture", last_texture);
        _blit_transition_material.SetShaderParameter("new_texture", new_texture);

        _blit_transition_material.SetShaderParameter("time", time);

        drawable_texture.BlitRectMulti(full_rect, [last_texture, new_texture], [], null, 0, _blit_transition_material);
    }
}

public class BlurBlitEffect : ShaderBlitEffect {
    private static Shader _blit_blur_shader;
    private static ShaderMaterial _blit_blur_material;

    private static DrawableTexture2D vertical_drawable_texture;

    public float blur_size = 0.25f;
    public float dim = 0.7f;

    public BlurBlitEffect() {
        _blit_blur_shader = ApplicationManager.GetShaderResource("blur_blit");

        _blit_blur_material = new ShaderMaterial {
            Shader = _blit_blur_shader
        };

        vertical_drawable_texture = new DrawableTexture2D();
    }

    public void Recompile() {
        _blit_blur_shader = GD.Load<Shader>(_blit_blur_shader.ResourcePath);
    }

    public void Blit(ref DrawableTexture2D drawable_texture, Rect2I full_rect) {
        vertical_drawable_texture.Setup(full_rect.Size.X, full_rect.Size.Y, BlitEffectRect.FORMAT);

        _blit_blur_material.SetShaderParameter("blur_size", blur_size);
        _blit_blur_material.SetShaderParameter("dim", dim);

        _blit_blur_material.SetShaderParameter("horizontal", false);
        vertical_drawable_texture.BlitRect(full_rect, drawable_texture, null, 0, _blit_blur_material); // Blur Vertical
        
        _blit_blur_material.SetShaderParameter("horizontal", true);
        drawable_texture.BlitRect(full_rect, vertical_drawable_texture, null, 0, _blit_blur_material); // Blur Horizontal
    }
}