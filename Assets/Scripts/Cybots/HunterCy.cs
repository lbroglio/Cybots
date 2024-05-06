using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HunterCy : Cybot
{

    private Player player;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        player = GameObject.Find("FirstPersonRig(Clone)").GetComponent<Player>();

        currentBehavior = new WalkToPlayer(player, map);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        // If moving to the player
        if (currentBehavior.GetType() == typeof(WalkToPlayer))
        {
            // If the player is seen start chasing
            if (doActionRet == (int)GoToPlayer.StateVals.SEE_PLAYER)
            {
                currentBehavior = new WalkingChase();
            }
        }
        // If chasing the player
        else if (currentBehavior.GetType() == typeof(WalkingChase))
        {
            // If the player is lost
            if (doActionRet == (int)Chase.StateVals.LOST_PLAYER)
            {
                // Wait for a new chase
                currentBehavior = new WalkToPlayer(player, map);
            }
            // If at the player
            else if (doActionRet == (int)Chase.StateVals.AT_PLAYER)
            {
                currentBehavior = new CapturePlayer();
            }
        }
        // If capturing the player
        else if (currentBehavior.GetType() == typeof(CapturePlayer))
        {
            // If the player is out of range 
            if (doActionRet == (int)CapturePlayer.StateVals.PLAYER_GONE)
            {
                // If this cybot can see the player chase 
                if (canSeePlayer())
                {
                    currentBehavior = new WalkingChase();
                }
                // Otherwise wait for a chase to start 
                else
                {
                    currentBehavior = new WalkToPlayer(player, map);
                }
            }
        }
    }
}




/// <summary>
/// Behavior for a cybot which is trying to go to the player
/// </summary>
public class WalkToPlayer : GoToPlayer
{


    public WalkToPlayer(Player player, MapBuilder map) : base(player, map)
    {

        MoveType = CybotMoveTypes.WALK;
    }

    public override int DoAction(Cybot cybot)
    {
        return base.DoAction(cybot);
    }
}

/// <summary>
/// Class for a cybot which is chasing the player at walking speed
/// </summary>
public class WalkingChase : Chase
{
    public WalkingChase()
    {

        MoveType = CybotMoveTypes.WALK;
    }

    public override int DoAction(Cybot cybot)
    {
        return base.DoAction(cybot);
    }
}
