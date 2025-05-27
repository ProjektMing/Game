using UnityEngine;

public class Win : MonoBehaviour
{
    public GameObject WinInterface;

    private void Start()
    {
        WinInterface.SetActive(false);
    }

    public void 显示胜利界面()
    {
        WinInterface.SetActive(true);
    }
}