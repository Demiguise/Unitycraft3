using UnityEngine;
using System.Collections;

public class UnitDebug : MonoBehaviour {

    private int uniqueID;

	// Use this for initialization
	void Start () {
        uniqueID = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetUID(int id)
    {
        uniqueID = id;
    }

    public void Log(string compName, string statement)
    {
        string toLog = "[" + uniqueID.ToString() + "][" + compName + "] " + statement;
        Debug.Log(toLog);
    }

    public void LogIfTrue(string compName, string statement, bool checkVar)
    {
        if (checkVar)
        {
            string toLog = "[" + uniqueID.ToString() + "][" + compName + "] " + statement;
            Debug.Log(toLog);
        }
    }
}
