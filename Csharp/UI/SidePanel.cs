using Godot;

public partial class SidePanel : Panel {
    [Export] public int open_minimum_size = 300;
    [Export] public int closed_minimum_size = 60;
    [Export] public HSplitContainer split_container;

    [Export] public Button open_button, close_button;
    [Export] public Control open_element, closed_element;

    private float _target_minimum_size;
    private bool _closed = false;

    public override void _Ready() {
        open_button.ButtonUp += _ToggleOpen;
        close_button.ButtonUp += _ToggleOpen;

        _target_minimum_size = open_minimum_size;
    }

    private void _ToggleOpen() {
        _closed = !_closed;

        _target_minimum_size = _closed ? closed_minimum_size : open_minimum_size;

        closed_element.Visible = _closed;
	    open_element.Visible = !_closed;

        split_container.Collapsed = true;

        Tween tween = GetTree().CreateTween();
        
        tween.TweenProperty(this, "custom_minimum_size:x", _target_minimum_size, 0.1f).SetTrans(Tween.TransitionType.Quad);
        
        void OnTweenFinished() { 
            split_container.Collapsed = _closed; 
            split_container.SplitOffset = (int)CustomMinimumSize.X; 

            tween.Finished -= OnTweenFinished;
        }

        tween.Finished += OnTweenFinished;
    }
}