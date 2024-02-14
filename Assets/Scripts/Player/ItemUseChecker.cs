using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUseChecker : MonoBehaviour
{
    public Action<IInteracable> onItemUse;


    private void OnTriggerEnter(Collider other)
    {
        Transform target = other.transform;
        IInteracable obj = null;

        do
        {
            obj = target.GetComponent<IInteracable>();  // obj = Ÿ���� IIntercable ������Ʈ
            target = target.parent; // Ÿ�� = Ÿ���� �θ�
        } while (obj == null && target != null); // obj�� ã�Ұų� ���̻� �θ� ������ ���� ����

        if (obj != null)
        {
            onItemUse?.Invoke(obj); // IInteracable�� �ִ� ������Ʈ�� ����޴ٰ� �˸�
        }
    }
}

