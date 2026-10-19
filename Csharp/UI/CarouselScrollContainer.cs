using System;
using Godot;
using Godot.Collections;

/// Can contain large amount of content without using more instances of the content than what is visible. 
public partial class CarouselScrollContainer : ScrollContainer {
    // Exports

    [Export] public PackedScene item_template;

    [Export] public VBoxContainer parent_container, item_container;

    [Export] public int total_items;

    [Export] public int item_height = 60;

    // Public

    public Array<Control> items { get { return _items; } }

    // Private

    private Array<Control> _items = new Array<Control>();

    private Control _top_spacer, _bottom_spacer;

    private int _last_scroll = -1, _last_total_items = -1, _last_first_item = -1;
    private Vector2 _last_size = Vector2.Zero;

    public override void _Ready()
    {
        parent_container.AddThemeConstantOverride("separation", 0);
        item_container.AddThemeConstantOverride("separation", 0);

        _top_spacer = new Panel();
        _bottom_spacer = new Panel();

        parent_container.AddChild(_top_spacer);
        parent_container.MoveChild(_top_spacer, item_container.GetIndex());

        item_container.AddSibling(_bottom_spacer);
    }

    public override void _Process(double delta)
    {
        if (_last_scroll != ScrollVertical || _last_size != Size || _last_total_items != total_items) {
            int max_items = (int)Mathf.Ceil(Size.Y / item_height) + 1;
            int target_items = Mathf.Min(max_items, total_items);

            var last_item_count = _items.Count;
            int first_item = GetFirstVisibleItemIndex();

            while(_items.Count != target_items) { // Match target items
                if(_items.Count > target_items) { // Remove extra
                    RemoveItem();
                }
                else { // Add new
                    _items.Add(_CreateItem());

                    int index = _items.Count - 1;

                    item_container.AddChild(_items[index]);

                    _UpdateItem(first_item + index, _items[index]);
                }
            }

            int top_space = _GetTopSpace();
            int bottom_space = _GetBottomSpace();

            _top_spacer.CustomMinimumSize = Vector2.Down * top_space;
            _bottom_spacer.CustomMinimumSize = Vector2.Down * bottom_space;

            _top_spacer.Visible = target_items == max_items;
            _bottom_spacer.Visible = target_items == max_items;

            int shift = first_item - _last_first_item;
            _last_first_item = first_item;

            if (shift == 0) { // Scale only
                for(int i = last_item_count; i < _items.Count; i++) {
                    _UpdateItem(first_item + i, _items[i]);
                }
            }
            else { // Scroll + possibly scale
                int a_shift = Mathf.Abs(shift);
                if (a_shift > target_items) { // Completely reupdate
                    Refresh();
                }
                else { // We can shift past ones up or down 
                    int last_index = _items.Count - 1;
                
                    for(int j = 0; j < a_shift; j++) {
                        Control item;
                        int item_index;

                        if(shift < 0) { // Moving up
                            item = _items[last_index];
                            _items.RemoveAt(last_index);
                            _items.Insert(0, item);

                            item_container.MoveChild(item, 0);

                            item_index = first_item + (a_shift - j) - 1;
                        }
                        else { // Moving down
                            item = _items[0];
                            _items.RemoveAt(0);
                            _items.Add(item); // Add instead of insert because it is probably faster and makes more sense logic wise.

                            item_container.MoveChild(item, last_index);

                            item_index = first_item + _items.Count - (a_shift - j);
                        }

                        _UpdateItem(item_index, item);
                    }
                }
            }
        }

        _last_scroll = ScrollVertical;
        _last_size = Size;
    }

    virtual protected Control _CreateItem() {
        Node item = item_template != null ? item_template.Instantiate() : null;

        return item is Control ? (Control)item : null;
    }

    public void Refresh() {
        int first_item = GetFirstVisibleItemIndex();

        for(int i = 0; i < _items.Count; i++) {
            _UpdateItem(first_item + i, _items[i]);
        }
    }

    public void RemoveItem() {
        _items[_items.Count - 1].QueueFree();
        _items.RemoveAt(_items.Count - 1);
    }

    /// item_index is the index in total items, not items array.
    virtual protected void _UpdateItem(int item_index, Control item) {
        
    }

    private int _GetTotalHeight() {
        return item_height * total_items;
    }

    private int _GetTopSpace()
    {
        int top = Mathf.Min(Mathf.Max(ScrollVertical - (int)_top_spacer.Position.Y, 0), _GetTotalHeight() - (int)Size.Y - (int)_top_spacer.Position.Y);
        top -= top % item_height;

        return top;
    }
    
    private int _GetBottomSpace()
    {
        int bottom = Mathf.Max(_GetTotalHeight() - (_GetTopSpace() + (int)Size.Y) - (int)_top_spacer.Position.Y, 0);
        bottom -= bottom % item_height;

        return bottom;
    }

    public int GetFirstVisibleItemIndex()
    {
        return Mathf.Clamp(_GetTopSpace() / item_height, 0, Mathf.Max(total_items - 1, 0));
    }
}