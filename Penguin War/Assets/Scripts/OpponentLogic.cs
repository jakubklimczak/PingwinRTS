using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentLogic : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(ChooseAction());
    }

    IEnumerator ChooseAction()
    {
        /*
        if(NOT GAME OVER) {

            if () //enemy has enough resources (we will pick a value) && random number in good range
            {
                //build a random thing they can afford (upgrades, buildings etc) - priority - igloo
                //buildings can be created near the opponents base (2 tiles from their last building AND away from the nest) OR in case of the bridge - nearest water tile
            }

            if() { //idle worker penguin
                // random - send for a resource bot has the least, make a warrior (smaller chance)
            }

            if() { //idle warrior
                //move near other warrior
            }

            if() { //enemy near && more than 1 warrior
                //send warriors near the enemy penguins
            }

            if() { //a lot of warriors in bots possesion
                //send warriors near the players base
            }

            if() { //bot has more than X of a resource (too much)
                //change one worker from that resource to one they have the least of
            }

        }
        */



        yield return new WaitForSeconds(.1f);
    }
}
