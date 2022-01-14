using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsSimulationData : ScriptableObject
{
    [HideInInspector] public int physicsTime;
    [HideInInspector] public bool randomForce;
    [HideInInspector] public int force;
    [HideInInspector] public bool simulate;
    [HideInInspector] public List<GameObject> objects = new List<GameObject>();

    private List<Vector3> startPos = new List<Vector3>();
    private List<Quaternion> startRot = new List<Quaternion>();
    private List<Rigidbody> allRigidBodies = new List<Rigidbody>();
    private List<Rigidbody> selectedRigidBodies = new List<Rigidbody>();
    private List<Rigidbody> outsideRB = new List<Rigidbody>();
    private float timer = 0;


    public void Init()
    {
        AddRigidbodyToSlected();
        GetAllRigidbodies();
    }

    private void AddRigidbodyToSlected()
    {
        selectedRigidBodies.Clear();

        for (int i = 0; i < objects.Count; i++)
        {
            if(!objects[i].GetComponent<Rigidbody>())
            {
                objects[i].AddComponent<Rigidbody>();
            }
        }

        for (int z = 0; z < objects.Count; z++)
        {
            selectedRigidBodies.Add(objects[z].GetComponent<Rigidbody>());
        }
    }

    private void GetAllRigidbodies()
    {
        allRigidBodies.Clear();
        startPos.Clear();
        startRot.Clear();
        outsideRB.Clear();

        allRigidBodies.AddRange(FindObjectsOfType<Rigidbody>());

        for (int i = 0; i < objects.Count; i++)
        {
            startPos.Add(objects[i].transform.position);
            startRot.Add(objects[i].transform.rotation);
        }

        for (int r = 0; r < allRigidBodies.Count; r++)
        {
            if(selectedRigidBodies.Contains(allRigidBodies[r]))
            {
                continue;
            }else{

                if(!allRigidBodies[r].isKinematic)
                {
                    outsideRB.Add(allRigidBodies[r]);
                    allRigidBodies[r].isKinematic = true;
                }
            }

        }
    }

    public void SimulatePhysics()
    {
        simulate = true;
    
        for (int i = 0; i < selectedRigidBodies.Count; i++)
        {
            selectedRigidBodies[i].velocity = Vector3.zero;

            if(randomForce)
            {
                Vector3 randomForceAmount = new Vector3(Random.Range(-force, force),Random.Range(-force, force),Random.Range(-force, force));
                selectedRigidBodies[i].AddForce(randomForceAmount, ForceMode.Impulse);
                selectedRigidBodies[i].AddTorque(randomForceAmount, ForceMode.Impulse);
            }
        }

        Physics.autoSimulation = false;
    }

    public void UpdatePhysics()
    {
        if(simulate){
            timer += 1 * Time.deltaTime;
            Physics.Simulate (Time.fixedDeltaTime*0.3f);
        }

        if(timer > physicsTime)
        {
            Physics.autoSimulation = true;
            simulate = false;
            timer = 0;

            RBState();
        }
    }

    public void ResetState()
    {
        simulate = false;
        timer = 0;

        for (int i = 0; i < objects.Count; i++)
        {
            if(objects[i] == null) continue;

            objects[i].transform.position = startPos[i];
            objects[i].transform.rotation = startRot[i];
        }

        RBState();
        
        Physics.autoSimulation = true;
    }

    public void Clear()
    {
        Physics.autoSimulation = true;
        simulate = false;
        timer = 0;
        
        objects = new List<GameObject>();
        startPos = new List<Vector3>();
        startRot = new List<Quaternion>();
        allRigidBodies = new List<Rigidbody>();
        selectedRigidBodies = new List<Rigidbody>();
        
        RBState();
    }

    private void RBState()
    {
        for (int i = 0; i < selectedRigidBodies.Count; i++)
        {
            DestroyImmediate(selectedRigidBodies[i]);
        }

        for (int z = 0; z < outsideRB.Count; z++)
        {
            outsideRB[z].isKinematic = false;
        }
    }
}