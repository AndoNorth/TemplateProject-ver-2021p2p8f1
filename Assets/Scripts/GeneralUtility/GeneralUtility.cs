using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
// brief: this script is used for general Utility
public static class GeneralUtility
{
    #region timers
    // call a function after elapsed time
    public class FunctionTimer
    {
        // dummy class to have access to MonoBehaviour functions
        public class MonoBehaviourHook : MonoBehaviour
        {
            public Action _onUpdate;
            private void Update()
            {
                if (_onUpdate != null)
                {
                    _onUpdate();
                }
            }
        }
        // static
        private static List<FunctionTimer> _activeTimerList;
        private static GameObject _initGameObject;
        private static void InitIfNeeded()
        {
            if (_initGameObject == null)
            {
                _initGameObject = new GameObject("FunctionTimer_InitGameObject");
                _activeTimerList = new List<FunctionTimer>();
            }
        }
        public static FunctionTimer Create(Action action, float timer)
        {
            return Create(action, timer, "", false, false);
        }
        public static FunctionTimer Create(Action action, float timer, string timerName)
        {
            return Create(action, timer, timerName, false, false);
        }
        public static FunctionTimer Create(Action action, float timer, string timerName, bool useUnscaledDeltaTime)
        {
            return Create(action, timer, timerName, useUnscaledDeltaTime, false);
        }
        public static FunctionTimer Create(Action action, float timer, string timerName, bool useUnscaledDeltaTime, bool stopAllWithSameName)
        {
            InitIfNeeded();
            if (stopAllWithSameName)
            {
                StopAllTimersWithSameName(timerName);
            }

            GameObject gameObject = new GameObject("FunctionTimer Object " + timerName, typeof(MonoBehaviourHook));

            FunctionTimer functionTimer = new FunctionTimer(gameObject, action, timer, timerName, useUnscaledDeltaTime);

            gameObject.GetComponent<MonoBehaviourHook>()._onUpdate = functionTimer.Update;

            _activeTimerList.Add(functionTimer);

            return functionTimer;
        }
        private static void RemoveTimer(FunctionTimer functionTimer)
        {
            InitIfNeeded();
            _activeTimerList.Remove(functionTimer);
        }
        private static void StopAllTimersWithSameName(string timerName)
        {
            InitIfNeeded();
            for (int i = 0; i < _activeTimerList.Count; i++)
            {
                if (_activeTimerList[i]._timerName == timerName)
                {
                    _activeTimerList[i].DestroySelf();
                    i--;
                }
            }
        }
        private static void StopFirstTimerWithName(string timerName)
        {
            for (int i = 0; i < _activeTimerList.Count; i++)
            {
                if (_activeTimerList[i]._timerName == timerName)
                {
                    _activeTimerList[i].DestroySelf();
                    return;
                }
            }
        }

        // non-static
        private Action _action;
        private float _timer;
        private string _timerName;
        private GameObject _gameObject;
        private bool _useUnscaledDeltaTime;

