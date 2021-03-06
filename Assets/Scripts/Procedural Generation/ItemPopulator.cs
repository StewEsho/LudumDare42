﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
 * Manages chests and enemy placement in rooms when procedurally generating dungeons.
 */
public class ItemPopulator : MonoBehaviour
{
    [SerializeField] GameObject chest;
    [SerializeField] GameObject door;
    [SerializeField] GameObject goal;
    [SerializeField] GameObject key;
    [SerializeField] List<GameObject> chestWeapons; //One of each of these is put into a chest
    [SerializeField] List<GameObject> chestItems; //The rest of the chests are filled out with items from this list
    [SerializeField] List<GameObject> enemies;
    public int ChestCount = 15;
    [HideInInspector] public int DoorCount;
    private Rect firstRoom; //bad cod ik but w/e

    private int totalChests = 0;

    //todo: add doors + keys
    private List<GameObject> chestObjectsToInstantiate = new List<GameObject>(); //todo: not needed? maybe for keys? idk yet
    
    public void PopulateRoomsWithChests(List<Rect> rooms)
    {
        Debug.Log("Populating rooms with chests!");
        //copy lists since they will be mutated.
        List<Rect> localRooms = new List<Rect>(rooms);
        List<GameObject> localChestWeapons = new List<GameObject>(chestWeapons);
        firstRoom = localRooms[0];
        localRooms.RemoveAt(0); //Remove starting room.
        localRooms.Shuffle();
        //Add the correct number of chests
        while (totalChests < ChestCount)
        {
            foreach (Rect room in localRooms)
            {
                if (totalChests >= ChestCount)
                {
                    return; //no more chests needed!
                }
                
                Vector2 pos = new Rect(room.position + (Vector2.one), room.size - 2 * Vector2.one).RandomPoint();
                if (localChestWeapons.Count > 0)
                {
                    GameObject newChest = Instantiate(chest, pos, Quaternion.identity, transform);
                    int index = Random.Range(0, localChestWeapons.Count);
                    newChest.GetComponent<Chest>().SetItem(localChestWeapons[index]);
                    localChestWeapons.RemoveAt(index);
                }
                else if (chestItems.Count > 0)
                {
                    GameObject newChest = Instantiate(chest, pos, Quaternion.identity, transform);
                    int index = Random.Range(0, chestItems.Count);
                    newChest.GetComponent<Chest>().SetItem(chestItems[index]);    
                }
                else
                {
                    return; // no more items can be added, so no more chests are created.
                }
                totalChests++;
            }
            localRooms.Shuffle();
        }
    }

    public void PopulateRoomsWithEnemies(List<Rect> rooms)
    {
        Debug.Log("Populating rooms with enemies!");
        //copy lists since they will be mutated.
        List<Rect> localRooms = new List<Rect>(rooms);
        firstRoom = localRooms[0];
        localRooms.RemoveAt(0); //Remove starting room.
        localRooms.Shuffle();
        foreach (Rect room in localRooms)
        {
            if (Random.value > 0.4) // 60% chance a room will have enemies
            {
                int enemyCount = Random.Range(1, 6); //1-5 enemies per room.
                for (int i = 0; i < enemyCount; i++)
                {
                    int enemyIndex = Random.Range(0, enemies.Count);
                    Instantiate(enemies[enemyIndex], room.RandomPoint(), Quaternion.identity);
                }
            }
        }
    }

    public void PopulateHallwaysWithDoors(List<Rect> rooms, List<Rect> hallways)
    {
        foreach (Rect hallway in hallways)
        {
            if (Random.value > 0.75f)
            {
                Vector2 pos = hallway.center;

                Instantiate(door, pos, Quaternion.identity);
                DoorCount++;
            }
        }

        for (int i = 0; i < DoorCount; i++)
        {
            int index = Random.Range(0, rooms.Count);
            GameObject newChest = Instantiate(chest, rooms[index].RandomPoint(), Quaternion.identity, transform);
            newChest.GetComponent<Chest>().SetItem(key);
        }
    }

    public void SpawnGoal()
    {
        if (DoorCount <= 0)
        {
            Instantiate(goal, firstRoom.center, Random.rotation);
        }
    }
}    