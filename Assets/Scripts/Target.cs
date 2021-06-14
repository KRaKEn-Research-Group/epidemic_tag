using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.Barracuda;
using Unity.MLAgents.Sensors;


public class Target : Agent
{

    public GameObject tagger;
    public GameObject agentObject;
    public float strength = 350f;
    public NNModel modelSource;

    Rigidbody agentRigidbody;
    Vector3 orientation;
    int totalSteps = 100;
    int stepsLeft = 100;

    static int nrOfAgents = 0;

    EnvironmentParameters defaultParams;

    public override void Initialize()
    {
        nrOfAgents = nrOfAgents + 1;

        agentRigidbody = gameObject.GetComponent<Rigidbody>();
        orientation = Vector3.zero;
        defaultParams = Academy.Instance.EnvironmentParameters;

        ResetParameters();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(tagger.transform.position);
        sensor.AddObservation(agentObject.transform.position);
        var environment = gameObject.transform.parent.gameObject;
        // var targets = environment.GetComponentsInChildren<>();
        // for (var i = 0; i < targets.Length; i++) {
        //     sensor.AddObservation(targets[i].transform.position);
        // }
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        for (var i = 0; i < vectorAction.Length; i++)
        {
            vectorAction[i] = Mathf.Clamp(vectorAction[i], -1f, 1f);
        }
        float x = vectorAction[0];
        float y = 0f;
        float z = vectorAction[1];

        AddReward(0.05f * (
            vectorAction[0] * vectorAction[0] + 
            0 + 
            vectorAction[1] * vectorAction[1]) / 2f);
        
        orientation = new Vector3(x, y, z);
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
    }

    public override void OnEpisodeBegin()
    {
        Debug.Log("on episode begin - target");

        MeshRenderer renderer = agentObject.GetComponent<MeshRenderer>();
        renderer.material.color = Color.green;

        gameObject.transform.localPosition = new Vector3(
            (1 - 2 * UnityEngine.Random.value) * 5, 0.5f, (1 - 2 * UnityEngine.Random.value) * 5
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
            agentRigidbody.velocity = default(Vector3);
        }

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
            return;
        }

        var environment = gameObject.transform.parent.gameObject;
        var targets = environment.GetComponentsInChildren<Target>();

        int howMany = 0;
        foreach (var t in targets)
        {
            if (t.GetComponent<MeshRenderer>().material.color == Color.green)
            {
                howMany = howMany + 1;
            }
        }

        if (howMany == 0)
        {
            Debug.Log("how many == 0 target" + "\tDebug on " + agentObject.name);
            // foreach (var t in targets)
            // {
            //     if (t.GetComponent<MeshRenderer>().material.color == Color.green)
            //     {
            //         t.EndEpisode();
            //         return;
            //     }
            // }
            EndEpisode();
            return;
        }

    }

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
        // var agent_tag = collision.gameObject.tag;
        if (collision.gameObject.CompareTag("Tagger"))
        {
            agent.AddReward(1f);
            AddReward(-1f);

            MeshRenderer renderer = agentObject.GetComponent<MeshRenderer>();
            renderer.material.color = Color.red;
            agentObject.tag = "Tagger";

            // NNModel model = ModelLoader.Load(modelSource);            
            // NNModel model = Resources.Load("ML-Agents/Models/TaggerBehavior.onnx") as NNModel;

            SetModel("TaggerBehavior", modelSource);
            Respawn();
        }
    }

    public void Respawn()
    {
        gameObject.transform.localPosition = new Vector3(
            (1 - 2 * UnityEngine.Random.value) * 5f,
            0.5f,
            (1 - 2 * UnityEngine.Random.value) * 5f
        );
    }
}
