
using Godot;
using System.Threading.Tasks;

[GlobalClass]
public partial class Zoomer : Node {
	[Export] public Label zoom_display;

	private bool _tweening;
	private Tween _tween;
	private float _last_time;

	public override void _Ready() {
		zoom_display.Hide();
	}

	public override void _Process(double delta) {
		if(ApplicationManager.is_user_typing || ApplicationManager.currently_focused_window != GetTree().Root) {
			return;
		}

		float zoom = GetTree().Root.ContentScaleFactor;
		
		if (Input.IsActionJustPressed("scale_up")) {
			zoom += 0.1f;
		}
		if (Input.IsActionJustPressed("scale_down")) {
			zoom -= 0.1f;
		}
		
		zoom = Mathf.Clamp(zoom, 0.5f, 1.3f);
		
		_last_time -= (float)delta;
		
		if (zoom != GetTree().Root.ContentScaleFactor) {
			GetTree().Root.ContentScaleFactor = zoom;
			
			zoom_display.Text = $"{Mathf.RoundToInt(100 * zoom)}%";
			
			if (_last_time < 0.0f) {
				zoom_display.Show();
				zoom_display.SelfModulate = Colors.Transparent;
				
				if (_tween != null) {
					_tween.Stop();
				}
				
				_tween = GetTree().CreateTween();
				_tween.Finished += () => _DisableZoomDisplayAsync();
				_tween.TweenProperty(zoom_display, "self_modulate", Colors.White, 0.3f);
			}
			
			_last_time = 0.5f; // Reset
		}
	}
	
	private async Task _DisableZoomDisplayAsync() {
		while(_last_time > 0.0f) {
			await ToSignal(GetTree().CreateTimer(_last_time), SceneTreeTimer.SignalName.Timeout);
		}

		_tween = GetTree().CreateTween();
		_tween.Finished += zoom_display.Hide;
		_tween.TweenProperty(zoom_display, "self_modulate", Colors.Transparent, 1.0f);
		_tween.TweenCallback(new Callable(this, "_HideZoomDisplay"));
		_tween.TweenProperty(zoom_display, "custom_minimum_size:x", 0f, 0.1f);
	}

	private void _HideZoomDisplay() {
		zoom_display.CustomMinimumSize = zoom_display.Size;
		zoom_display.Text = "";
	}
}
