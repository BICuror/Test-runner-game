using UnityEngine.Events;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public UnityEvent Joined;

    private bool _hasJoined;

    public void SetNewPosition(Vector3 position)
    {
        transform.localPosition = new Vector3(position.x, 0f, position.y);
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.TryGetComponent<Unit>(out Unit unit))
        {
            unit.JoinPlayer();
        }   
        else if (other.gameObject.TryGetComponent<Trap>(out Trap trap))
        {
            Die();
        }
    }

    public void JoinPlayer()
    {
        if (_hasJoined == false)
        {
            _hasJoined = true;

            Joined.Invoke();

            gameObject.layer = LayerMask.NameToLayer("JoinedUnit");

            FindObjectOfType<PlayerController>().AddUnit(this);
        }
    }

    private void Die()
    {
        transform.SetParent(null);

        GetComponent<Rigidbody>().AddForce(transform.up * 5f, ForceMode.Impulse);

        GetComponent<Rigidbody>().useGravity = true;

        GetComponent<Rigidbody>().angularVelocity = new Vector3(Random.Range(-360f, 360f), Random.Range(-360f, 360f), Random.Range(-360f, 360f));

        FindObjectOfType<PlayerController>().RemoveUnit(this);

        Destroy(gameObject, 3f);
    }
}
