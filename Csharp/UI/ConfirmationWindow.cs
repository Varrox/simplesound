using Godot;

public partial class ConfirmationWindow : Window {
	[Export] Button accept_button, decline_button, cancel_button;
    [Export] CheckBox ignore;
    [Export] Label message_label;

	[Export] public string message = "Confirmation Window", accept_text = "Ok", decline_text, cancel_text = "Cancel";

    [Signal]
    public delegate void OnAcceptEventHandler();

    [Signal]
    public delegate void OnDeclineEventHandler();

    [Signal]
    public delegate void OnCancelEventHandler();

    [Signal]
    public delegate void OnCloseEventHandler(Confirm confirm);

    public bool free_on_close = false, front = true, ignored = false, show_ignore = false;

    public override void _Ready() {
        accept_button.Text = accept_text;
        accept_button.Visible = accept_text != string.Empty;

        cancel_button.Text = cancel_text;
        cancel_button.Visible = cancel_text != string.Empty;

        decline_button.Text = decline_text;
        decline_button.Visible = decline_text != string.Empty;

        message_label.Text = message;

        accept_button.ButtonUp += Accept;
        decline_button.ButtonUp += Decline;
        cancel_button.ButtonUp += Cancel;

        AlwaysOnTop = front;
        ignore.Visible = show_ignore;
    }

    public void Close(Confirm confirm) {
        ignored = ignore.ButtonPressed;
        Visible = false;
        EmitSignal(SignalName.OnClose, (int)confirm);
        if (free_on_close) QueueFree();
    }

    public void Accept() {
        Close(Confirm.Accepted);
        EmitSignal(SignalName.OnAccept);
    }

    public void Decline() {
        Close(Confirm.Declined);
        EmitSignal(SignalName.OnDecline);
    }

    public void Cancel() {
        Close(Confirm.Cancelled);
        EmitSignal(SignalName.OnCancel);
    }
}

public enum Confirm {
    Accepted,
    Declined,
    Cancelled
}
