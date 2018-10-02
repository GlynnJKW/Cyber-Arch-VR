using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerExtend : Player {

    public static bool SetInstance(){
        if(Player.instance){
            if(Player.instance.gameObject){
                GameObject go = Player.instance.gameObject;
                Destroy(Player.instance);
                Player.instance = go.AddComponent<PlayerExtend>();
                return true;
            }
        }
        return false;
    }

}