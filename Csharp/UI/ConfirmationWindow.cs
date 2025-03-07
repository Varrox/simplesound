using Godot;
using System;

public partial class ConfirmationWindow : Window
{
	[Export] Button accept, cancel;
    [Export] Label message;

	[Export] public string Title, Message = "Confirmation Window", AcceptText = "Ok", CancelText = "Cancel";

	[Signal] public delegate void OnAcceptEventHandler();
    [Signal] public delegate void OnCancelEventHandler();

    public override void _Ready()
    {
        (this as Window).Title = Title;
        accept.Text = AcceptText;
        cancel.Text = CancelText;
        message.Text = Message;
        accept.ButtonDown += Accept;
        cancel.ButtonDown += Cancel;
    }

    public void Close()
    { 
        Visible = false;
    }

    public void Cancel()
    {
        Close();
        EmitSignal("OnCancel");
    }

    public void Accept()
    {
        Close();
        EmitSignal("OnAccept");
    }
}
