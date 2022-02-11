using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TemplateProject
{
    public class TestGrid : MonoBehaviour
    {
        // grid
        GridSystem<HeatMapGridObject> grid;
        GridSystem<StringGridObject> stringGrid;
        public int x = 10, y = 10;
        public float cellSize = 1f;
        public bool showDebug = true;

        // heatmap
        public HeatMapGenericsVisuals heatmapGenerics;

        // Start is called before the first frame update
        void Start()
        {
            grid = new GridSystem<HeatMapGridObject>(x, y, cellSize, new Vector3(Mathf.RoundToInt(-x / 2), Mathf.RoundToInt(-y / 2)), (GridSystem<HeatMapGridObject> grid, int x, int y) => new HeatMapGridObject(grid, x, y), showDebug);
            stringGrid = new GridSystem<StringGridObject>(x, y, cellSize, new Vector3(Mathf.RoundToInt(-x / 2), Mathf.RoundToInt(-y / 2)), (GridSystem<StringGridObject> grid, int x, int y) => new StringGridObject(grid, x, y), showDebug);

            // heatmap.SetupGrid(grid);
            // heatmapBool.SetupGrid(grid);
            heatmapGenerics.SetupGrid(grid);
        }
        private void Update()
        {
            Vector3 position = GeneralUtility.GetMouseWorldPosition();
            if (Input.GetKeyDown(KeyCode.A)) { stringGrid.GetGridObject(position).AddLetter("A"); }
            if (Input.GetKeyDown(KeyCode.B)) { stringGrid.GetGridObject(position).AddLetter("B"); }
            if (Input.GetKeyDown(KeyCode.C)) { stringGrid.GetGridObject(position).AddLetter("C"); }

            if (Input.GetKeyDown(KeyCode.Alpha1)) { stringGrid.GetGridObject(position).AddNumber("1"); }
            if (Input.GetKeyDown(KeyCode.Alpha2)) { stringGrid.GetGridObject(position).AddNumber("2"); }
            if (Input.GetKeyDown(KeyCode.Alpha3)) { stringGrid.GetGridObject(position).AddNumber("3"); }
        }
    }

    public class HeatMapGridObject
    {
        public const int HEAT_MAP_MIN_VALUE = 1;
        public const int HEAT_MAP_MAX_VALUE = 256;

        private GridSystem<HeatMapGridObject> grid;
        private int x, y;
        private int value = 1;

        public HeatMapGridObject(GridSystem<HeatMapGridObject> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
        }

        public void AddValue(int addValue)
        {
            value += addValue;
            Mathf.Clamp(value, HEAT_MAP_MIN_VALUE, HEAT_MAP_MAX_VALUE);
            grid.TriggerGridObjectChanged(x, y);
        }
        public int GetValue()
        {
            return value;
        }

        public float GetValueNormalized()
        {
            return (float)value / HEAT_MAP_MAX_VALUE;
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }

    public class StringGridObject
    {
        private GridSystem<StringGridObject> grid;
        private int x, y;

        private string letters;
        private string numbers;

        public StringGridObject(GridSystem<StringGridObject> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
            letters = "";
            numbers = "";
        }

        public void AddLetter(string letter)
        {
            letters += letter;
            grid.TriggerGridObjectChanged(x, y);
        }
        public void AddNumber(string number)
        {
            numbers += number;
            grid.TriggerGridObjectChanged(x, y);
        }
        public override string ToString()
        {
            return letters + "\n" + numbers;
        }
    }
}
