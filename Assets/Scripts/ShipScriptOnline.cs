﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class ShipScriptOnline : MonoBehaviour
{

    public float xOffset = 0;
    public float zOffset = 0;
    private float nextZRotation = 90f;
    private GameObject clickedTile;
    int hitCount = 0;
    public int shipSize;


    List<GameObject> touchTiles = new List<GameObject>();


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Tile"))
        {
            touchTiles.Add(collision.gameObject);
        }
    }

    public void ClearTileList()
    {
        touchTiles.Clear();
    }


    public Vector3 GetOffsetVec(Vector3 tilePos)
    {
        return new Vector3(tilePos.x + xOffset, 2, tilePos.z + zOffset);
    }

    public void RotateShip()
    {
        if (clickedTile == null) return;
        touchTiles.Clear();
        transform.localEulerAngles += new Vector3(0, 0, nextZRotation);
        nextZRotation *= -1;
        float temp = xOffset;
        xOffset = zOffset;
        zOffset = temp;
        SetPosition(clickedTile.transform.position);
    }

    public void SetPosition(Vector3 newVec)
    {
        ClearTileList();
        transform.localPosition = new Vector3(newVec.x + xOffset, 2, newVec.z + zOffset);
    }

    public void SetClickedTile(GameObject tile)
    {
        clickedTile = tile;
    }

    public bool OnGameBoard()
    {
        return touchTiles.Count == shipSize;
    }

    public bool HitCheckSank()
    {
        hitCount++;
        return shipSize <= hitCount;
    }


    public int[] GetTileNumbers()
    {
        int[] tileNumbers = new int[touchTiles.Count];
        for (int i = 0; i < touchTiles.Count; i++)
        {
            string tileName = touchTiles[i].name;
            int tileNumber = Int32.Parse(Regex.Match(tileName, @"\d+").Value);
            tileNumbers[i] = tileNumber;
        }
        touchTiles.Clear();
        return tileNumbers;
    }

}
