using UnityEngine;
using System.Collections;

public class UnitDeath : MonoBehaviour {

    private float timeSinceDeath;
    private float lerpTime;
    private float lerpDuration;
    private float maxTimeOnField;
    private Color initialColor;
    private Color endColor;
    private Transform rootObject;
    private Transform mainMesh;


	// Use this for initialization
	void Start () {
        this.GetComponent<Animator>().Play("Base Layer.DeathFire");
        timeSinceDeath = 0f;
        lerpTime = 0f;
        lerpDuration = 4f;
        maxTimeOnField = 3f;
        mainMesh = transform.FindChild("Mesh");
        rootObject = this.transform;
        initialColor = endColor = mainMesh.renderer.material.color;
        endColor.a = 0;
	}
	
	// Update is called once per frame
	void Update () {
        timeSinceDeath += Time.deltaTime;
        if (timeSinceDeath > maxTimeOnField)
        {
            for (int i = 0 ; i < rootObject.childCount ; i++ )
            {
                if (rootObject.GetChild(i).renderer != null)
                {
                    rootObject.GetChild(i).renderer.material.color = Color.Lerp(initialColor, endColor, lerpTime);
                }
            }
            if (lerpTime < 1)
            {
                lerpTime += Time.deltaTime / lerpDuration;
            }
            if (mainMesh.renderer.material.color.a == 0)
            {
                Destroy(this.gameObject);
            }
        }
	}

    private float generateTimeOnField()
    {
        float timeOnField = 0f;
        AnimatorStateInfo animInfo = this.GetComponent<Animator>().GetNextAnimatorStateInfo(0);
        return timeOnField;
    }
}
