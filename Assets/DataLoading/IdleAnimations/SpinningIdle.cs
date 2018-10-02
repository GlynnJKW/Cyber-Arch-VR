using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SpinningIdle : Idle{
    private float rotationSpeed;

    public SpinningIdle(float rotationSpeed){
        this.rotationSpeed = rotationSpeed;
    }
    
    public override void Tick(){
        // Actually rotate the player.
        Player.instance.transform.Rotate(Vector3.up, rotationSpeed);
    }
}