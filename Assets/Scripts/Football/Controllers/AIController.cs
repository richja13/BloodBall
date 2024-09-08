using Core;
using Core.Data;
using Core.Enums;
using Football.Controllers;
using Football.Data;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    static bool _offence = true;
    float _fieldHalf = -1;
    Vector3 _meshBounds;
    static List<PlayerData> _backPlayers = new();
    static List<PlayerData> _centrePlayers = new();
    static List<PlayerData> _forwardPlayers = new();

    void Start()
    {
        _meshBounds = MatchData.FieldObject.GetComponent<MeshFilter>().mesh.bounds.size;
        LoadPlayers();
    }

    void LoadPlayers()
    {
        foreach (var player in MovementData.AllPlayers)
        {
            var fieldPosition = player.FieldPosition;
            if (fieldPosition == PositionOnField.Forward)
                _forwardPlayers.Add(player);
            else if (fieldPosition == PositionOnField.Centre)
                _centrePlayers.Add(player);
            else
                _backPlayers.Add(player);
        }
    }

    void Update()
    {
        CheckFieldHalf();
        //ManageForward();
        //ManageCentre();
        Debug.Log(MovementData.PlayerHasBall + ": Player has ball");
        if (MatchData.RedTeamHasBall)
            _offence = true;
        if (MatchData.BlueTeamHasBall)
            _offence = false;

        foreach (var data in MovementData.AllPlayers)
        {
            if (data.name == MovementData.RedSelectedPlayer.name || data.name == MovementData.BlueSelectedPlayer.name)
                continue;

            MovementController.AttackEnemy(data);

            switch (data.state)
            {
                case PlayerState.Attack:
               
                    if (MatchData.BlueTeamHasBall || MatchData.RedTeamHasBall)
                    {
                        if (data.FieldPosition != PositionOnField.Back)
                        {
                            if (data.Target == Vector3.zero)
                                data.Target = GenerateRandomVector(data.SpawnPoint.position, MovementData.Ball.transform.position);

                            if (CoreViewModel.CheckVector(data.PlayerPosition, data.Target, 2))
                                data.Target = GenerateRandomVector(data.SpawnPoint.position, MovementData.Ball.transform.position);
                            else
                                MovePlayers(data.Target, data.PlayerPosition, data);
                        }
                        else
                        {
                            float ballDistance = Vector3.Distance(data.PlayerPosition, MovementData.Ball.transform.position);
                            float extraDistance = (data.playerTeam == Team.Blue) ? -ballDistance : ballDistance;
                            data.Target = new Vector3(data.SpawnPoint.position.x + (extraDistance * _fieldHalf), data.SpawnPoint.position.y, data.SpawnPoint.position.z);

                            if(!CoreViewModel.CheckVector(data.PlayerPosition, data.Target, 1))
                                MovePlayers(data.Target, data.PlayerPosition, data);
                        }
                    }
                    else
                    {
                        data.Target = MovementData.Ball.transform.position;
                        data.state = PlayerState.GetBall;
                    }
                
                break;

                case PlayerState.Defence:
                    OnDefence(data);
                  
                    break;
                case PlayerState.Marking:
                    if (CoreViewModel.CheckVector(data.PlayerPosition, data.Target, 0.2f))
                        MovePlayers(data.Target, data.PlayerPosition, data);
                    break;

                case PlayerState.GetBall:
                    if ((data.playerTeam == Team.Red && !MatchData.RedTeamHasBall) || (data.playerTeam == Team.Blue && !MatchData.BlueTeamHasBall))
                    {
                        Rigidbody ballRb = MovementData.Ball.GetComponent<Rigidbody>();
                        MovementController.InterceptionDirection(MovementData.Ball.transform.position, 
                            data.PlayerPosition, ballRb.velocity, 15, out var position, out var direction);
                        
                        data.Target = position;
                        MovePlayers(position, data.PlayerPosition, data);
                    }
                    break;

                case PlayerState.Tackle:

                    break;
            }
        }
    }

    void OnDefence(PlayerData data)
    {
        data.MarkedPlayer = null;
        List<PlayerData> EnemyPlayers = new();
        PlayerData SelectedEnemy;
        PlayerData closestPlayer;

        if (data.playerTeam == Team.Red)
        {
            foreach (var players in MovementData.BlueTeam)
                EnemyPlayers.Add(players.GetComponent<PlayerData>());

            SelectedEnemy = MovementData.BlueSelectedPlayer.GetComponent<PlayerData>();
        }
        else
        {
            foreach (var players in MovementData.RedTeam)
                EnemyPlayers.Add(players.GetComponent<PlayerData>());
            SelectedEnemy = MovementData.RedSelectedPlayer.GetComponent<PlayerData>();
        }

        if (MatchData.BlueTeamHasBall || MatchData.RedTeamHasBall)
        {
            if (Vector3.Distance(data.PlayerPosition, MovementData.Ball.transform.position) < 8)
            {
                closestPlayer = MovementController.FindClosestPlayer(EnemyPlayers, data.Torso.transform, out var distance);

                closestPlayer.MarkedPlayer ??= data.transform;

                    if (closestPlayer == SelectedEnemy)
                    {
                        MovementController.InterceptionDirection(closestPlayer.PlayerPosition,
                          data.PlayerPosition, closestPlayer.Torso.GetComponent<Rigidbody>().velocity, 3, out var position, out var direction);

                        data.Target = new Vector3(position.x, 0, position.z);
                        MovePlayers(data.Target, data.PlayerPosition, data);
                        //data.state = PlayerState.GetBall;
                    }
                    else
                    {
                        if (Vector3.Distance(data.PlayerPosition, SelectedEnemy.PlayerPosition) > 10)
                        {
                            if (CoreViewModel.CheckVector(data.PlayerPosition, data.SpawnPoint.position, 7))
                                if (closestPlayer.MarkedPlayer == data.transform)
                                {
                                    data.Target = closestPlayer.PlayerPosition;
                                    data.state = PlayerState.Marking;
                                }
                        }
                        else
                        {
                            data.Target = MovementData.Ball.transform.position;
                            data.state = PlayerState.GetBall;
                        }
                    }
            }
            else
            {
                float ballDistance = Vector3.Distance(data.PlayerPosition, MovementData.Ball.transform.position);
                float extraDistance = (data.playerTeam == Team.Blue) ? -ballDistance / 5 : ballDistance / 5;
                data.Target = new Vector3(data.SpawnPoint.position.x + (extraDistance * _fieldHalf), data.SpawnPoint.position.y, data.SpawnPoint.position.z);

                MovePlayers(data.Target, data.PlayerPosition, data);
            }
        }
        else
        {
            data.Target = MovementData.Ball.transform.position;
            data.state = PlayerState.GetBall;
        }
    }
   

    internal static void ManageForward()
    {
        foreach (var data in _forwardPlayers)
        {
            if (data.playerTeam == Team.Red)
                data.state = (_offence) ? PlayerState.Defence : PlayerState.Attack;
            else
                data.state = (_offence) ? PlayerState.Attack : PlayerState.Defence;
        }
    }

    internal static void ManageCentre()
    {
        foreach (var data in _centrePlayers)
        {
            if (data.playerTeam == Team.Red)
                data.state = (_offence) ? PlayerState.Attack : PlayerState.Defence;
            else
                data.state = (_offence) ? PlayerState.Defence : PlayerState.Attack;
        }
    }

    internal static void ManageBack()
    {
        foreach (var data in _backPlayers)
        {
            data.state = PlayerState.Defence;
        }
    }

    void CheckFieldHalf() => _fieldHalf = (MovementData.Ball.transform.position.x < 0) ? -1 : 1;

    void MovePlayers(Vector3 target, Vector3 playerPos, PlayerData data) => data.Movement = (MatchData.MatchStarted) ? new Vector3(target.x - playerPos.x, 0, target.z - playerPos.z).normalized : Vector3.zero;

    bool CheckYPosition(Vector3 playerPos, Vector3 target, float distance)
    {
        if (Mathf.Abs(playerPos.z) - Mathf.Abs(target.z) > distance)
            return true;
        else
            return false;
    }

    Vector3 GenerateRandomVector(Vector3 playerPosition, Vector3 ballPosition)
    {
        Vector3 newVector = new();
        newVector.y = 0;
        if (MatchData.RedTeamHasBall)
        {
            float maxHeight = playerPosition.z;
            float maxWidth = -12;

            if (Vector3.Distance(playerPosition, ballPosition) > 1 && Vector3.Distance(playerPosition, ballPosition) < 4)
                newVector.z = ballPosition.z;
            else
                newVector.z = maxHeight;

            newVector.x = Random.Range(ballPosition.x, maxWidth);
        }

        if (MatchData.BlueTeamHasBall)
        {
            float maxHeight = playerPosition.z;
            float maxWidth = 12;

            if (Vector3.Distance(playerPosition, ballPosition) > 2 && Vector3.Distance(playerPosition, ballPosition) < 10)
                newVector.z = ballPosition.z;
            else
                newVector.z = maxHeight;

            newVector.x = Random.Range(ballPosition.x, maxWidth);
        }
        return newVector;
    }


    internal static void RestartMatch()
    {
        foreach (var data in MovementData.AllPlayers)
            data.Torso.transform.position = new(data.SpawnPoint.position.x, data.Torso.transform.position.y, data.SpawnPoint.position.z);
   
        MatchData.MatchStarted = false;
        MovementData.Ball.transform.parent = null;
        MovementData.Ball.transform.position = Vector3.zero;
        MovementData.Ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        CoreViewModel.StartMatch();
    }
}   

