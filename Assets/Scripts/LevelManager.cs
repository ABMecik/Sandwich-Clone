using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    Vector3 StartPos = new Vector3(-1,0.2f,1);
    Vector3 IngrediantSize = new Vector3(1,0.2f,1);
    [SerializeField]
    public GameObject[] map = new GameObject[9];
    public List<GameObject>[,] GameBoard = new List<GameObject>[3,3];


    private void Start() {
        StartPos.y = IngrediantSize.y/2;

        Vector3 pos = StartPos;

        for(int x=0; x<3; x++){
            for(int y=0; y<3; y++){
                GameBoard[x,y] = new List<GameObject>();

                if(map[x+y*3] != null){
                    GameObject obj = Instantiate(map[x+y*3],pos,Quaternion.identity);
                    obj.transform.SetParent(transform);
                    GameBoard[x,y].Add(obj);
                }

                pos += new Vector3(0, 0, -IngrediantSize.z);
            }
            pos = new Vector3(pos.x,pos.y,StartPos.z);
            pos += new Vector3(IngrediantSize.x, 0, 0);
        }
    }

    public (int,int) GetLocationByObject(GameObject obj){

        for(int x=0; x<3; x++){
            for(int y=0; y<3; y++){
                if(GameBoard[x,y].Contains(obj)){
                    return (x,y);
                }
            }
        }
        return (0,0);
    }

    public List<GameObject> GetObjectLine(GameObject obj){
        var (x,y) = GetLocationByObject(obj);
        return GameBoard[x,y];
    }

    public void Transfer(List<GameObject> objs, int oldx, int oldy, int newx, int newy){
        foreach(GameObject obj in objs){
            GameBoard[oldx,oldy].Remove(obj);
            GameBoard[newx,newy].Add(obj);
        }
    }

    public (int,List<GameObject>) NonEmptyBoxesCount(){
        int counter = 0;
        List<GameObject> finalLine = null;
        for(int x=0; x<3; x++){
            for(int y=0; y<3; y++){
                if(GameBoard[x,y].Count>0){
                    counter++;
                    finalLine = GameBoard[x,y];
                }
            }
        }
        if(counter==1){
            return (counter,finalLine);
        }
        else{
            return (counter,null);
        }
    }

}
