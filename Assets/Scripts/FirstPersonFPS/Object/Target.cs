using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : InteractionObject
{
    [SerializeField]
    float targetUpDelayTime = 3.0f;

    bool isPossibleHit = true;

    public override void TakeDamage(int damage)
    {
        currentHP -= damage;

        if(currentHP < 0 && isPossibleHit == true)
        {
            isPossibleHit = false;

            StartCoroutine("OnTargetDown");
        }
    }

    IEnumerator OnTargetDown()
    {
        yield return StartCoroutine(OnAimation(0, 90));

        StartCoroutine("OnTargetUp");
    }

    IEnumerator OnTargetUp()
    {
        yield return new WaitForSeconds(targetUpDelayTime);

        yield return StartCoroutine(OnAimation(90, 0));

        isPossibleHit = true;
    }

    IEnumerator OnAimation(float start, float end)
    {
        float percent = 0.0f;
        float current = 0.0f;
        float time = 1.0f;

        while(percent < 1)
        {
            current += Time.deltaTime;
            percent = current / time;

            transform.rotation = Quaternion.Slerp(Quaternion.Euler(start, 0, 0), Quaternion.Euler(end, 0, 0), percent);

            yield return null;
        }
    }
}
