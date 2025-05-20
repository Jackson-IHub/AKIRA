using UnityEngine;

public class DialogueRotation : MonoBehaviour
{

    private Camera cam;
    public bool isChild;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(cam.transform);
        if(isChild)
        {
            this.transform.localEulerAngles = new Vector3(this.transform.localEulerAngles.x, this.transform.localEulerAngles.y + 15, this.transform.localEulerAngles.z);
        }
        else
        {
            this.transform.localEulerAngles = new Vector3(this.transform.localEulerAngles.x, this.transform.localEulerAngles.y - 15, this.transform.localEulerAngles.z);
        }
       
    }
}
