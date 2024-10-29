using UnityEngine;

public class TestBall : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
            transform.position -= Vector3.forward * Time.deltaTime;
        
        if(Input.GetKey(KeyCode.D))
            transform.position += Vector3.right * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
            transform.position -= Vector3.right * Time.deltaTime;

        if (Input.GetKey(KeyCode.S))
            transform.position += Vector3.forward * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("BluePlayer") == true)
        {
            Debug.Log("Obronione");
            collision.gameObject.GetComponent<Rigidbody>().AddForce(-collision.contacts[0].point * 50);
        }
        else
            Debug.Log("Nie Obronione");
        

    }
}
