using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FightSequence());
    }

    private IEnumerator FightSequence()
    {
        while (true)
        {
            yield return new WaitForSeconds(2.0f);
            gameObject.GetComponent<Animator>().Play("BattleCamFrame1");
            yield return new WaitForSeconds(7.5f);
            gameObject.GetComponent<Animator>().Play("BattleCamFrame2");
            yield return new WaitForSeconds(5.5f);
            gameObject.GetComponent<Animator>().Play("BattleCamFrame3");
            yield return new WaitForSeconds(5.5f);
            gameObject.GetComponent<Animator>().Play("BattleCamFrame4");
            yield return new WaitForSeconds(8.5f);
        }
    }
}
