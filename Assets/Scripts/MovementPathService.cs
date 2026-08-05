using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class MovementPathService
{
    // Calcula la ruta y devuelve si es alcanzable y cuánto mide la caminata real
    public bool TryGetValidPath(Vector3 start, Vector3 target, float maxDistance, out NavMeshPath path, out float totalDistance)
    {
        path = new NavMeshPath();
        totalDistance = 0f;

        // 1. Pedimos a Unity que calcule el camino esquivando el mapa
        if (NavMesh.CalculatePath(start, target, NavMesh.AllAreas, path))
        {
            // 2. Calculamos la longitud total sumando la distancia entre cada "esquina" del camino
            totalDistance = CalculatePathLength(path);

            // 3. Si la ruta está completa y no supera los metros que le quedan al turno... ¡Es válida!
            if (path.status == NavMeshPathStatus.PathComplete && totalDistance <= maxDistance)
            {
                return true;
            }
        }

        Debug.Log("Invalid path");

        return false;
    }

    private float CalculatePathLength(NavMeshPath path)
    {
        float length = 0f;
        if (path.corners.Length < 2) return length;

        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            length += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        }
        return length;
    }

    public void MoveUnitAlongPath(BaseUnit unit, Vector3[] waypoints, float moveSpeed, UnityAction onMoveCompleted)
    {
        //variables necesarias para calcular el tiempo para velocidad de movimiento constante
        Vector3 lastPos = unit.transform.position;
        float segmentDistance = 0;
        float timeForSegment = 0;
        LTSeq moveSeq = LeanTween.sequence();
        for (int i = 0; i < waypoints.Length; i++)
        {
            Debug.Log($"Waypoint {i}: {waypoints[i]}");
            //añadir un offset a la posicion Y del waypoint
            waypoints[i] += new Vector3(0, unit.transform.position.y, 0);
            // 1. Calculamos la distancia de este tramo específico
            segmentDistance = Vector3.Distance(lastPos, waypoints[i]);

            // 2. Calculamos el tiempo basado en la velocidad que queramos
            timeForSegment = segmentDistance / moveSpeed;

            if (timeForSegment > 0.01f)
            {
                // Usamos setEase(LeanTweenType.linear) para que no acelere/frene en cada esquina
                moveSeq.append(unit.transform.LeanMove(waypoints[i], timeForSegment).setEase(LeanTweenType.linear));
            }

            lastPos = waypoints[i];
        }
        //añadir el callback al final de la secuencia
        moveSeq.append(() => onMoveCompleted?.Invoke());
    }
}