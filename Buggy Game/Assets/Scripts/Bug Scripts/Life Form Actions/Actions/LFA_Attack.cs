using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LFA_Attack : LifeFormAction
{
    [Tooltip("Attack Root.")] [SerializeField]
    private Transform root;
    
    [Tooltip("Attack radius.")] [SerializeField]
    private GlobalFloat attackRadius;

    [Tooltip("Attack wind up time.")] [SerializeField]
    private GlobalFloat windUpTime;
    
    [Tooltip("Attack active time.")] [SerializeField]
    private GlobalFloat activeTime;
    
    [Tooltip("Attack cooldown time.")] [SerializeField]
    private GlobalFloat cooldownTime;

    [Tooltip("Attack Visuals. Should be set to destroy on their own.")] [SerializeField]
    private List<GameObject> visuals = new();

    public override IEnumerator PerformAction(Vector3 position = new Vector3())
    {
        if (Vector3.Distance(position, root.position) < attackRadius.CurrentValue)
        {
            yield return new WaitForSeconds(windUpTime.CurrentValue);

            float timer = 0;

            foreach (GameObject visual in visuals)
            {
                Instantiate(visual, root.position, Quaternion.identity);
            }
            while (timer < activeTime.CurrentValue)
            {
                Collider[] hits = new Collider[10];
                Physics.OverlapSphereNonAlloc(root.position, attackRadius.CurrentValue, hits);
                foreach (Collider hit in hits)
                {
                    if (hit && hit.gameObject != gameObject && hit.GetComponent<LifeFormDestruction>())
                    {
                        hit.GetComponent<LifeFormDestruction>().Destroy();
                    }
                }

                timer += Time.deltaTime;
                yield return null;
            }
            
            yield return new WaitForSeconds(cooldownTime.CurrentValue);
        }

        yield return null;
    }
}