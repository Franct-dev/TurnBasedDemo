using UnityEngine;
using UnityEngine.AI;

public class UnitTesting : MonoBehaviour
{
    private MovementPathService mps;
    public BaseUnit unit;
    public float moveSpeed = 5;

    private void Start()
    {
        mps = new MovementPathService();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                if (mps.TryGetValidPath(unit.transform.position, hit.point, 9999, out NavMeshPath path, out float totalDistance))
                {
                    mps.MoveUnitAlongPath(unit, path.corners, moveSpeed, ()=> Debug.Log("Unit movement ended"));
                } 
            }
        }
    }
}
