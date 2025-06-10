using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private GameObject destroyEffectPrefab;
    [SerializeField] private GameObject smokeEffectPrefab;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Controllable"))
        {
            PlayerMovement unit = collision.GetComponent<PlayerMovement>();
            if (unit != null)
            {
                SoundManager.Instance.PrintSoundEffect("melt");
                unit.enabled = false;
                unit.Die();

                if (destroyEffectPrefab != null)
                {
                    GameObject fx = Instantiate(destroyEffectPrefab, transform.position, Quaternion.identity);
                    Destroy(fx, 2f);
                }
                if (smokeEffectPrefab != null)
                {
                    GameObject fx2 = Instantiate(smokeEffectPrefab, collision.transform.position, Quaternion.identity);
                    Destroy(fx2, 2f);
                }

                this.gameObject.SetActive(false);
            }
        }
    }
}
