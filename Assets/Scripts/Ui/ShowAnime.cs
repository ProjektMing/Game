using UnityEngine;

public class SlideInEffect : MonoBehaviour
{
    public float duration = 1.0f; 
    public float targetY = -2.0f; 

    private Vector3 startPos;
    private Vector3 endPos;
    private float startTime;
    private bool isSliding = false;

    private void Start()
    {
        startPos = new Vector3(transform.position.x, -7f, transform.position.z);
        endPos = new Vector3(transform.position.x, targetY, transform.position.z);


        transform.position = startPos;


        StartSlide();
    }

    public void StartSlide()
    {
        startTime = Time.time;
        isSliding = true;
    }

    private void Update()
    {
        if (isSliding)
        {
            var timePassed = Time.time - startTime;
            var progress = Mathf.Clamp01(timePassed / duration);

            transform.position = Vector3.Lerp(startPos, endPos, EaseOutQuad(progress));

            if (progress >= 1.0f) isSliding = false;
        }
    }

    private float EaseOutQuad(float t)
    {
        return t * (2 - t);
    }
}