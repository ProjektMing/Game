using UnityEngine;

public class Move : MonoBehaviour
{
    public Transform[] controlPoints = new Transform[4];
    public float fullPathDuration = 5f; // 走完完整路径 (t=0 到 t=1) 所需时间
    public bool loop = false;
    public bool orientToPath = true; // 物体是否朝向路径方向

    private float currentTime = 0f;
    private float currentNormalizedT = 0f; // 当前在曲线上的参数 t (0 to 1)
    private Vector3 lastPosition;
    private bool isInitialized = false;

    // Spawner会调用这个方法
    public void InitializePath(Transform[] pathControlPoints, float totalDurationForFullPath,
        float startNormalizedTimeT = 0f)
    {
        if (pathControlPoints == null || pathControlPoints.Length < 4 ||
            pathControlPoints[0] == null || pathControlPoints[1] == null ||
            pathControlPoints[2] == null || pathControlPoints[3] == null)
        {
            Debug.LogError("BezierFollower: Control points for initialization are not set up correctly!", gameObject);
            enabled = false;
            return;
        }

        controlPoints = pathControlPoints;
        fullPathDuration = totalDurationForFullPath > 0 ? totalDurationForFullPath : 0.01f; // 防止除以0
        currentNormalizedT = Mathf.Clamp01(startNormalizedTimeT);
        currentTime = currentNormalizedT * fullPathDuration; // 根据起始t值设置当前时间

        transform.position = GetPointOnCubicBezier(
            controlPoints[0].position, controlPoints[1].position,
            controlPoints[2].position, controlPoints[3].position,
            currentNormalizedT
        );
        lastPosition = transform.position;
        isInitialized = true;
        enabled = true; // 确保脚本是激活的
    }

    private void Update()
    {
        if (!isInitialized || controlPoints == null || controlPoints.Length < 4 ||
            controlPoints[0] == null || controlPoints[1] == null ||
            controlPoints[2] == null || controlPoints[3] == null) // 再次检查以防万一
            return;

        currentTime += Time.deltaTime;
        currentNormalizedT = currentTime / fullPathDuration;

        if (currentNormalizedT > 1f)
        {
            if (loop)
            {
                currentNormalizedT = 1f; // 完成当前循环
                ApplyPositionAndOrientation(currentNormalizedT); // 更新到终点位置

                currentTime = 0f; // 重置时间
                currentNormalizedT = 0f; // t也归零开始新的循环
                // 如果需要，可以重新初始化位置到起点
                // transform.position = GetPointOnCubicBezier(controlPoints[0].position, ... , 0);
                // lastPosition = transform.position;
            }
            else
            {
                currentNormalizedT = 1f; // 停在终点
                // 可以选择在此处禁用脚本或执行其他操作（例如，通知Spawner此敌人已完成路径）
                // gameObject.SetActive(false); // 例如，返回对象池
                // enabled = false;
            }
        }

        currentNormalizedT = Mathf.Clamp01(currentNormalizedT); // 确保t在0-1之间
        ApplyPositionAndOrientation(currentNormalizedT);
    }

    private void ApplyPositionAndOrientation(float t)
    {
        var newPosition = GetPointOnCubicBezier(controlPoints[0].position, controlPoints[1].position,
            controlPoints[2].position, controlPoints[3].position, t);
        transform.position = newPosition;

        if (orientToPath && newPosition != lastPosition)
        {
            var direction = (newPosition - lastPosition).normalized;
            if (direction != Vector3.zero)
            {
                var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle - 90f); // Sprite默认朝上则-90
            }
        }

        lastPosition = newPosition;
    }

    public static Vector3 GetPointOnCubicBezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        var oneMinusT = 1f - t;
        return
            oneMinusT * oneMinusT * oneMinusT * p0 +
            3f * oneMinusT * oneMinusT * t * p1 +
            3f * oneMinusT * t * t * p2 +
            t * t * t * p3;
    }

    private void OnDrawGizmos()
    {
        if (controlPoints == null || controlPoints.Length < 4 ||
            controlPoints[0] == null || controlPoints[1] == null ||
            controlPoints[2] == null || controlPoints[3] == null)
            return;
        Gizmos.color = Color.gray;
        Gizmos.DrawLine(controlPoints[0].position, controlPoints[1].position);
        Gizmos.DrawLine(controlPoints[2].position, controlPoints[3].position);
        Gizmos.color = Color.green;
        var previousPoint = controlPoints[0].position;
        var segments = 30;
        for (var i = 1; i <= segments; i++)
        {
            var t_param = (float)i / segments;
            var currentPoint = GetPointOnCubicBezier(controlPoints[0].position, controlPoints[1].position,
                controlPoints[2].position, controlPoints[3].position, t_param);
            Gizmos.DrawLine(previousPoint, currentPoint);
            previousPoint = currentPoint;
        }
    }
}