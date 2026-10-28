using System.Collections.Generic;
using Godot;

public partial class Background : TextureRect {
    private Dictionary<int, BlitEffect> effects;
    private DrawableTexture2D drawable_texture;

    private bool _queued_for_redraw;

    private Vector2 _last_size;
    private bool _last_visible;

    public const DrawableTexture2D.DrawableFormat FORMAT = DrawableTexture2D.DrawableFormat.Rgbah;

    public override void _Ready() {
        effects = new Dictionary<int, BlitEffect>();
        drawable_texture = new DrawableTexture2D();

        TransitionBlitEffect gup = new TransitionBlitEffect();
        gup.last_texture = GD.Load<Texture2D>("res://Icons/Default Covers/DefaultCover.png");
        gup.new_texture = GD.Load<Texture2D>("res://Icons/Logos/logochrimis.png");

        gup.time = 0f;

        AppendEffect(gup);

        AppendEffect(new BlurBlitEffect());

        QueueRedrawBlits();
        
        Resized += QueueRedrawBlits;
        Hidden += QueueRedrawBlits;

        _last_size = -Vector2.One;
        _last_visible = false;
    }

    public override void _Process(double delta) {
        bool changed = !_last_visible || Size != _last_size;

        if ((_queued_for_redraw || changed) && Visible) {
            _last_visible = Visible;
            _last_size = Size;

            _queued_for_redraw = false;
            RedrawBlits();
        }
    }

    public int AppendEffect(BlitEffect effect) {
        int idx = effects.Count;

        while (effects.ContainsKey(idx)) idx++;

        effects.Add(idx, effect);
        RedrawBlits();

        return idx;
    }

    public void RemoveEffect(int effect_idx) {
        effects.Remove(effect_idx);
        RedrawBlits();
    }

    public void QueueRedrawBlits() {
        _queued_for_redraw = true;
    }

    public void RedrawBlits() {
        Rect2I full_rect = new Rect2I(0, 0, (Vector2I)Size);

        drawable_texture.Setup(full_rect.Size.X, full_rect.Size.Y, FORMAT);

        foreach(int idx in effects.Keys) {
            effects[idx].Blit(ref drawable_texture, full_rect);
        }

        Texture = drawable_texture;
    }
}

public interface BlitEffect {
    public void Blit(ref DrawableTexture2D drawable_texture, Rect2I full_rect);
}

public interface ShaderBlitEffect : BlitEffect {
    
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

    public void Blit(ref DrawableTexture2D drawable_texture, Rect2I full_rect) {
        vertical_drawable_texture.Setup(full_rect.Size.X, full_rect.Size.Y, Background.FORMAT);

        _blit_blur_material.SetShaderParameter("blur_size", blur_size);
        _blit_blur_material.SetShaderParameter("dim", dim);

        _blit_blur_material.SetShaderParameter("horizontal", false);
        vertical_drawable_texture.BlitRect(full_rect, drawable_texture, null, 0, _blit_blur_material); // Blur Vertical
        
        _blit_blur_material.SetShaderParameter("horizontal", true);
        drawable_texture.BlitRect(full_rect, vertical_drawable_texture, null, 0, _blit_blur_material); // Blur Horizontal
    }
}