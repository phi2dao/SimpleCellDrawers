using System;
using System.Collections.Generic;

using UnityEngine;

using Verse;

namespace SimpleCellDrawers
{
    public abstract class SimpleCellDrawer
    {
        protected const int DefaultRenderQueue = 3600;
        protected const float DefaultOpacity = 0.33f;
        private const int MaxCellsPerMesh = 16383;

        protected SimpleCellDrawer(
            Func<Color> colorGetter,
            Func<IntVec3, Color> extraColorGetter,
            int renderQueue,
            float opacity = DefaultOpacity)
        {
            this.colorGetter = colorGetter;
            this.extraColorGetter = extraColorGetter;
            this.renderQueue = renderQueue;
            this.opacity = opacity;
        }

        protected SimpleCellDrawer(
            Func<Color> colorGetter,
            Func<IntVec3, Color> extraColorGetter,
            float opacity = DefaultOpacity) :
            this(colorGetter, extraColorGetter, DefaultRenderQueue, opacity)
        { }

        public virtual void Draw()
        {
            if (dirty)
            {
                RegenerateMeshes();
            }
            for (int i = 0; i < meshes.Count; i++)
            {
                Graphics.DrawMesh(meshes[i], Vector3.zero, Quaternion.identity, material, 0);
            }
        }

        public void SetDirty() => dirty = true;

        protected abstract IEnumerable<IntVec3> CellsToDraw();

        private void RegenerateMeshes()
        {
            for (int i = 0; i < meshes.Count; i++)
            {
                meshes[i].Clear();
            }
            int mi = 0;
            int ci = 0;
            Mesh mesh = GetMesh(mi);
            float y = AltitudeLayer.MapDataOverlay.AltitudeFor();
            bool careAboutVertexColors = false;
            foreach (IntVec3 cell in CellsToDraw())
            {
                verts.Add(new Vector3(cell.x, y, cell.z));
                verts.Add(new Vector3(cell.x, y, cell.z + 1));
                verts.Add(new Vector3(cell.x + 1, y, cell.z + 1));
                verts.Add(new Vector3(cell.x + 1, y, cell.z));
                Color color = extraColorGetter(cell);
                colors.Add(color);
                colors.Add(color);
                colors.Add(color);
                colors.Add(color);
                careAboutVertexColors = color != Color.white || careAboutVertexColors;
                int count = verts.Count;
                tris.Add(count - 4);
                tris.Add(count - 3);
                tris.Add(count - 2);
                tris.Add(count - 4);
                tris.Add(count - 2);
                tris.Add(count - 1);
                if (++ci >= MaxCellsPerMesh)
                {
                    FinalizeWorkingDataIntoMesh(mesh);
                    mesh = GetMesh(++mi);
                    ci = 0;
                }
            }
            FinalizeWorkingDataIntoMesh(mesh);
            CreateMaterialIfNeeded(careAboutVertexColors);
            dirty = false;
        }

        private Mesh GetMesh(int index)
        {
            while (meshes.Count <= index)
            {
                Mesh mesh = new Mesh { name = GetType().Name };
                meshes.Add(mesh);
            }
            return meshes[index];
        }

        private void FinalizeWorkingDataIntoMesh(Mesh mesh)
        {
            if (verts.Count > 0)
            {
                mesh.SetVertices(verts);
                verts.Clear();
                mesh.SetTriangles(tris, 0);
                tris.Clear();
                mesh.SetColors(colors);
                colors.Clear();
            }
        }

        private void CreateMaterialIfNeeded(bool careAboutVertexColors)
        {
            if (material == null || materialCaresAboutVertexColors != careAboutVertexColors)
            {
                Color color = colorGetter().ToTransparent(opacity);
                material = SolidColorMaterials.SimpleSolidColorMaterial(color, careAboutVertexColors);
                materialCaresAboutVertexColors = careAboutVertexColors;
                material.renderQueue = renderQueue;
            }
        }

        protected readonly Func<Color> colorGetter;
        protected readonly Func<IntVec3, Color> extraColorGetter;
        protected readonly int renderQueue = DefaultRenderQueue;
        protected readonly float opacity = DefaultOpacity;

        protected bool dirty;

        private static readonly List<Vector3> verts = new List<Vector3>();
        private static readonly List<int> tris = new List<int>();
        private static readonly List<Color> colors = new List<Color>();

        private Material material;
        private bool materialCaresAboutVertexColors;
        private readonly List<Mesh> meshes = new List<Mesh>();
    }
}
