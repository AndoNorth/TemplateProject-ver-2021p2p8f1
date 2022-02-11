using System;
using System.Collections.Generic;
using UnityEngine;

namespace TemplateProject
{
    public class TilemapSystem
    {
        public event EventHandler OnLoaded;

        GridSystem<TilemapObject> grid;

        public TilemapSystem(int width, int height, float cellSize, Vector3 originPosition, bool showDebug)
        {
            grid = new GridSystem<TilemapObject>(width, height, cellSize, originPosition, (GridSystem<TilemapObject> grid, int x, int y) => new TilemapObject(grid, x, y), showDebug);
        }

        public void SetTilemapSprite(Vector3 worldPos, TilemapObject.TilemapSprite tilemapSprite)
        {
            TilemapObject tilemapObject = grid.GetGridObject(worldPos);
            if (tilemapObject != null)
            {
                tilemapObject.SetTilemapSprite(tilemapSprite);
            }
        }

        public void SetTilemapVisual(TilemapSystem tilemap, TilemapVisuals tilemapVisual)
        {
            tilemapVisual.SetupGrid(tilemap, grid);
        }
        // save / load
        public class SaveObject
        {
            public TilemapObject.TilemapSaveObject[] tilemapObjectSaveObjectArray;
            // add more fields here to save more data
        }

        public void SaveTilemap()
        {
            List<TilemapObject.TilemapSaveObject> tilemapObjectSaveObjectList = new List<TilemapObject.TilemapSaveObject>();
            for (int x = 0; x < grid.GetWidth(); x++)
            {
                for (int y = 0; y < grid.GetHeight(); y++)
                {
                    TilemapObject tilemapObject = grid.GetGridObject(x, y);
                    tilemapObjectSaveObjectList.Add(tilemapObject.Save());
                }
            }

            SaveObject saveObject = new SaveObject { tilemapObjectSaveObjectArray = tilemapObjectSaveObjectList.ToArray() }; // json arrays work easier than json lists

            SaveLoadSystem.SaveObject("tilemap", saveObject, true);
        }

        public void LoadTilemap()
        {
            SaveObject saveObject = SaveLoadSystem.LoadObject<SaveObject>("tilemap_0");
            foreach (TilemapObject.TilemapSaveObject tilemapObjectSaveObject in saveObject.tilemapObjectSaveObjectArray)
            {
                TilemapObject tilemapObject = grid.GetGridObject(tilemapObjectSaveObject.x, tilemapObjectSaveObject.y);
                tilemapObject.Load(tilemapObjectSaveObject);
            }

            OnLoaded?.Invoke(this, EventArgs.Empty);
        }

        public class TilemapObject
        {
            public enum TilemapSprite
            {
                None,
                Square,
                Circle,
                Diamond
            }

            private GridSystem<TilemapObject> grid;
            private int x;
            private int y;
            private TilemapSprite tilemapSprite;

            public TilemapObject(GridSystem<TilemapObject> grid, int x, int y)
            {
                this.grid = grid;
                this.x = x;
                this.y = y;
            }

            public void SetTilemapSprite(TilemapSprite tilemapSprite)
            {
                this.tilemapSprite = tilemapSprite;
                grid.TriggerGridObjectChanged(x, y);
            }
            public TilemapSprite GetTilemapSprite()
            {
                return tilemapSprite;
            }

            public override string ToString()
            {
                return tilemapSprite.ToString();
            }
            [System.Serializable] // required as we use it in an array
            public class TilemapSaveObject
            {
                public TilemapSprite tilemapSprite;
                public int x;
                public int y;
                // add more fields here to save more data
            }
            public TilemapSaveObject Save()
            {
                return new TilemapSaveObject
                {
                    tilemapSprite = tilemapSprite,
                    x = x,
                    y = y
                };
            }
            public void Load(TilemapSaveObject saveObject)
            {
                tilemapSprite = saveObject.tilemapSprite;
            }
        }

    }
}
