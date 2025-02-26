using System;
using System.Collections.Generic;

using UnityEngine;

using Verse;

namespace SimpleCellDrawers
{
    public class ViewRectCellDrawer : SimpleCellDrawer
    {
        public ViewRectCellDrawer(
            Func<IntVec3, bool> cellBoolGetter,
            Func<Color> colorGetter,
            Func<IntVec3, Color> extraColorGetter,
            int renderQueue,
            float opacity = DefaultOpacity) :
            base(colorGetter, extraColorGetter, renderQueue, opacity)
        {
            this.cellBoolGetter = cellBoolGetter;
        }

        public ViewRectCellDrawer(
            Func<IntVec3, bool> cellBoolGetter,
            Func<Color> colorGetter,
            Func<IntVec3, Color> extraColorGetter,
            float opacity = DefaultOpacity) :
            base(colorGetter, extraColorGetter, opacity)
        {
            this.cellBoolGetter = cellBoolGetter;
        }

        public ViewRectCellDrawer(IViewRectCellGiver giver, int renderQueue, float opacity = DefaultOpacity) :
            this(giver.GetCellBool, () => giver.Color, giver.GetCellExtraColor, renderQueue, opacity)
        { }

        public ViewRectCellDrawer(IViewRectCellGiver giver, float opacity = DefaultOpacity) :
            this(giver, DefaultRenderQueue, opacity)
        { }

        public override void Draw()
        {
            CellRect newView = CellRect.ViewRect(Find.CurrentMap);
            if (newView.IsEmpty)
            {
                return;
            }
            if (dirty || !newView.FullyContainedWithin(view))
            {
                view = Pad(newView).ClipInsideMap(Find.CurrentMap);
                dirty = true;
            }
            base.Draw();
        }

        protected override IEnumerable<IntVec3> CellsToDraw()
        {
            foreach (IntVec3 cell in view)
            {
                if (cellBoolGetter(cell))
                {
                    yield return cell;
                }
            }
        }

        private CellRect Pad(CellRect rect)
        {
            rect.minX -= rect.Width;
            rect.maxX += rect.Width;
            rect.minZ -= rect.Height;
            rect.maxZ += rect.Height;
            return rect;
        }

        private readonly Func<IntVec3, bool> cellBoolGetter;
        private CellRect view;
    }
}
