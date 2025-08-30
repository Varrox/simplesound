using Godot;

public partial class ConfirmationWindow : Window
{
	[Export] Button accept, decline, cancel;
    [Export] Label message;

	[Export] public string Message = "Confirmation Window", AcceptText = "Ok", DeclineText, CancelText = "Cancel";

	[Signal] public delegate void OnAcceptEventHandler();
    [Signal] public delegate void OnDeclineEventHandler();
    [Signal] public delegate void OnCancelEventHandler();
    [Signal] public delegate void OnCloseEventHandler(Confirm confirm);

    public bool free_on_close = false;

    public override void _Ready()
    {
        accept.Text = AcceptText;
        cancel.Text = CancelText;

        if (DeclineText != null)
        {
            decline.Text = DeclineText;
            decline.Visible = true;
        }
        else
            decline.Visible = false;

        message.Text = Message;

        accept.ButtonUp += Accept;
        decline.ButtonUp += Decline;
        cancel.ButtonUp += Cancel;
    }

    public void Close(Confirm confirm)
    { 
        Visible = false;
        EmitSignal("OnClose", (int)confirm);
        if (free_on_close) QueueFree();
    }

    public void Accept()
    {
        Close(Confirm.Accepted);
        EmitSignal("OnAccept");
    }

    public void Decline()
    {
        Close(Confirm.Declined);
        EmitSignal("OnDecline");
    }

    public void Cancel()
    {
        Close(Confirm.Cancelled);
        EmitSignal("OnCancel");
    }
}

public enum Confirm
{
    Accepted,
    Declined,
    Cancelled
}
