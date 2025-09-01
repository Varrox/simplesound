using Godot;
using System;

public partial class ConfirmationWindow : Window
{
	[Export] Button accept, decline, cancel;
    [Export] Label message;

	[Export] public string Message = "Confirmation Window", AcceptText = "Ok", DeclineText, CancelText = "Cancel";

    public Action OnAccept;
    public Action OnDecline;
    public Action OnCancel;
    public Action<Confirm> OnClose;

    public bool free_on_close = false, front = true;

    public override void _Ready()
    {
        accept.Text = AcceptText;
        accept.Visible = AcceptText != string.Empty;

        cancel.Text = CancelText;
        cancel.Visible = CancelText != string.Empty;

        decline.Text = DeclineText;
        decline.Visible = DeclineText != string.Empty;

        message.Text = Message;

        accept.ButtonUp += Accept;
        decline.ButtonUp += Decline;
        cancel.ButtonUp += Cancel;

        AlwaysOnTop = front;
    }

    public void Close(Confirm confirm)
    { 
        Visible = false;
        OnClose?.Invoke(confirm);
        if (free_on_close) QueueFree();
    }

    public void Accept()
    {
        Close(Confirm.Accepted);
        OnAccept?.Invoke();
    }

    public void Decline()
    {
        Close(Confirm.Declined);
        OnDecline?.Invoke();
    }

    public void Cancel()
    {
        Close(Confirm.Cancelled);
        OnCancel?.Invoke();
    }
}

public enum Confirm
{
    Accepted,
    Declined,
    Cancelled
}
