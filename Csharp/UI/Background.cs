using System.Collections.Generic;
using Godot;

public partial class Background : TextureRect {
    private List<BlitEffect> effects;
    private DrawableTexture2D drawable_texture;

    public override void _Ready()
    {
        drawable_texture = new DrawableTexture2D();
        CallDeferred("SetupDrawableTexture");
    }

    public void SetupDrawableTexture() {
        drawable_texture.Setup((int)Size.X, (int)Size.Y, DrawableTexture2D.DrawableFormat.Rgba8);
    }

    public void AddEffect(BlitEffect effect) {
        effects.Add(effect);
        Redraw();
    }

    public void RemoveEffect(BlitEffect effect) {
        effects.Remove(effect);
        Redraw();
    }

    public void Redraw() {
        SetupDrawableTexture();
        Rect2 full_rect = new Rect2(0, 0, Size);

        for(int i = 0; i < effects.Count; i++) {
            effects[i].Blit(ref drawable_texture, full_rect);
        }

        Texture = drawable_texture;
    }
}

public interface BlitEffect {
    public void Blit(ref DrawableTexture2D drawable_texture, Rect2 full_rect);
}

public class BlurBlitEffect : BlitEffect {
    private static Shader _blit_blur_shader;

    public BlurBlitEffect() {
        _blit_blur_shader = ApplicationManager.GetShaderResource("blur_blit");
    }

    public void Blit(ref DrawableTexture2D drawable_texture, Rect2 full_rect) {
        drawable_texture.BlitRect()
    }
}