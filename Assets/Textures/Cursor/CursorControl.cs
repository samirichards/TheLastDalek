using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorControl : MonoBehaviour
{
    public Texture2D MainCursor;
    public Texture2D CursorHighlight;
    public Texture2D CursorHighlightAlt;
    // Start is called before the first frame update
    void Start()
    {
        SetCursorStandard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCursorStandard()
    {
        Cursor.SetCursor(MainCursor, Vector2.zero, CursorMode.Auto);
    }
}
