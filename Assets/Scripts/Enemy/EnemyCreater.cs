using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Pool; // 需要 List
using UnityEngine.SceneManagement; // 需要场景管理

public class EnemyCreater : MonoBehaviour
{
    [Header("Path Settings")] public Transform[] pathControlPoints = new Transform[4];
    public float enemyPathDuration = 10f;

    [Header("Wave Settings")] public int numberOfEnemiesInWave = 5; // 确保这是你第一关需要的数量
    public float spawnInterval = 1.0f;
    public float initialSpawnTOffset = 0f;

    [Header("Object Pool Settings")] public string enemyPoolTag = "Enemy";

    [Header("Level Transition")] public Scene nextScene;
    public string nextSceneName = "level2"; // 下一关的场景名称]


    private List<GameObject> activeEnemies = new(); // 存储当前活跃的敌人
    private int enemiesSpawnedThisWave = 0;
    private bool waveSpawnComplete = false;
    private bool transitioningToNextScene = false; // 防止重复跳转

    private void Start()
    {
        if (ObjectPooler.Instance == null)
        {
            Debug.LogError("ObjectPooler.Instance not found!");
            enabled = false;
            return;
        }

        if (pathControlPoints == null || pathControlPoints.Length < 4 ||
            pathControlPoints[0] == null || pathControlPoints[1] == null ||
            pathControlPoints[2] == null || pathControlPoints[3] == null)
        {
            Debug.LogError("Path control points not set up correctly!", gameObject);
            enabled = false;
            return;
        }

        if (nextScene == null)
            Debug.LogWarning("Next scene name is not set in EnemyCreater. Level transition will not occur.", this);

        StartCoroutine(SpawnEnemyWave());
    }

    private IEnumerator SpawnEnemyWave()
    {
        activeEnemies.Clear();
        enemiesSpawnedThisWave = 0;
        waveSpawnComplete = false;
        transitioningToNextScene = false;

        Debug.Log($"EnemyCreater: Starting wave of {numberOfEnemiesInWave} enemies.");

        for (var i = 0; i < numberOfEnemiesInWave; i++)
        {
            var spawnStartT = initialSpawnTOffset; // 让所有敌人从同一起点开始，除非你有更复杂的逻辑
            // 如果你想让每个敌人起始点错开:
            // float spawnStartT = initialSpawnTOffset + (i * someOffsetPerEnemy);
            spawnStartT = Mathf.Clamp01(spawnStartT);

            var spawnPosition = Move.GetPointOnCubicBezier(
                pathControlPoints[0].position, pathControlPoints[1].position,
                pathControlPoints[2].position, pathControlPoints[3].position,
                spawnStartT
            );

            var enemyInstance = ObjectPooler.Instance.SpawnFromPool(enemyPoolTag, spawnPosition, Quaternion.identity);


            if (enemyInstance != null)
            {
                activeEnemies.Add(enemyInstance); // 将生成的敌人添加到活跃列表
                enemiesSpawnedThisWave++;

                // Enemy enemyScript = enemyInstance.GetComponent<Enemy>();
                // if (enemyScript != null)
                // {
                //    enemyScript.spawner = this; // 如果采用Enemy回调方式
                // }

                var follower = enemyInstance.GetComponent<Move>();
                if (follower != null)
                    follower.InitializePath(pathControlPoints, enemyPathDuration, spawnStartT);
                else
                    Debug.LogError($"Spawned enemy '{enemyInstance.name}' does not have a Move component!",
                        enemyInstance);
            }
            else
            {
                Debug.LogError($"Failed to spawn enemy {i + 1} from pool.");
            }

            yield return new WaitForSeconds(spawnInterval);
        }

        waveSpawnComplete = true;
        Debug.Log("EnemyCreater: Enemy wave spawning complete.");
        // 第一次检查，万一所有敌人在生成完毕瞬间就已经被清除了
        CheckAllEnemiesDefeated();
    }

    // 这个方法会在每一帧被调用，或者你也可以改成一个固定时间间隔的协程来检查
    private void Update() // 或者你可以用一个专门的 test() 方法，但需要在某处调用它
    {
        if (!transitioningToNextScene) // 只有在没有进行场景转换时才检查
            CheckAllEnemiesDefeated();
    }

    public void CheckAllEnemiesDefeated() // 改为 public 以便其他地方可能调用，或者保持 private 由 Update 调用
    {
        if (!waveSpawnComplete || transitioningToNextScene) // 如果敌人还没生成完，或者已在跳转，则不检查
            return;

        // 移除已经返回对象池（即非激活状态）的敌人
        // 从后往前遍历，因为在遍历过程中移除元素会导致问题
        for (var i = activeEnemies.Count - 1; i >= 0; i--)
            if (!activeEnemies[i] || !activeEnemies[i].activeInHierarchy)
            {
                Debug.Log("enemy defeated");
                activeEnemies.RemoveAt(i);
            }

        // 如果所有计划的敌人都已生成，并且活跃敌人列表为空
        if (enemiesSpawnedThisWave >= numberOfEnemiesInWave && activeEnemies.Count == 0)
        {
            Debug.Log("All enemies defeated! Transitioning to next level.");
            transitioningToNextScene = true; // 设置标志，防止重复调用
            LoadNextScene();
        }
    }


    private void LoadNextScene()
    {
        // SceneManager.LoadScene(nextScene.name);

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("Cannot load next scene: nextSceneName is not set in EnemyCreater.", this);
            transitioningToNextScene = false; // 如果加载失败，重置标志
        }
    }


    // OnDrawGizmosSelected (保持不变)
    private void OnDrawGizmosSelected()
    {
        if (pathControlPoints == null || pathControlPoints.Length < 4 ||
            pathControlPoints[0] == null || pathControlPoints[1] == null ||
            pathControlPoints[2] == null || pathControlPoints[3] == null)
            return;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(pathControlPoints[0].position, pathControlPoints[1].position);
        Gizmos.DrawLine(pathControlPoints[2].position, pathControlPoints[3].position);
        Gizmos.color = Color.cyan;
        var previousPoint = pathControlPoints[0].position;
        var segments = 30;
        for (var i = 1; i <= segments; i++)
        {
            var t_param = (float)i / segments;
            var currentPoint = Move.GetPointOnCubicBezier(pathControlPoints[0].position, pathControlPoints[1].position,
                pathControlPoints[2].position, pathControlPoints[3].position, t_param);
            Gizmos.DrawLine(previousPoint, currentPoint);
            previousPoint = currentPoint;
        }
    }

    // 你原来的 test() 方法可以移除，因为 Update() 中会调用 CheckAllEnemiesDefeated()
    // void test()
    // {
    // }
}