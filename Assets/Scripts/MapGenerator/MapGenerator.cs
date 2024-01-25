using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public enum Grid
    {
        Empty,
        Floor,
        Wall,
    }
    public GameObject initialRoom;
    // Start is called before the first frame update
    public int roomNumber = 8;
    public int layOutWidth = 5;
    public int layOutHeight = 5;
    public int[,] roomLayout;
    public int roomWidth = 24;
    public int roomHeight = 13;
    public int corridorLength = 5;
    public Grid[,] grid;
    public Tilemap floorTileMap;
    public RuleTile floorRuleTile;
    void Start()
    {
        roomLayout = new int[layOutWidth, layOutHeight];
        grid = new Grid[layOutWidth * roomWidth + corridorLength * (layOutWidth - 1), layOutHeight * roomHeight + corridorLength * (layOutHeight - 1)];
    }
    public void GenerateRoomLayout()
    {
        // Clear the existing room layout
        for (int x = 0; x < layOutWidth; x++)
        {
            for (int y = 0; y < layOutHeight; y++)
            {
                roomLayout[x, y] = 0;
            }
        }

        // Set the starting room in the middle
        int startX = layOutWidth / 2;
        int startY = layOutHeight / 2;
        roomLayout[startX, startY] = 1;

        // Generate additional rooms
        int currentRoomCount = 1;
        while (currentRoomCount < roomNumber)
        {
            // Get a random direction
            int direction = Random.Range(0, 4);

            // Calculate the new room position based on the direction
            int newX = startX;
            int newY = startY;
            switch (direction)
            {
                case 0: // Up
                    newY++;
                    break;
                case 1: // Down
                    newY--;
                    break;
                case 2: // Left
                    newX--;
                    break;
                case 3: // Right
                    newX++;
                    break;
            }

            // Check if the new position is within the layout bounds and not already occupied
            if (newX >= 0 && newX < layOutWidth && newY >= 0 && newY < layOutHeight && roomLayout[newX, newY] == 0)
            {
                // Set the new room in the layout
                roomLayout[newX, newY] = 1;
                currentRoomCount++;

                // Update the current room position
                startX = newX;
                startY = newY;
            }
        }
    }
    public void GenerateRoomTileMap()
    {

    }
}
