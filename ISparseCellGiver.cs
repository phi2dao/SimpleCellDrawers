using System.Collections.Generic;

using UnityEngine;

using Verse;

namespace SimpleCellDrawers
{
    public interface ISparseCellGiver
    {
        Color Color { get; }
        IEnumerable<IntVec3> GetCells();
        Color GetCellExtraColor(IntVec3 cell);
    }
}
