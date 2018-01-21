using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeting : MonoBehaviour {

    public List<Transform> targets;
    private Transform selectedTarget;

    private Transform myTransform;

    public bool isTargeting;

	// Use this for initialization
	void Start () {
        myTransform = transform;
        targets = new List<Transform>();
        selectedTarget = null;
        AddAllEnemies();
	}

    public void AddAllEnemies()
    {
        GameObject[] go = GameObject.FindGameObjectsWithTag("Enemy");
        foreach( GameObject enemy in go)
        {
            AddTarget(enemy.transform);
        }
    }

    public void AddTarget(Transform enemy)
    {
        targets.Add(enemy);
    }

    private void SortTargetsByDistance()
    {
        targets.Sort(delegate(Transform t1, Transform t2) {
            return (Vector3.Distance(t1.position, myTransform.position).CompareTo(Vector3.Distance(t2.position, myTransform.position)));
        });
    }

    void TargetEnemy()
    {
        if (selectedTarget == null)
        {
            SortTargetsByDistance();
            selectedTarget = targets[0];
        }
        else
        {
            int index = targets.IndexOf(selectedTarget);

            if (index < targets.Count - 1)
            {
                index++;
            }
            else
            {
                index = 0;
            }
            selectedTarget = targets[index];
            myTransform.LookAt(targets[index]);
        }
    }

	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.F))
        {
            TargetEnemy();
            isTargeting = true;
        }
	}
}
