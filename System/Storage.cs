using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

public static class Storage
{
    //Keep track of whether in battle and the state of said battle.
    public static bool dance=false;
    public static bool battle=false;

    //Save game object variables.
    public static int[] donExp={0, 10};
    public static float[] savedPos={56.0f, 0, 55.0f};

    //Previous position before battle. Is not saved when game is saved.
    public static float[] prevBattlePos=new float[]{};

    //Current champs and foes in battle.
    public static string[] currentChamps=new string[]{"Don"};
    public static string[] currentFoes=new string[]{"Slime", "Slime"};

    //List of ALL champions and foes. Your current team is stored by name only in the currentChamps array.
    public static Champion getChamps(string name){
        if(name=="Don") return new Don(615, 670, 34, 10, 37, 0.5f, 8, 100);
        if(name=="Slime") return new Slime(300, 30, 10, 30, 10, 0.18f, 7, 100);
        if(name=="Shell Slime") return new Spike(300, 25, 50, 25, 45, 0.10f, 7, 100);
        else return new Dummy(300, 30, 10, 30, 10, 0.10f, 7, 100);
    }

    public static void SaveGame(){
        Save save=new Save();
        save.donExp=donExp;
        save.savedPos=savedPos;
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath+"/save.nnmg");
        bf.Serialize(file, save);
        file.Close();
    }
    public static void LoadGame(){
        if (File.Exists(Application.persistentDataPath+"/save.nnmg")){
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath+"/save.nnmg", FileMode.Open);
            Save save = (Save)bf.Deserialize(file);
            donExp=save.donExp;
            savedPos=save.savedPos;
            file.Close();
        }
    }
}