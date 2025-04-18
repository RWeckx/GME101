
using UnityEngine;

public class HomingMissile : Bomb
{
    [SerializeField]
    private float _speedBoostWhenInRange = 1.7f;
    [SerializeField]
    private float _rotationOffset = 90f;
    private bool _targetAcquired = false;
    private bool _hasExploded = false;
    private GameObject _target;

    // Update is called once per frame
    void Update()
    {
        FindClosestTarget();

        if (_targetAcquired && _hasExploded == false && _target != null)
            if (_target.GetComponent<Enemy>().GetIsDead() == false)
                CalculateMovement();
            else 
                MoveLaser();
        else
            MoveLaser();
    }

    private void CalculateMovement()
    {
        transform.position = Vector3.MoveTowards(transform.position, _target.transform.position, _speed * _speedBoostWhenInRange * Time.deltaTime);
        RotateTowardsTarget();
    }

    private void FindClosestTarget()
    {
        if (_targetAcquired == false)
        {
            float previousDistance = 10;
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

            foreach (GameObject enemy in enemies)
            {
                if (enemy != null)
                {
                    float distance = Vector3.Distance(enemy.transform.position, transform.position);
                    if (distance < previousDistance)
                    {
                        previousDistance = distance;
                        _target = enemy;
                        _targetAcquired = true;
                    }
                }
            }
        }
    }

    private void RotateTowardsTarget()
    {
        Vector3 diff = _target.transform.position - transform.position;
        diff.Normalize();
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z - _rotationOffset);
    }
}
