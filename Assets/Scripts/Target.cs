using UnityEngine;
using Unity.MLAgents;

public class Target : MonoBehaviour
{
    void FixedUpdate()
    {
        gameObject.transform.Rotate(new Vector3(0, 0, 0), 0.0);
    }

    void OnTriggerEnter(Collider collision)
    {
        var agent = collision.gameObject.GetComponent<Agent>();
        if (agent != null)
        {
            agent.AddReward(1f);
            Respawn();
        }
    }

    public void Respawn()
    {
        gameObject.transform.localPosition = new Vector3(
            (1 - 2 * Random.value) * 5f,
            // 2f + Random.value * 5f,
            0.5f,
            (1 - 2 * Random.value) * 5f
        );
    }
}
