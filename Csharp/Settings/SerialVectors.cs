using Godot;

public struct SerialVector2
{
    public float x, y;

    public SerialVector2() {}
    public static SerialVector2 Create(float x, float y) {
        return new SerialVector2
        {
            x = x,
            y = y
        };
    }

    public static implicit operator Vector2(SerialVector2 sv2) => new Vector2(sv2.x, sv2.y);
    public static implicit operator SerialVector2(Vector2 v2) => SerialVector2.Create(v2.X, v2.Y);
}

public struct SerialVector2I
{
    public int x, y;

    public SerialVector2I() {}
    public static SerialVector2I Create(int x, int y) {
        return new SerialVector2I
        {
            x = x,
            y = y
        };
    }

    public static implicit operator Vector2I(SerialVector2I sv2) => new Vector2I(sv2.x, sv2.y);
    public static implicit operator SerialVector2I(Vector2I v2) => SerialVector2I.Create(v2.X, v2.Y);
}