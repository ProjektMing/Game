using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Level Settings")] public string nextLevelName = "Level2";

    [Tooltip("Total number of enemies expected to be spawned in this level/wave for completion.")]
    public int totalEnemiesForLevel = 5; // 这一关/波次总共会生成的敌人数量

    private int enemiesSpawnedSoFar = 0; // 到目前为止已经生成的敌人数量
    private int enemiesDefeatedSoFar = 0; // 到目前为止已经击败的敌人数量
    private bool allEnemiesSpawned = false; // 是否所有预期的敌人都已经生成完毕
    private bool levelTransitioning = false; // 防止重复加载场景

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        // DontDestroyOnLoad(gameObject); // 如果需要在场景切换时保留
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        ResetLevelStats();
        // 确保 totalEnemiesForLevel 是一个有效的值
        if (totalEnemiesForLevel <= 0)
            Debug.LogWarning(
                "LevelManager: totalEnemiesForLevel is not set to a positive value. Level completion might not work as expected.",
                this);
    }

    public void ResetLevelStats()
    {
        enemiesSpawnedSoFar = 0;
        enemiesDefeatedSoFar = 0;
        allEnemiesSpawned = false;
        levelTransitioning = false; // 重置场景转换标志
        Debug.Log($"LevelManager: Stats Reset. Total enemies for this level: {totalEnemiesForLevel}");
    }

    // 由 EnemyCreater 在生成一个敌人后调用
    public void ReportEnemySpawned()
    {
        if (levelTransitioning) return;

        enemiesSpawnedSoFar++;
        // Debug.Log($"LevelManager: Enemy Spawned. ({enemiesSpawnedSoFar}/{totalEnemiesForLevel})");

        // 检查是否所有预期的敌人都已生成
        if (enemiesSpawnedSoFar >= totalEnemiesForLevel)
        {
            allEnemiesSpawned = true;
            Debug.Log("LevelManager: All expected enemies have been spawned.");
            // 在所有敌人都生成后，如果此时场上已经没有敌人了（即之前生成的都被打死了），则立即检查过关
            CheckForLevelCompletion();
        }
    }

    // 由 Enemy 在 Die() 时调用
    public void ReportEnemyDefeated()
    {
        if (levelTransitioning) return;

        enemiesDefeatedSoFar++;
        Debug.Log($"LevelManager: Enemy Defeated. ({enemiesDefeatedSoFar}/{totalEnemiesForLevel})");

        CheckForLevelCompletion();
    }

    private void CheckForLevelCompletion()
    {
        if (levelTransitioning) return;

        // 条件：1. 所有预期的敌人都已经生成了
        //       2. 并且，所有已生成的敌人都已经被击败了 (即击败数等于总数)
        if (allEnemiesSpawned && enemiesDefeatedSoFar >= totalEnemiesForLevel)
        {
            Debug.Log("LevelManager: Level COMPLETED! All spawned enemies defeated.");
            levelTransitioning = true; // 设置标志，防止多次调用LoadNextLevel
            LoadNextLevel();
        }
        // 可选的调试信息，帮助理解状态
        // else
        // {
        //     Debug.Log($"LevelManager: Completion check: AllSpawned={allEnemiesSpawned}, Defeated={enemiesDefeatedSoFar}, Required={totalEnemiesForLevel}");
        // }
    }

    private void LoadNextLevel()
    {
        Debug.Log($"LevelManager: Loading next level - {nextLevelName}");
        if (!string.IsNullOrEmpty(nextLevelName))
        {
            SceneManager.LoadScene(nextLevelName);
        }
        else
        {
            Debug.LogError("LevelManager: Next level name is not set!", this);
            levelTransitioning = false; // 如果加载失败，允许再次尝试（或者其他错误处理）
        }
    }

    // 获取当前已生成敌人数量（供 EnemyCreater 参考，如果需要）
    public int GetTotalEnemiesForLevel()
    {
        return totalEnemiesForLevel;
    }
}