using UnityEngine;

public static class JoyStickHelper
{
    /// <summary>
    /// 当前摇杆位置
    /// </summary>
    private static Vector2 curJoyStickPos;
    
    public enum JoyStickState
    {
        Move,
        Stop
    }
    
    private static JoyStickState joyStickState = JoyStickState.Stop;
    
    public static void SetCurJoyStickPos(Vector2 position)
    {
        curJoyStickPos = position;
    }

    public static Vector2 GetCurJoyStickPos()
    {
        return curJoyStickPos;
    }

    public static void SetJoyStickState(bool isMove = false)
    {
        joyStickState = isMove ? JoyStickState.Move : JoyStickState.Stop;
    }

    public static JoyStickState GetJoyStickState()
    {
        return joyStickState;
    }
}