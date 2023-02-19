using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlatformGeneration : MonoBehaviour
{
    [SerializeField] private List<float> forward; // смещение дл€ платформы спереди
    [SerializeField] private List<float> left; // смещение дл€ платформы слева
    [SerializeField] private List<float> right; // смещение дл€ платформы справа

    [SerializeField] private float spawnTimer; // через какое врем€ спаунить новую платформу

    private bool canGenerate = true;
    private List<int> directionsLog = new();
    private string currentDirection;
    private string previousDirection;
    private int currentLayerOrder = 0;

    private void Start()
    {
        // нулевые координаты платформы при старте игры
        LastPosition.lastBox = new() { 0, 0, 0 };

        // заполнение базовых направлений движени€ платформ
        directionsLog = new() { 0,0,0,0,0,0,0,0,0,0,0,0 };

        // спаун первой платформы
        PlatformSpawner();
    }

    void Update()
    {
        // canGenerate переходит в true каждые spawnTimer секунд, позвол€€ спаунить новую платформу
        if (canGenerate) StartCoroutine(GeneratorDelay());
    }

    private IEnumerator GeneratorDelay()
    {
        canGenerate = false;
        yield return new WaitForSeconds(spawnTimer);
        PlatformSpawner();
        canGenerate = true;
    }

    // спаунит новую платформу
    private void PlatformSpawner()
    {
        // спаунит из пула платформу или делает новую если платформ не хватает
        var platform = PlatformPool.Instance.Get(); 
        
        // задаютс€ координат следующей платформы
        List<float> direction = SetPosition();
        Vector3 position = new(LastPosition.lastBox[0] + direction[0], LastPosition.lastBox[1] + direction[1], LastPosition.lastBox[2]);
        platform.transform.SetPositionAndRotation(position, new Quaternion());

        // задаЄтс€ позици€ в слое платформы platform
        platform.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = SetLayerOrder();
        platform.gameObject.SetActive(true);

        // очищаютс€ координаты последней платформы и задаютс€ новые, равные текущей платформе
        LastPosition.lastBox.Clear();
        LastPosition.lastBox = new() { platform.gameObject.transform.position.x,
            platform.gameObject.transform.position.y,
            platform.gameObject.transform.position.z };
    }

    // задаЄтс€ смещение координат (слева, справа, спереди) в которых должна по€витс€ следующа€ платформа
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

    // задаЄтс€ позици€ на слое относительно прошлой платформы
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

    // алгоритм выбора направлени€, в котром должна по€витс€ следующа€ платформа
    private List<float> ChooseNextPositionAlgorythm()
    {
        previousDirection = currentDirection;
        int directionSum = directionsLog.Sum();
        int selector = Random.Range(0, 2);
        List<float> selection = new();

        // -1 лево, 0 пр€мо, 1 право
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

    // балансировка взаимоисключающих лево-право и вперЄд после поворота
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
