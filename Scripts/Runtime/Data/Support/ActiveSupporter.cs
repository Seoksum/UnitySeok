using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSupporter : MonoBehaviour
{
    public List<GameObject> targets = new List<GameObject>();

    public float lateTime = 1;

    private IEnumerator m_lateSetActive_Routine;

    public void SetActive(bool isActive)
    {
        for (int cnte = 0; cnte < targets.Count; cnte++)
        {
            targets[cnte].SetActive(isActive);
        }
    }
    public void LateSetActive(bool isActive)
    {
        if (m_lateSetActive_Routine != null)
            StopCoroutine(m_lateSetActive_Routine);
        m_lateSetActive_Routine = LateSetActive_Routine(isActive);

        StartCoroutine(m_lateSetActive_Routine);
    }

    private IEnumerator LateSetActive_Routine(bool isActive)
    {
        yield return new WaitForSeconds(lateTime);

        SetActive(isActive);
    }
}
