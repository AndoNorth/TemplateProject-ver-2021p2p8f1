using UnityEngine;

namespace TemplateProject
{
    public class TestTilemap : MonoBehaviour
    {
        public bool showDebug;
        public TilemapVisuals tilemapVisual;

        private TilemapSystem tilemap;
        private TilemapSystem.TilemapObject.TilemapSprite tilemapSprite;

        // Start is called before the first frame update
        void Start()
        {
            tilemap = new TilemapSystem(10, 10, 1f, Vector3.zero, showDebug);
            tilemap.SetTilemapVisual(tilemap, tilemapVisual);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 position = GeneralUtility.GetMouseWorldPosition();
                tilemap.SetTilemapSprite(position, tilemapSprite);
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                tilemapSprite = TilemapSystem.TilemapObject.TilemapSprite.None;
                Debug.Log(tilemapSprite.ToString());
            }
            if (Input.GetKeyDown(KeyCode.Y))
            {
                tilemapSprite = TilemapSystem.TilemapObject.TilemapSprite.Square;
                Debug.Log(tilemapSprite.ToString());
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                tilemapSprite = TilemapSystem.TilemapObject.TilemapSprite.Circle;
                Debug.Log(tilemapSprite.ToString());
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                tilemapSprite = TilemapSystem.TilemapObject.TilemapSprite.Diamond;
                Debug.Log(tilemapSprite.ToString());
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                tilemap.SaveTilemap();
                Debug.Log("saved");
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                tilemap.LoadTilemap();
                Debug.Log("loaded");
            }
        }
    }
}
