using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject _unitPrefab;

    [SerializeField] private List<Unit> _units;

    private void Awake()
    {
        for (int i = 0; i < 9; i++)
        {
            GameObject unitGameObject = Instantiate(_unitPrefab, transform.position, transform.rotation, transform);

            AddUnit(unitGameObject.GetComponent<Unit>());
        }
    }

    public void AddUnit(Unit unit)
    {
        if (_units.Contains(unit)) return;

        _units.Add(unit);

        unit.transform.SetParent(transform);

        unit.transform.localRotation = Quaternion.identity;
    }

    public void RemoveUnit(Unit unit)
    {
        _units.Remove(unit);
    }

    public void ChangeShape(Vector3[] shape)
    {
        float step = (float)shape.Length / (float)_units.Count;

        for (int i = 0; i < _units.Count; i++)
        {
            int index = (int)(step * i);

            if (index >= shape.Length) index = shape.Length - 1;
            
            _units[i].SetNewPosition(shape[index]);
        }
    }
}
