using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WispExtensions
{
    public static class WispCursor
    {
        public static WispCursorState GetCursorState()
        {
            WispCursorState result = new WispCursorState();
            
            result.lockMode = Cursor.lockState;
            result.visible = Cursor.visible;

            return result;
        }

        public static void SetCursorState(WispCursorState ParamState)
        {
            Cursor.lockState = ParamState.lockMode;
            Cursor.visible = ParamState.visible;
        }
    }
}

public class WispCursorState
{
    public CursorLockMode lockMode = CursorLockMode.None;
    public bool visible = true;
}