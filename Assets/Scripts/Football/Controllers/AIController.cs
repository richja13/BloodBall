using Core.Data;
using Core.Enums;
using Football.Data;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public bool Offence = true;
    public float _fieldHalf = -1;
    Vector3 _meshBounds;
    List<GameObject> _backPlayers = new();
    List<GameObject> _centrePlayers = new();
    List<GameObject> _forwardPlayers = new();
    // Start is called before the first frame update
    void Start()
    {
        _meshBounds = MatchData.FieldObject.GetComponent<MeshFilter>().mesh.bounds.size;
        LoadPlayers();
    }

    void LoadPlayers()
    {
        foreach (var player in MovementData.RedTeam)
        {
            var fieldPosition = player.GetComponent<PlayerData>().FieldPosition;
            if (fieldPosition == PositionOnField.Forward)
                _forwardPlayers.Add(player);
            else if (fieldPosition == PositionOnField.Centre)
                _centrePlayers.Add(player);
            else
                _backPlayers.Add(player);
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckFieldHalf();

        ManageForward();
        ManageCentre();
        ManageBack();

        if (_fieldHalf == -1)
            Offence = true;
        else
            Offence = false;

        foreach (var player in MovementData.RedTeam)
        {
            if (player.name == MovementData.SelectedPlayer.name)
                continue;

            var data = player.GetComponent<PlayerData>();

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
                
                case PlayerState.Defence:
                    if (data.FieldPosition == PositionOnField.Centre || data.FieldPosition == PositionOnField.Forward)
                        if (CheckYPosition(player.transform.position, data.Target, 1))
                            data.Target = GenerateRandomVector(data.SpawnPoint.position, MovementData.Ball.transform.position, data.state);
                        else
                            MovePlayers(data.Target, player.transform);
                    else
                    {
                        //Zrobic krycie zawodnikow
                    }

                break;

                case PlayerState.Idle:
                    if (!CheckVector(player.transform.position, data.SpawnPoint.position, 5))
                        MovePlayers(data.SpawnPoint.position, player.transform);
                    else
                        Debug.Log("IDLE");
                break;

                case PlayerState.GetBall:
                    MovePlayers(MovementData.Ball.transform.position, player.transform);
                break;
            }
        }
    }

    void ManageForward()
    {
        foreach (var player in _forwardPlayers)
        {
            var data = player.GetComponent<PlayerData>();

            if (CheckVector(player.transform.position, MovementData.Ball.transform.position, 25))
                if (Offence)
                    data.state = PlayerState.Attack;
                else
                    data.state = PlayerState.Defence;
            else
                data.state = PlayerState.Idle;

            if (!MatchData.RedTeamHasBall && CheckVector(player.transform.position, MovementData.Ball.transform.position, 20))
                data.state = PlayerState.GetBall;
        }
    }

    void ManageCentre()
    {
        foreach (var player in _centrePlayers)
        {
            var data = player.GetComponent<PlayerData>();

                if (Offence)
                    data.state = PlayerState.Attack;
                else
                    data.state = PlayerState.Defence;

            if (!MatchData.RedTeamHasBall && CheckVector(player.transform.position, MovementData.Ball.transform.position, 20))
                data.state = PlayerState.GetBall;
        }
    }


    void ManageBack()
    {
        foreach (var player in _backPlayers)
        {
            var data = player.GetComponent<PlayerData>();

            if (CheckVector(player.transform.position, MovementData.Ball.transform.position, 25))
                if (Offence)
                    data.state = PlayerState.Attack;
                else
                    data.state = PlayerState.Defence;
            else
                data.state = PlayerState.Idle;

            if (!MatchData.RedTeamHasBall && CheckVector(player.transform.position, MovementData.Ball.transform.position, 20))
                data.state = PlayerState.GetBall;
        }
    }

    void CheckFieldHalf() => _fieldHalf = (MovementData.Ball.transform.position.x < 0) ? -1 : 1;

    void MovePlayers(Vector3 target, Transform player) => player.position = Vector3.MoveTowards(new Vector3(player.position.x, 0.84f, player.position.z), target, Time.deltaTime * 15f);

    bool CheckVector(Vector3 playerPos, Vector3 target, float distance)
    {
        if (Vector3.Distance(playerPos, target) < distance)
            return true;
        else 
            return false;
    }

    bool CheckYPosition(Vector3 playerPos, Vector3 target, float distance)
    {
        if (Mathf.Abs(playerPos.z) - Mathf.Abs(target.z) > distance)
            return true;
        else
            return false;
    }

    Vector3 GenerateRandomVector(Vector3 playerPosition, Vector3 ballPosition, PlayerState state)
    {
        Vector3 newVector = new();
        newVector.y = 0;
        if(MatchData.RedTeamHasBall)
        {
            float maxHeight = playerPosition.z;
            float maxWidth = 0;

            if (Mathf.Abs(ballPosition.x) < Mathf.Abs(ballPosition.x))
                maxWidth = ballPosition.x + 20;
            else
                maxWidth = ballPosition.x - 20;

            if (Vector3.Distance(playerPosition, ballPosition) > 5 && Vector3.Distance(playerPosition, ballPosition) < 15)
                newVector.z = ballPosition.z;
            else
                newVector.z = maxHeight;

            newVector.x = Random.Range(ballPosition.x + 5, maxWidth);
        }
        return newVector;
    }
}
