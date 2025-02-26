using System;
using System.Collections.Generic;

using UnityEngine;

using Verse;

namespace SimpleCellDrawers
{
    public class SparseCellDrawer : SimpleCellDrawer
    {
        public SparseCellDrawer(
            Func<IEnumerable<IntVec3>> cellGetter,
            Func<Color> colorGetter,
            Func<IntVec3, Color> extraColorGetter,
            int renderQueue,
            float opacity = DefaultOpacity) :
            base(colorGetter, extraColorGetter, renderQueue, opacity)
        {
            this.cellGetter = cellGetter;
        }

        public SparseCellDrawer(
            Func<IEnumerable<IntVec3>> cellGetter,
            Func<Color> colorGetter,
            Func<IntVec3, Color> extraColorGetter,
            float opacity = DefaultOpacity) :
            base(colorGetter, extraColorGetter, opacity)
        {
            this.cellGetter = cellGetter;
        }

        public SparseCellDrawer(ISparseCellGiver giver, int renderQueue, float opacity = DefaultOpacity) :
            this(giver.GetCells, () => giver.Color, giver.GetCellExtraColor, renderQueue, opacity)
        { }

        public SparseCellDrawer(ISparseCellGiver giver, float opacity = DefaultOpacity) :
            this(giver, DefaultRenderQueue, opacity)
        { }

        protected override IEnumerable<IntVec3> CellsToDraw() => cellGetter();

        private readonly Func<IEnumerable<IntVec3>> cellGetter;
    }
}
