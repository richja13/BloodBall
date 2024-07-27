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
    static List<GameObject> _backPlayers = new();
    static List<GameObject> _centrePlayers = new();
    static List<GameObject> _forwardPlayers = new();
    // Start is called before the first frame update
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
        Debug.Log("Blue team has ball " + MatchData.BlueTeamHasBall);
        Debug.Log("Red team has ball " + MatchData.RedTeamHasBall);

        CheckFieldHalf();
        ManageForward();
        ManageCentre();

        if (MatchData.RedTeamHasBall)
            _offence = true;
        if (MatchData.BlueTeamHasBall)
            _offence = false;

        foreach (var player in MovementData.AllPlayers)
        {
            if (player.name == MovementData.RedSelectedPlayer.name || player.name == MovementData.BlueSelectedPlayer.name)
                continue;

            var data = player.GetComponent<PlayerData>();

            switch (data.state)
            {
                case PlayerState.Attack:
               
                    if (MatchData.BlueTeamHasBall || MatchData.RedTeamHasBall)
                    {
                        if (data.FieldPosition != PositionOnField.Back)
                        {
                            if (data.Target == Vector3.zero)
                                data.Target = GenerateRandomVector(data.SpawnPoint.position, MovementData.Ball.transform.position, data.state);
                            if (CoreViewModel.CheckVector(player.transform.position, data.Target, 2))
                                data.Target = GenerateRandomVector(data.SpawnPoint.position, MovementData.Ball.transform.position, data.state);
                            else
                                MovePlayers(data.Target, player.transform, data.Speed);
                        }
                        else
                        {
                            float ballDistance = Vector3.Distance(player.transform.position, MovementData.Ball.transform.position);
                            float extraDistance = (data.playerTeam == Team.Blue) ? ballDistance / 3 : -ballDistance / 3;
                            data.Target = new Vector3(data.SpawnPoint.position.x + extraDistance, data.SpawnPoint.position.y, data.SpawnPoint.position.z);

                            MovePlayers(data.Target, player.transform, data.Speed);
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
                    List<GameObject> EnemyPlayers;
                    GameObject SelectedEnemy;
                    Transform closestPlayer;

                    if (data.playerTeam == Team.Red)
                    {
                        EnemyPlayers = MovementData.BlueTeam;
                        SelectedEnemy = MovementData.BlueSelectedPlayer;
                    }
                    else
                    {
                        EnemyPlayers = MovementData.RedTeam;
                        SelectedEnemy = MovementData.RedSelectedPlayer;
                    }

                    if (MatchData.BlueTeamHasBall || MatchData.RedTeamHasBall)
                    {
                        if (Vector3.Distance(player.transform.position, MovementData.Ball.transform.position) < 35)
                        {
                            closestPlayer = MovementController.FindClosestPlayer(EnemyPlayers, player.transform, out var distance);

                            closestPlayer.GetComponent<PlayerData>().MarkedPlayer ??= player.transform;

                            if (closestPlayer.GetComponent<PlayerData>().MarkedPlayer == player.transform)
                            {
                                if (closestPlayer.gameObject == SelectedEnemy.gameObject)
                                {
                                    data.Target = MovementData.Ball.transform.position;
                                    goto case PlayerState.GetBall;
                                }
                                else
                                {
                                    if (Vector3.Distance(player.transform.position, SelectedEnemy.transform.position) > 30)
                                    {
                                        if (CoreViewModel.CheckVector(player.transform.position, data.SpawnPoint.position, 80))
                                            if (closestPlayer.GetComponent<PlayerData>().MarkedPlayer == player.transform)
                                            {
                                                data.Target = closestPlayer.position;
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
                                if (CoreViewModel.CheckVector(player.transform.position, data.Target, 2))
                                    data.Target = GenerateRandomVector(data.SpawnPoint.position, MovementData.Ball.transform.position, data.state);
                                else
                                    MovePlayers(data.Target, player.transform, data.Speed);
                            }
                        }
                        else
                        {
                            float ballDistance = Vector3.Distance(player.transform.position, MovementData.Ball.transform.position);
                            float extraDistance = (data.playerTeam == Team.Blue) ? ballDistance / 3 : -ballDistance/3;
                            data.Target = new Vector3(data.SpawnPoint.position.x + extraDistance , data.SpawnPoint.position.y, data.SpawnPoint.position.z);

                            MovePlayers(data.Target, player.transform, data.Speed);
                        }
                    }
                    else
                    {
                        data.Target = MovementData.Ball.transform.position;
                        data.state = PlayerState.GetBall;
                    }
                    break;

                case PlayerState.Marking:
                    if (!CoreViewModel.CheckVector(player.transform.position, data.Target, 2))
                        MovePlayers(data.Target, player.transform, data.Speed);
                    break;

                case PlayerState.GetBall:
                    if (CoreViewModel.CheckVector(player.transform.position, data.Target, 40))
                        MovePlayers(MovementData.Ball.transform.position, player.transform, data.Speed);
                    break;

                case PlayerState.Tackle:

                    break;
            }
        }
    }

    void ManageForward()
    {
        foreach (var player in _forwardPlayers)
        {
            var data = player.GetComponent<PlayerData>();
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
                 if (!CoreViewModel.CheckVector(player.transform.position, MovementData.Ball.transform.position, 5))
                     data.state = (_offence) ? PlayerState.Attack : PlayerState.Defence;
                 else
                     data.state = PlayerState.Idle;

                 if (!MatchData.RedTeamHasBall && CoreViewModel.CheckVector(player.transform.position, MovementData.Ball.transform.position, 20))
                     data.state = PlayerState.GetBall;
             }
             else
             {
                 if (!CoreViewModel.CheckVector(player.transform.position, MovementData.Ball.transform.position, 5))
                     data.state = (_offence) ? PlayerState.Defence : PlayerState.Attack;
                 else
                     data.state = PlayerState.Idle;

                 if (!MatchData.BlueTeamHasBall && CoreViewModel.CheckVector(player.transform.position, MovementData.Ball.transform.position, 20))
                     data.state = PlayerState.GetBall;
             }
         }*/
    }

    void ManageCentre()
    {
        foreach (var player in _centrePlayers)
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

        /* foreach (var player in _centrePlayers)
         {
             var data = player.GetComponent<PlayerData>();

             if (data.playerTeam == Team.Red)
             {
                 data.state = (_offence) ? PlayerState.Attack : PlayerState.Defence;

                 if (!MatchData.RedTeamHasBall && CoreViewModel.CheckVector(player.transform.position, MovementData.Ball.transform.position, 20) && CoreViewModel.CheckVector(player.transform.position, data.SpawnPoint.position, 80))
                     data.state = PlayerState.GetBall;
             }
             else
             {
                 data.state = (_offence) ? PlayerState.Defence : PlayerState.Attack;

                 if (!MatchData.BlueTeamHasBall && CoreViewModel.CheckVector(player.transform.position, MovementData.Ball.transform.position, 30) && CoreViewModel.CheckVector(player.transform.position, data.SpawnPoint.position, 80))
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
    }

    /*  foreach (var player in _backPlayers)
      {
          var data = player.GetComponent<PlayerData>();

          if (CoreViewModel.CheckVector(player.transform.position, data.SpawnPoint.position, 80))
          {
              if (data.playerTeam == Team.Red)
              {
                  data.state = (_offence) ? PlayerState.Attack : PlayerState.Defence;

               *//*   if (!MatchData.RedTeamHasBall && CoreViewModel.CheckVector(player.transform.position, MovementData.Ball.transform.position, 25))
                      data.state = PlayerState.GetBall;*//*
              }
              else
              {
                  data.state = (_offence) ? PlayerState.Defence : PlayerState.Attack;
*//*
                    if (!MatchData.BlueTeamHasBall && CoreViewModel.CheckVector(player.transform.position, MovementData.Ball.transform.position, 25))
                        data.state = PlayerState.GetBall;*//*
                }
            }
            else
                data.state = PlayerState.Idle;    
        }
  }*/

    void CheckFieldHalf() => _fieldHalf = (MovementData.Ball.transform.position.x < 0) ? -1 : 1;

    void MovePlayers(Vector3 target, Transform player, float speed) => player.position = (MatchData.MatchStarted) ? Vector3.MoveTowards(new Vector3(player.position.x, 0.84f, player.position.z), target, Time.deltaTime * speed * 3) : player.position;

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
                maxWidth = ballPosition.x + 20;
            else
                maxWidth = ballPosition.x - 20;

            if (Vector3.Distance(playerPosition, ballPosition) > 5 && Vector3.Distance(playerPosition, ballPosition) < 15)
                newVector.z = ballPosition.z;
            else
                newVector.z = maxHeight;

            newVector.x = Random.Range(ballPosition.x + 10, maxWidth);
        }

        if (MatchData.BlueTeamHasBall)
        {
            float maxHeight = playerPosition.z;
            float maxWidth = 0;

            if (Mathf.Abs(ballPosition.x) > Mathf.Abs(ballPosition.x))
                maxWidth = ballPosition.x - 20;
            else
                maxWidth = ballPosition.x + 20;

            if (Vector3.Distance(playerPosition, ballPosition) > 5 && Vector3.Distance(playerPosition, ballPosition) < 15)
                newVector.z = ballPosition.z;
            else
                newVector.z = maxHeight;

            newVector.x = Random.Range(ballPosition.x - 10, maxWidth);
        }
        return newVector;
    }


    internal static void RestartMatch()
    {
        foreach (var player in MovementData.AllPlayers)
        {
            var data = player.gameObject.GetComponent<PlayerData>();
            player.transform.position = new Vector3(data.SpawnPoint.position.x, 0.84f, data.SpawnPoint.position.z);
        }
   
        MatchData.MatchStarted = false;
        MovementData.Ball.transform.position = Vector3.zero;
        MovementData.Ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        CoreViewModel.StartMatch();
    }
}   

