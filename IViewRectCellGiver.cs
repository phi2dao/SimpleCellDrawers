using UnityEngine;

using Verse;

namespace SimpleCellDrawers
{
    public interface IViewRectCellGiver
    {
        Color Color { get; }
        bool GetCellBool(IntVec3 cell);
        Color GetCellExtraColor(IntVec3 cell);
    }
}
