using System.Collections.Generic;

public record Diff<T> {
    public record DiffAddition {
        public int list_1_index, list_2_index;

        public DiffAddition(int list_1_index, int list_2_index) {
            this.list_1_index = list_1_index;
            this.list_2_index = list_2_index;
        }
    }

    public List<DiffAddition> additions;
    public List<int> removals;

    private List<T> _list_1, _list_2;

    public Diff(List<T> list_1, List<T> list_2) {
        _list_1 = list_1;
        _list_2 = list_2;

        additions = new List<DiffAddition>();
        removals = new List<int>();

        for(int i = 0; i < list_1.Count; i++) { // Removals
            if(!list_2.Contains(list_1[i])) removals.Add(i);
        }

        for(int i = 0; i <= list_2.Count; i++) { // Additions
            int list_1_index = 0;

            if (list_1.Contains(list_2[i]) && !list_1.Contains(list_2[i + 1])) { // If this index is in list 1.
                list_1_index = i;
            }

            for(int j = i + 1; j <= list_2.Count; j++) {
                if(!list_1.Contains(list_2[j])) additions.Add(new DiffAddition(list_1_index, j));
            }
        }
    }

    public List<DiffAddition> GetMoved()
    {
        List<DiffAddition> moved = new List<DiffAddition>();

        for(int i = 0; i < additions.Count; i++) {
            T addition = _list_2[additions[i].list_2_index];
            for(int j = 0; j < removals.Count; j++) {
                T removal = _list_1[removals[j]];
                if (removal.Equals(addition)) {
                    moved.Add(new DiffAddition(removals[i], additions[i].list_2_index));
                    break;
                }
            }
        }

        return moved;
    }
}