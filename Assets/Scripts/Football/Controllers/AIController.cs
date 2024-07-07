using Core.Data;
using Core.Enums;
using Football.Data;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public bool Offence = true;
    public float _fieldHalf = -1;
    Vector3 _meshBounds;
    // Start is called before the first frame update
    void Start()
    {
        _meshBounds = MatchData.FieldObject.GetComponent<MeshFilter>().mesh.bounds.size;
        Debug.Log("Field center " + _meshBounds.x/2);
    }

    // Update is called once per frame
    void Update()
    {
        CheckFieldHalf();

        foreach (var player in MovementData.RedTeam)
        {
            if (player.name == MovementData.SelectedPlayer.name)
                continue;
                
            var data = player.GetComponent<PlayerData>();

            if (CheckVector(player.transform.position, MovementData.Ball.transform.position, 10) || !CheckVector(player.transform.position, MovementData.Ball.transform.position, 25))
                if (Offence)
                    data.state = PlayerState.Attack;
                else
                    data.state = PlayerState.Defence;
            else
                data.state = PlayerState.Idle;

            switch (data.state)
            {
                case PlayerState.Attack:
                    if (data.Target == Vector3.zero)
                        data.Target = GenerateRandomVector(data.SpawnPoint.position, MovementData.Ball.transform.position, data.state);

                    if (CheckVector(player.transform.position, data.Target, 1))
                        data.Target = GenerateRandomVector(data.SpawnPoint.position, MovementData.Ball.transform.position, data.state);
                    else
                        MovePlayers(data.Target, player.transform);
                break;
                    
                case PlayerState.Idle:
                    Debug.Log("IDLE");
                break;
            }
        }

    }

    void CheckFieldHalf() => _fieldHalf = (_meshBounds.x / 2 > 5) ? -1 : 1;

    void MovePlayers(Vector3 target, Transform player) => player.Translate(target * Time.deltaTime * 0.65f);
/*
    var meshBound FieldObj.GetComponent<MeshFilter>.mesh.Bounds

    float[,] = meshBounds.x, meshBounds.y

    if(team != has ball)
    {
        var PlayerDistance = Distance(ThisPlayerPosition, BallPosition)

        if(PlayerDistance < 50)
            ThisPlayer.State = GetBall();

    }

    if(team has ball)
    {
        var SelectedPlayerDistance = Distance(ThisPlayerPosition, SelectedPlayerPosition)
        MoveWithPlayer(SelectedPlayerDistance);

    }
      

    void MoveWithPlayer(Vector2 distance)
    {
        MaxWith = 500;
        MaxHeight = 10;
        Var CreateVectorX = Random.Range()

    }
*/

    bool CheckVector(Vector3 playerPos, Vector3 target, float distance)
    {
        if (Vector3.Distance(playerPos, target) < distance)
            return true;
        else 
            return false;
    }

    Vector3 GenerateRandomVector(Vector3 playerPosition, Vector3 ballPosition, PlayerState state)
    {
       Vector3 newVector = new();
       float maxHeight = playerPosition.z;
       float maxWidth = 0;

        if (state == PlayerState.Attack)
            maxWidth = ballPosition.x + 20 * _fieldHalf;
        else 
            maxWidth = ballPosition.x - 20 * _fieldHalf;

        newVector.z = Random.Range(playerPosition.z, maxHeight);
        newVector.x = Random.Range(ballPosition.x + 5, maxWidth);

        return newVector;
    }
}
