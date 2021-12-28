using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorManager : MonoBehaviour
{
    static CursorManager instance;
    [SerializeField]
    public Texture2D defaulfCursor;
    [SerializeField]
    public Texture2D clickCursor;
    public Vector2 hotspot;
    CursorMode mode;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        Cursor.SetCursor(defaulfCursor, hotspot, CursorMode.ForceSoftware);
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.SetCursor(clickCursor, hotspot, CursorMode.ForceSoftware);
        }

        if (Input.GetMouseButtonUp(0))
        {
            Cursor.SetCursor(defaulfCursor, hotspot, CursorMode.ForceSoftware);
        }

    }
}