        private FunctionTimer(GameObject gameObject, Action action, float timer, string timerName, bool useUnscaledDeltaTime)
        {
            this._gameObject = gameObject;
            this._action = action;
            this._timer = timer;
            this._timerName = timerName;
            this._useUnscaledDeltaTime = useUnscaledDeltaTime;
        }
        public void Update()
        {
            if (_useUnscaledDeltaTime)
            {
                _timer -= Time.unscaledDeltaTime;
            }
            else
            {
                _timer -= Time.deltaTime;
            }
            if (_timer < 0)
            {
                _action();
                DestroySelf();
            }
        }
        private void DestroySelf()
        {
            RemoveTimer(this);
            if (_gameObject != null)
            {
                UnityEngine.Object.Destroy(_gameObject);
            }
        }
    }
    #endregion
    #region getMouseWorldPosition
    // brief: get the mouse world position
    public static Vector3 GetMouseWorldPosition()
    {
        Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        vec.z = 0f;
        return vec;
    }
    // with position and camera reference
    public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
    {
        Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
        return worldPosition;
    }
    // with camera reference
    public static Vector3 GetMouseWorldPositionWithZ(Camera worldCamera)
    {
        return GetMouseWorldPositionWithZ(Input.mousePosition, worldCamera);
    }
    // with no reference
    public static Vector3 GetMouseWorldPositionWithZ()
    {
        return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
    }
    // face target
    // brief: for topdown transform to rotate the z axis to face the target
    public static void FaceTarget(Transform transform, Vector3 target, float offset = 90)
    {
        Vector3 difference = target - transform.position; // Camera.main.ScreenToWorldPoint(Input.mousePosition)
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ + offset);
    }
    #endregion
    #region textInWorld
    // create text in the world
    // brief: creates text in the game world, with default values - USE THIS
    public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), int fontSize = 40, Color? color = null, TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = 5000)
    {
        if (color == null) color = Color.white;
        return CreateWorldText(parent, text, localPosition, fontSize, (Color)color, textAnchor, textAlignment, sortingOrder);
    }
    // create text in the world - supporting function - IGNORE THIS
    public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder)
    {
        GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return textMesh;
    }
    #endregion
    public class World_Bar
    {

        private GameObject gameObject;
        private Transform transform;
        private Transform background;
        private Transform bar;

        public static int GetSortingOrder(Vector3 position, int offset, int baseSortingOrder = 5000)
        {
            return (int)(baseSortingOrder - position.y) + offset;
        }

        public class Outline
        {
            public float size = 1f;
            public Color color = Color.black;
        }

        public World_Bar(Transform parent, Vector3 localPosition, Vector3 localScale, Color? backgroundColor, Color barColor, float sizeRatio, int sortingOrder, Outline outline = null)
        {
            SetupParent(parent, localPosition);
            if (outline != null) SetupOutline(outline, localScale, sortingOrder - 1);
            if (backgroundColor != null) SetupBackground((Color)backgroundColor, localScale, sortingOrder);
            SetupBar(barColor, localScale, sortingOrder + 1);
            SetSize(sizeRatio);
        }
        private void SetupParent(Transform parent, Vector3 localPosition)
        {
            gameObject = new GameObject("World_Bar");
            transform = gameObject.transform;
            transform.SetParent(parent);
            transform.localPosition = localPosition;
        }
        private void SetupOutline(Outline outline, Vector3 localScale, int sortingOrder)
        {
            CreateWorldSprite(transform, "Outline", GameAssets.instance.white1x1, new Vector3(0, 0), localScale + new Vector3(outline.size, outline.size), sortingOrder, outline.color);
        }
        private void SetupBackground(Color backgroundColor, Vector3 localScale, int sortingOrder)
        {
            background = CreateWorldSprite(transform, "Background", GameAssets.instance.white1x1, new Vector3(0, 0), localScale, sortingOrder, backgroundColor).transform;
        }
        private void SetupBar(Color barColor, Vector3 localScale, int sortingOrder)
        {
            GameObject barGO = new GameObject("Bar");
            bar = barGO.transform;
            bar.SetParent(transform);
            bar.localPosition = new Vector3(-localScale.x * 0.5f, 0, 0);
            bar.localScale = new Vector3(1, 1, 1);
            Transform barIn = CreateWorldSprite(bar, "BarIn", GameAssets.instance.white1x1, new Vector3(localScale.x * 0.5f, 0), localScale, sortingOrder, barColor).transform;
        }
        public void SetRotation(float rotation)
        {
            transform.localEulerAngles = new Vector3(0, 0, rotation);
        }
        public void SetSize(float sizeRatio)
        {
            bar.localScale = new Vector3(sizeRatio, 1, 1);
        }
        public void SetColor(Color color)
        {
            bar.Find("BarIn").GetComponent<SpriteRenderer>().color = color;
        }
        public void Show()
        {
            gameObject.SetActive(true);
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        public void DestroySelf()
        {
            if (gameObject != null)
            {
                UnityEngine.Object.Destroy(gameObject);
            }
        }
    }
    // Create a Sprite in the World, no parent
    public static GameObject CreateWorldSprite(string name, Sprite sprite, Vector3 position, Vector3 localScale, int sortingOrder, Color color)
    {
        return CreateWorldSprite(null, name, sprite, position, localScale, sortingOrder, color);
    }

    // Create a Sprite in the World
    public static GameObject CreateWorldSprite(Transform parent, string name, Sprite sprite, Vector3 localPosition, Vector3 localScale, int sortingOrder, Color color)
    {
        GameObject gameObject = new GameObject(name, typeof(SpriteRenderer));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        transform.localScale = localScale;
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = sortingOrder;
        spriteRenderer.color = color;
        return gameObject;
    }
    // Is Mouse over a UI Element? Used for ignoring World clicks through UI
    public static bool IsPointerOverUI()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }
        else
        {
            PointerEventData pe = new PointerEventData(EventSystem.current);
            pe.position = Input.mousePosition;
            List<RaycastResult> hits = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pe, hits);
            return hits.Count > 0;
        }
    }

    // Get World Position from UI Position
    public static Vector3 GetWorldPositionFromUI()
    {
        return GetWorldPositionFromUI(Input.mousePosition, Camera.main);
    }

    public static Vector3 GetWorldPositionFromUI(Camera worldCamera)
    {
        return GetWorldPositionFromUI(Input.mousePosition, worldCamera);
    }

    public static Vector3 GetWorldPositionFromUI(Vector3 screenPosition, Camera worldCamera)
    {
        Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
        return worldPosition;
    }

    public static Vector3 GetWorldPositionFromUI_Perspective()
    {
        return GetWorldPositionFromUI_Perspective(Input.mousePosition, Camera.main);
    }

    public static Vector3 GetWorldPositionFromUI_Perspective(Camera worldCamera)
    {
        return GetWorldPositionFromUI_Perspective(Input.mousePosition, worldCamera);
    }

    public static Vector3 GetWorldPositionFromUI_Perspective(Vector3 screenPosition, Camera worldCamera)
    {
        Ray ray = worldCamera.ScreenPointToRay(screenPosition);
        Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, 0f));
        float distance;
        xy.Raycast(ray, out distance);
        return ray.GetPoint(distance);
    }

    public static Color GetRandomColor()
    {
        return new Color(UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f));
    }
}
