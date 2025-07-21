using UnityEngine;

public  class PosConvertTool : MonoBehaviour
{
    /// <summary>
    /// 转换场景坐标到UI坐标（pos - anchoredPos）
    /// </summary>
    /// <param name="targetPos">场景点</param>
    /// <param name="camera">UI摄像机</param>
    /// <param name="canvas"></param>
    /// <returns></returns>
    public static Vector2 ConvertUIPos(Vector3 targetPos, Camera camera, Canvas canvas,
        RectTransform relativelyRect = null)
    {
        Vector3 m_WorldToScreenPoint = camera.WorldToScreenPoint(targetPos);

        Vector2 m_ProjPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)canvas.transform,
            m_WorldToScreenPoint, camera, out m_ProjPoint);
        return m_ProjPoint;
    }

    /// <summary>
    /// UI物体坐标转场景坐标
    /// </summary>
    /// <param name="targetPos"></param>
    /// <param name="uiCamera"></param>
    /// <param name="canvas"></param>
    /// <returns></returns>
    public static Vector3 ConverScreenPos(Vector3 targetPos, Canvas canvas, GameObject obj = null)
    {
        Vector3 UIWorldPos = targetPos;
        Vector3 m_ProjPoint = canvas.transform.localToWorldMatrix.MultiplyPoint(UIWorldPos);
        return m_ProjPoint;
    }

    /// <summary>
    /// 不同UI节点下的UI anchoredPosition坐标转换
    /// </summary>
    /// <param name="sourcePos"></param>
    /// <param name="targetUIParent"></param>
    /// <param name="uiCamera"></param>
    /// <returns></returns>
    public static Vector2 ConvertAnchoredPos(Vector3 sourcePos, RectTransform targetUIParent, Camera uiCamera)
    {
        Vector2 sourceScreenPos = uiCamera.WorldToScreenPoint(sourcePos);
        Vector2 anchoredPos = Vector2.zero;
        bool isSucess = RectTransformUtility.ScreenPointToLocalPointInRectangle(targetUIParent, sourceScreenPos, uiCamera, out anchoredPos);
        return anchoredPos;
    }

    public static Vector3 UI2WorldPos(Vector3 anchoredPos, Camera uiCamera, Camera main)
    {
        var screenPos = uiCamera.WorldToScreenPoint(anchoredPos);
        return main.ScreenToWorldPoint(screenPos);
    }

    /// <summary>
    /// 转换场景坐标到UI坐标(场景主摄像机和UI摄像机不一致时)
    /// </summary>
    /// <param name="sourcePos"></param> 场景物体坐标
    /// <param name="targetUIParent"></param> 目标UI父级
    /// <param name="mainCamera"></param>
    /// <param name="uiCamera"></param>
    /// <returns></returns>
    public static Vector2 ConvertAnchoredPosFromScene(Vector3 sourcePos, RectTransform targetUIParent, Camera mainCamera, Camera uiCamera)
    {
        Vector2 sourceScreenPos = mainCamera.WorldToScreenPoint(sourcePos);
        Vector2 anchoredPos = Vector2.zero;
        bool isSucess = RectTransformUtility.ScreenPointToLocalPointInRectangle(targetUIParent, sourceScreenPos, uiCamera, out anchoredPos);
        return anchoredPos;
    }
}