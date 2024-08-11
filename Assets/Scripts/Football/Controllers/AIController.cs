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
        //ManageBack();
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
        ManageForward();
        ManageCentre();

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
                                data.Target = GenerateRandomVector(data.SpawnPoint.position, MovementData.Ball.transform.position, data.state);
                            if (CoreViewModel.CheckVector(data.PlayerPosition, data.Target, 2))
                                data.Target = GenerateRandomVector(data.SpawnPoint.position, MovementData.Ball.transform.position, data.state);
                            else
                                MovePlayers(data.Target, data.PlayerPosition, data);
                        }
                        else
                        {
                            float ballDistance = Vector3.Distance(data.PlayerPosition, MovementData.Ball.transform.position);
                            float extraDistance = (data.playerTeam == Team.Blue) ? ballDistance / 3 : -ballDistance / 3;
                            data.Target = new Vector3(data.SpawnPoint.position.x + extraDistance, data.SpawnPoint.position.y, data.SpawnPoint.position.z);

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

                    data.MarkedPlayer = null;
                    List<PlayerData> EnemyPlayers = new();
                    GameObject SelectedEnemy;
                    PlayerData closestPlayer;

                    if (data.playerTeam == Team.Red)
                    {
                        foreach(var players in MovementData.BlueTeam)
                            EnemyPlayers.Add(players.GetComponent<PlayerData>());

                        SelectedEnemy = MovementData.BlueSelectedPlayer;
                    }
                    else
                    {
                        foreach (var players in MovementData.RedTeam)
                            EnemyPlayers.Add(players.GetComponent<PlayerData>());
                        SelectedEnemy = MovementData.RedSelectedPlayer;
                    }

                    if (MatchData.BlueTeamHasBall || MatchData.RedTeamHasBall)
                    {
                        if (Vector3.Distance(data.PlayerPosition, MovementData.Ball.transform.position) < 5)
                        {
                            closestPlayer = MovementController.FindClosestPlayer(EnemyPlayers, data.transform, out var distance);

                            closestPlayer.MarkedPlayer ??= data.transform;

                            if (closestPlayer.MarkedPlayer == data.transform)
                            {
                                if (closestPlayer.gameObject == SelectedEnemy.gameObject)
                                {
                                    data.Target = MovementData.Ball.transform.position;
                                    goto case PlayerState.GetBall;
                                }
                                else
                                {
                                    if (Vector3.Distance(data.PlayerPosition, SelectedEnemy.transform.position) > 10)
                                    {
                                        if (CoreViewModel.CheckVector(data.PlayerPosition, data.SpawnPoint.position, 30))
                                            if (closestPlayer.MarkedPlayer == data.transform)
                                            {
                                                data.Target = closestPlayer.PlayerPosition;
                                                goto case PlayerState.Marking;
                                            }
                                    }
                                    else
                                    {
                                        data.Target = MovementData.Ball.transform.position;
                                        goto case PlayerState.GetBall;
                                    }
                                }
                            }
                            else
                            {
                                if (data.Target == Vector3.zero)
                                    data.Target = GenerateRandomVector(data.SpawnPoint.position, MovementData.Ball.transform.position, data.state);
                                if (CoreViewModel.CheckVector(data.PlayerPosition, data.Target, 0.2f))
                                    data.Target = GenerateRandomVector(data.SpawnPoint.position, MovementData.Ball.transform.position, data.state);
                                else
                                    MovePlayers(data.Target, data.PlayerPosition, data);
                            }
                        }
                        else
                        {
                            float ballDistance = Vector3.Distance(data.PlayerPosition, MovementData.Ball.transform.position);
                            float extraDistance = (data.playerTeam == Team.Blue) ? ballDistance / 3 : -ballDistance/3;
                            data.Target = new Vector3(data.SpawnPoint.position.x + extraDistance , data.SpawnPoint.position.y, data.SpawnPoint.position.z);

                            MovePlayers(data.Target, data.PlayerPosition, data);
                        }
                    }
                    else
                    {
                        data.Target = MovementData.Ball.transform.position;
                        data.state = PlayerState.GetBall;
                    }
                    break;

                case PlayerState.Marking:
                    if (!CoreViewModel.CheckVector(data.PlayerPosition, data.Target, 2))
                        MovePlayers(data.Target, data.PlayerPosition, data);
                    break;

                case PlayerState.GetBall:
                    if (CoreViewModel.CheckVector(data.PlayerPosition, data.Target, 20))
                        MovePlayers(MovementData.Ball.transform.position, data.PlayerPosition, data);
                    break;

                case PlayerState.Tackle:

                    break;
            }
        }
    }

    void ManageForward()
    {
        foreach (var data in _forwardPlayers)
        {
            if (data.playerTeam == Team.Red)
                data.state = (_offence) ? PlayerState.Attack : PlayerState.Defence;
            else
                data.state = (_offence) ? PlayerState.Defence : PlayerState.Attack;
        }

        /* foreach (var player in _forwardPlayers)
         {
             var data = player.GetComponent<PlayerData>();

             if (data.playerTeam == Team.Red)
             {
                 if (!CoreViewModel.CheckVector(data.PlayerPosition, MovementData.Ball.transform.position, 5))
                     data.state = (_offence) ? PlayerState.Attack : PlayerState.Defence;
                 else
                     data.state = PlayerState.Idle;

                 if (!MatchData.RedTeamHasBall && CoreViewModel.CheckVector(data.PlayerPosition, MovementData.Ball.transform.position, 20))
                     data.state = PlayerState.GetBall;
             }
             else
             {
                 if (!CoreViewModel.CheckVector(data.PlayerPosition, MovementData.Ball.transform.position, 5))
                     data.state = (_offence) ? PlayerState.Defence : PlayerState.Attack;
                 else
                     data.state = PlayerState.Idle;

                 if (!MatchData.BlueTeamHasBall && CoreViewModel.CheckVector(data.PlayerPosition, MovementData.Ball.transform.position, 20))
                     data.state = PlayerState.GetBall;
             }
         }*/
    }

    void ManageCentre()
    {
        foreach (var data in _centrePlayers)
        {
            if (data.playerTeam == Team.Red)
            {
                data.state = (_offence) ? PlayerState.Attack : PlayerState.Defence;
            }
            else
            {
                data.state = (_offence) ? PlayerState.Defence : PlayerState.Attack;
            }
        }

        /* foreach (var player in _centrePlayers)
         {
             var data = player.GetComponent<PlayerData>();

             if (data.playerTeam == Team.Red)
             {
                 data.state = (_offence) ? PlayerState.Attack : PlayerState.Defence;

                 if (!MatchData.RedTeamHasBall && CoreViewModel.CheckVector(data.PlayerPosition, MovementData.Ball.transform.position, 20) && CoreViewModel.CheckVector(data.PlayerPosition, data.SpawnPoint.position, 80))
                     data.state = PlayerState.GetBall;
             }
             else
             {
                 data.state = (_offence) ? PlayerState.Defence : PlayerState.Attack;

                 if (!MatchData.BlueTeamHasBall && CoreViewModel.CheckVector(data.PlayerPosition, MovementData.Ball.transform.position, 30) && CoreViewModel.CheckVector(data.PlayerPosition, data.SpawnPoint.position, 80))
                     data.state = PlayerState.GetBall;
             }
         }*/
    }

    /* async void ManageBack()
     {
         while (true)
         {
             await Task.Delay(3000);

             foreach (var player in _backPlayers)
             {
                 var data = player.GetComponent<PlayerData>();
                 if (data.playerTeam == Team.Red)
                 {
                     data.state = (_offence) ? PlayerState.Attack : PlayerState.Defence;
                 }
                 else
                 {
                     data.state = (_offence) ? PlayerState.Defence : PlayerState.Attack;
                 }
             }
         }*/


    public static void ManageBack()
    {
        foreach (var data in _backPlayers)
        {
            if (data.playerTeam == Team.Red)
            {
                data.state = (_offence) ? PlayerState.Attack : PlayerState.Defence;
            }
            else
            {
                data.state = (_offence) ? PlayerState.Defence : PlayerState.Attack;
            }
        }
    }

    /*  foreach (var player in _backPlayers)
      {
          var data = player.GetComponent<PlayerData>();

          if (CoreViewModel.CheckVector(data.PlayerPosition, data.SpawnPoint.position, 80))
          {
              if (data.playerTeam == Team.Red)
              {
                  data.state = (_offence) ? PlayerState.Attack : PlayerState.Defence;

               *//*   if (!MatchData.RedTeamHasBall && CoreViewModel.CheckVector(data.PlayerPosition, MovementData.Ball.transform.position, 25))
                      data.state = PlayerState.GetBall;*//*
              }
              else
              {
                  data.state = (_offence) ? PlayerState.Defence : PlayerState.Attack;
*//*
                    if (!MatchData.BlueTeamHasBall && CoreViewModel.CheckVector(data.PlayerPosition, MovementData.Ball.transform.position, 25))
                        data.state = PlayerState.GetBall;*//*
                }
            }
            else
                data.state = PlayerState.Idle;    
        }
  }*/

    void CheckFieldHalf() => _fieldHalf = (MovementData.Ball.transform.position.x < 0) ? -1 : 1;

    void MovePlayers(Vector3 target, Vector3 playerPos, PlayerData data) => data.Movement = (target - playerPos).normalized;

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
        if (MatchData.RedTeamHasBall)
        {
            float maxHeight = playerPosition.z;
            float maxWidth = 0;

            if (Mathf.Abs(ballPosition.x) < Mathf.Abs(ballPosition.x))
                maxWidth = ballPosition.x + 5;
            else
                maxWidth = ballPosition.x - 5;

            if (Vector3.Distance(playerPosition, ballPosition) > 2 && Vector3.Distance(playerPosition, ballPosition) < 10)
                newVector.z = ballPosition.z;
            else
                newVector.z = maxHeight;

            newVector.x = Random.Range(ballPosition.x + 2, maxWidth);
        }

        if (MatchData.BlueTeamHasBall)
        {
            float maxHeight = playerPosition.z;
            float maxWidth = 0;

            if (Mathf.Abs(ballPosition.x) > Mathf.Abs(ballPosition.x))
                maxWidth = ballPosition.x - 5;
            else
                maxWidth = ballPosition.x + 5;

            if (Vector3.Distance(playerPosition, ballPosition) > 2 && Vector3.Distance(playerPosition, ballPosition) < 10)
                newVector.z = ballPosition.z;
            else
                newVector.z = maxHeight;

            newVector.x = Random.Range(ballPosition.x - 2, maxWidth);
        }
        return newVector;
    }


    internal static void RestartMatch()
    {
        foreach (var data in MovementData.AllPlayers)
            data.PlayerPosition = data.SpawnPoint.position;
   
        MatchData.MatchStarted = false;
        MovementData.Ball.transform.position = Vector3.zero;
        MovementData.Ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        CoreViewModel.StartMatch();
    }
}   

