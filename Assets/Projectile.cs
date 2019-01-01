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

    [SerializeField] private int damage = 10;

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            return;
        }

        Debug.Log("Hit Something!");

        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Oh hit an enemy!");
            IHittable hit = other.GetComponent<IHittable>();
            Debug.Log("Hitting enemy with " + damage + " damage!");
            hit.GetHit(damage);
        }

        Explode();
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
            Explode();
        }
        else
        {
            this.transform.Translate(target * bulletSpeed * Time.deltaTime);
        }
    }

    private  void Explode()
    {
        explosion.Play();
        visual.SetActive(false);
        isBeingDestroyed = true;
    }

    private IEnumerator Destroy(float time)
    {
        yield return new WaitForSeconds(time);

        Destroy(this.gameObject);
    }
}
