using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 target;
    private Rigidbody rigidbody;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private ParticleSystem explosion;
    [SerializeField] private float time = 10f;
    [SerializeField] private GameObject visual;

    private bool isBeingDestroyed = false;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = this.GetComponent<Rigidbody>();
    }

    public void SetTarget(Vector3 _target)
    {
        target = _target.normalized;

        //this.transform.Translate(target);
        //rigidbody.velocity = target * 1000f;
    }

    private void Update()
    {  
        time--;

        if (isBeingDestroyed)
        {
            StartCoroutine("Destroy", 2f);
            return;
        }

        if (time <= 0)
        {
            explosion.Play();
            visual.SetActive(false);
            isBeingDestroyed = true;
        }
        else
        {
            this.transform.Translate(target * bulletSpeed * Time.deltaTime);
        }
    }

    private IEnumerator Destroy(float time)
    {
        yield return new WaitForSeconds(time);

        Destroy(this.gameObject);
    }
}
