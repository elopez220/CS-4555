using UnityEngine;
using UnityEditor.Build.Content;


public class Dart : MonoBehaviour
{
    public float damage = 1f;
    public GameObject popEffect;

    [System.Obsolete]
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Balloon"))
        {
            Balloon balloon = collision.gameObject.GetComponent<Balloon>();
            if (balloon != null)
            {
                balloon.PopBalloon();
            }
            Destroy(gameObject);
        }
    }
}
