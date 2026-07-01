using UnityEngine;

public class PathFollower : MonoBehaviour
{
    public BelzierCurve path;
    public float speed = 2f;
    float t = 0f;

    void Update(){
        t += speed * Time.deltaTime;
        t = Mathf.Clamp01(t);

        transform.position = path.GetPoint(t);
        transform.forward = (path.GetPoint(t + 0.01f) - transform.position).normalized;
    }
}