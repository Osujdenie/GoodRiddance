using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlatformGeneration : MonoBehaviour
{
    [SerializeField] private List<float> forward; // �������� ��� ��������� �������
    [SerializeField] private List<float> left; // �������� ��� ��������� �����
    [SerializeField] private List<float> right; // �������� ��� ��������� ������

    [SerializeField] private float spawnTimer; // ����� ����� ����� �������� ����� ���������

    private bool canGenerate = true;
    private List<int> directionsLog = new();
    private string currentDirection;
    private string previousDirection;
    private int currentLayerOrder = 0;

    private void Start()
    {
        // ������� ���������� ��������� ��� ������ ����
        LastPosition.lastBox = new() { 0, 0, 0 };

        // ���������� ������� ����������� �������� ��������
        directionsLog = new() { 0,0,0,0,0,0,0,0,0,0,0,0 };

        // ����� ������ ���������
        PlatformSpawner();
    }

    void Update()
    {
        // canGenerate ��������� � true ������ spawnTimer ������, �������� �������� ����� ���������
        if (canGenerate) StartCoroutine(GeneratorDelay());
    }

    private IEnumerator GeneratorDelay()
    {
        canGenerate = false;
        yield return new WaitForSeconds(spawnTimer);
        PlatformSpawner();
        canGenerate = true;
    }

    // ������� ����� ���������
    private void PlatformSpawner()
    {
        // ������� �� ���� ��������� ��� ������ ����� ���� �������� �� �������
        var platform = PlatformPool.Instance.Get(); 
        
        // �������� ��������� ��������� ���������
        List<float> direction = SetPosition();
        Vector3 position = new(LastPosition.lastBox[0] + direction[0], LastPosition.lastBox[1] + direction[1], LastPosition.lastBox[2]);
        platform.transform.SetPositionAndRotation(position, new Quaternion());

        // ������� ������� � ���� ��������� platform
        platform.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = SetLayerOrder();
        platform.gameObject.SetActive(true);

        // ��������� ���������� ��������� ��������� � �������� �����, ������ ������� ���������
        LastPosition.lastBox.Clear();
        LastPosition.lastBox = new() { platform.gameObject.transform.position.x,
            platform.gameObject.transform.position.y,
            platform.gameObject.transform.position.z };
    }

    // ������� �������� ��������� (�����, ������, �������) � ������� ������ �������� ��������� ���������
    private List<float> SetPosition()
    {
        List<float> direction = new();
        List<float> position = ChooseNextPositionAlgorythm();
        LastPosition.previousPlatformDirection = previousDirection;
        LastPosition.plarformDirection = currentDirection;
        direction.Add(position[0]);
        direction.Add(position[1]);
        return direction;
    }

    // ������� ������� �� ���� ������������ ������� ���������
    private int SetLayerOrder()
    {
        if (currentDirection == "forward") 
        {
            currentLayerOrder -= 1;
            return currentLayerOrder - 1;
        }
        if (currentDirection == "left")
        {
            currentLayerOrder -= 1;
            return currentLayerOrder - 1;
        }
        if (currentDirection == "right")
        {
            currentLayerOrder += 1;
            return currentLayerOrder + 1;
        }
        return currentLayerOrder;
    }

    // �������� ������ �����������, � ������ ������ �������� ��������� ���������
    private List<float> ChooseNextPositionAlgorythm()
    {
        previousDirection = currentDirection;
        int directionSum = directionsLog.Sum();
        int selector = Random.Range(0, 2);
        List<float> selection = new();

        // -1 ����, 0 �����, 1 �����
        if (directionSum == 0)
        {
            if (selector == 0)
            {
                selection = new() 
                {
                    left[0],
                    left[1]
                };
                currentDirection = "left";
                directionsLog.Add(-1);
            }
            else
            {
                selection = new()
                {
                    right[0],
                    right[1]
                };
                currentDirection = "right";
                directionsLog.Add(1);
            }
        }
        else if (directionSum < 0)
        {
            if (selector == 0)
            {
                selection = new()
                {
                    forward[0],
                    forward[1]
                };
                currentDirection = "forward";
                directionsLog.Add(0);
            }
            else
            {
                selection = new()
                {
                    right[0],
                    right[1]
                };
                currentDirection = "right";
                directionsLog.Add(1);
            }
        }
        else
        {
            if (selector == 0)
            {
                selection = new()
                {
                    forward[0],
                    forward[1]
                };
                currentDirection = "forward";
                directionsLog.Add(0);
            }
            else
            {
                selection = new()
                {
                    left[0],
                    left[1]
                };
                currentDirection = "left";
                directionsLog.Add(-1);
            }
        }
        int balancer = DirectionBalancer();
        if (balancer == 1)
        {
            selection = new()
            {
                forward[0],
                forward[1]
            };
            currentDirection = "forward";
            directionsLog.Add(0);
        }
        return selection;
    }

    // ������������ ����������������� ����-����� � ����� ����� ��������
    private int DirectionBalancer()
    {
        int balancerInt = 0;
        int length = directionsLog.Count;
        if ((previousDirection == "left") && (currentDirection == "right"))
        {
            balancerInt = 1;
        }
        if ((previousDirection == "right") && (currentDirection == "left"))
        {
            balancerInt = 1;
        }
        if ((directionsLog[length - 3] == -1) && (directionsLog[length - 2] == 0))
        {
            balancerInt = 1;
        }
        if ((directionsLog[length - 3] == 1) && (directionsLog[length - 2] == 0))
        {
            balancerInt = 1;
        }
        return balancerInt;
    }
}
