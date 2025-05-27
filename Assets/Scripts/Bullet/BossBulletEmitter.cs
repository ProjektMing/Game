using UnityEngine;
using System.Collections;

namespace Bullet
{
    /// <summary>
    /// Boss子弹发射器，实现复杂的弹幕模式
    /// 包含螺旋弹幕模式，可以根据Boss血量调整难度
    /// </summary>
    public class BossBulletEmitter : BulletEmitter
    {
        [Header("螺旋弹幕设置")] [SerializeField] private float rotationSpeed = 60f; // 旋转速度（度/秒）

        private Coroutine _currentPatternRoutine; // 当前弹幕协程
        private float _currentRotationSpeed; // 当前旋转速度

        /// <summary>
        /// 初始化Boss发射器
        /// </summary>
        private void Start()
        {
            Debug.Log("Boss Start初始化开始");

            // 确保有发射点
            if (firePoint == null)
            {
                Debug.Log("创建默认发射点");
                CreateDefaultFirePoints();
            }

            _currentRotationSpeed = rotationSpeed;
            baseFireRate = 0.15f; // 设置基础发射频率
            Debug.Log("启动SpiralPattern");
            StartCoroutine(SpiralPattern());
        }

        /// <summary>
        /// 创建默认的发射点配置
        /// </summary>
        private void CreateDefaultFirePoints()
        {
            var point = new GameObject("FirePoint");
            point.transform.parent = transform;
            point.transform.localPosition = Vector3.zero; // 发射点在Boss中心
            firePoint = point.transform;
            Debug.Log("创建了单个发射点");
        }

        /// <summary>
        /// 螺旋弹幕模式
        /// </summary>
        private IEnumerator SpiralPattern()
        {
            Debug.Log("SpiralPattern 开始执行");
            float angle = 0;

            while (true)
            {
                if (!canFire || Time.timeScale == 0)
                {
                    yield return new WaitForSeconds(0.1f);
                    continue;
                }

                // 计算旋转后的发射方向
                var direction = new Vector2(
                    Mathf.Cos(angle * Mathf.Deg2Rad),
                    Mathf.Sin(angle * Mathf.Deg2Rad)
                );

                Fire(direction, baseBulletSpeed, Bullet.BulletTag.Player);

                angle += _currentRotationSpeed * Time.deltaTime;
                yield return null;
            }
        }

        /// <summary>
        /// 根据Boss血量调整弹幕难度
        /// </summary>
        /// <param name="healthPercentage">Boss当前血量百分比（0-1）</param>
        public void AdjustDifficulty(float healthPercentage)
        {
            // 血量越低，发射速度越快
            var newFireRate = Mathf.Lerp(baseFireRate, baseFireRate * 0.5f, 1 - healthPercentage);
            SetFireRate(newFireRate);

            // 血量越低，子弹速度越快
            var newBulletSpeed = Mathf.Lerp(baseBulletSpeed, baseBulletSpeed * 1.5f, 1 - healthPercentage);
            SetBulletSpeed(newBulletSpeed);

            // 血量越低，旋转速度越快
            _currentRotationSpeed = Mathf.Lerp(rotationSpeed, rotationSpeed * 1.5f, 1 - healthPercentage);
        }

        /// <summary>
        /// 组件禁用时清理协程
        /// </summary>
        private void OnDisable()
        {
            // 确保在组件禁用时停止所有协程
            if (_currentPatternRoutine != null)
            {
                StopCoroutine(_currentPatternRoutine);
                _currentPatternRoutine = null;
            }

            StopAllCoroutines();
        }

        public override void EnableFiring()
        {
            base.EnableFiring();
            if (_currentPatternRoutine == null) _currentPatternRoutine = StartCoroutine(SpiralPattern());
        }

        public override void DisableFiring()
        {
            base.DisableFiring();
            if (_currentPatternRoutine != null)
            {
                StopCoroutine(_currentPatternRoutine);
                _currentPatternRoutine = null;
            }
        }
    }
}