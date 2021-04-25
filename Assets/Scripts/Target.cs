using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class Target : Agent
{
    public GameObject tagger;
    public GameObject agentObject;
    public float strength = 350f;

    Rigidbody agentRigidbody;
    Vector3 orientation;
    // float jumpCoolDown;
    int totalSteps = 20;
    int stepsLeft = 20;

    EnvironmentParameters defaultParams;

    public override void Initialize()
    {
        agentRigidbody = gameObject.GetComponent<Rigidbody>();
        orientation = Vector3.zero;
        defaultParams = Academy.Instance.EnvironmentParameters;

        ResetParameters();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(tagger.transform.position);
        sensor.AddObservation(agentObject.transform.position);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        for (var i = 0; i < vectorAction.Length; i++)
        {
            vectorAction[i] = Mathf.Clamp(vectorAction[i], -1f, 1f);
        }
        float x = vectorAction[0];
        // float y = ScaleAction(vectorAction[1], 0, 1);
        float y = 0f;
        float z = vectorAction[1];
        agentRigidbody.AddForce(new Vector3(x, y, z) * strength);

        AddReward(0.05f * (
            vectorAction[0] * vectorAction[0] + 
            0 + 
            vectorAction[1] * vectorAction[1]) / 2f);
        
        orientation = new Vector3(x, y, z);
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Horizontal");
        // actionsOut[1] = Input.GetKey(KeyCode.Space) ? 1.0f : 0.0f;
        actionsOut[1] = Input.GetAxis("Vertical");
    }

    public override void OnEpisodeBegin()
    {
        gameObject.transform.localPosition = new Vector3(
            (1 - 2 * Random.value) * 5, 0.5f, (1 - 2 * Random.value) * 5
        );
        agentRigidbody.velocity = Vector3.zero;
        var environment = gameObject.transform.parent.gameObject;
        var targets = environment.GetComponentsInChildren<Target>();
        foreach (var t in targets)
        {
            t.Respawn();
        }
        stepsLeft = totalSteps;
        ResetParameters();
    }

    private void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, new Vector3(0f, -1f, 0f), 0.51f))
        {
            RequestDecision();
            stepsLeft -= 1;
            // jumpCoolDown = 0.1f;
            agentRigidbody.velocity = default(Vector3);
        }

        // jumpCoolDown -= Time.fixedDeltaTime;

        if (gameObject.transform.position.y < -1)
        {
            AddReward(-1);
            EndEpisode();
            return;
        }

        if (gameObject.transform.localPosition.x < -17 || gameObject.transform.localPosition.x > 17
            || gameObject.transform.localPosition.z < -17 || gameObject.transform.localPosition.z > 17)
        {
            AddReward(-1);
            EndEpisode();
            return;
        }
        if  (stepsLeft == 0)
        {
            EndEpisode();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (orientation.magnitude > float.Epsilon)
        {
            agentObject.transform.rotation = Quaternion.Lerp(agentObject.transform.rotation,
                Quaternion.LookRotation(orientation), Time.deltaTime * 10f);
        }
    }

    public void ResetParameters()
    {
        var targetScale = defaultParams.GetWithDefault("target_scale", 1.0f);
        tagger.transform.localScale = new Vector3(targetScale, targetScale, targetScale);
    }

    void OnTriggerEnter(Collider collision)
    {
        var agent = collision.gameObject.GetComponent<Agent>();
        if (agent != null)
        {
            agent.AddReward(1f);
            AddReward(-1f);

            MeshRenderer renderer = agentObject.GetComponent<MeshRenderer>();
            renderer.material.color = Color.red;

            Respawn();
        }
    }

    public void Respawn()
    {
        gameObject.transform.localPosition = new Vector3(
            (1 - 2 * Random.value) * 5f,
            0.5f,
            (1 - 2 * Random.value) * 5f
        );
    }
}
