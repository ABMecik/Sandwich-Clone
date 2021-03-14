using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Managers")]
    public Components components;
    [System.NonSerialized]
    public List<Move> History;
    [System.NonSerialized]
    public GameObject selected;
    [System.NonSerialized]
    public GameObject target;
    [System.NonSerialized]
    public int moves;


    [Header("Shake")]
    public float Magnitude = .05f;
    public float ShakeDuration = 1f;
    Vector3 cameraDefaultPosition;
    
    #region instance
    private void Awake() {
        if(instance == null){
            instance = this;
        }
    }
    #endregion

    void Start()
    {
        History = new List<Move>();
        moves = 0;
        cameraDefaultPosition = Camera.main.transform.position;
    }

    private void Update() {

        if(moves == 0){
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(Input.GetMouseButtonDown(0)){
                if(Physics.Raycast(ray, out hit, 100.0f)){
                    if(hit.transform != null){
                        if(hit.transform.CompareTag("ingredient") || hit.transform.CompareTag("Bread")){
                            //selected = selectTopObject(hit.transform.gameObject);
                            selected = hit.transform.gameObject;
                        }
                    }
                }
            }

            if(selected != null){
                if(Physics.Raycast(ray, out hit, 100.0f)){
                    if(hit.transform != null){
                        if(hit.transform.gameObject != selected){
                            if(hit.transform.CompareTag("ingredient") || hit.transform.CompareTag("Bread")){
                                if(Vector2.Distance(new Vector2(hit.transform.position.x,hit.transform.position.z),new Vector2(selected.transform.position.x,selected.transform.position.z))<=(selected.transform.localScale.x+0.1) && Vector2.Distance(new Vector2(hit.transform.position.x,hit.transform.position.z),new Vector2(selected.transform.position.x,selected.transform.position.z))>=(selected.transform.localScale.x-0.1)){
                                    target = hit.transform.gameObject;
                                    flip();
                                }
                            }
                        }
                    }
                }
            }
        }

        if(Input.GetMouseButtonUp(0)){
            selected = null;
            target = null;
        }
    }

    private GameObject selectTopObject(GameObject sel){
        GameObject current = sel;
        foreach(var obj in components.levelManager.map){
            if(obj != null){
                if(obj.transform.position.y > current.transform.position.y){
                    if(obj.transform.position.x == current.transform.position.x && obj.transform.position.z == current.transform.position.z){
                         current = obj;               
                    }
                }
            }
        }

        return current;
    }

    private void flip(){
        

        Vector3 targetPos = target.transform.position;
        List<GameObject> objs = components.levelManager.GetObjectLine(selected);
        var (x1,y1) = components.levelManager.GetLocationByObject(selected);
        var (x2,y2) = components.levelManager.GetLocationByObject(target);
        Move move = new Move(x1,y1,x2,y2);
        List<GameObject> recopy = new List<GameObject>();
        
        objs.Reverse();
        foreach(GameObject obj in objs){
            moves++;
            recopy.Add(obj);
            move.Add(obj,obj.transform.position);
            obj.GetComponent<ObjectController>().MoveTo(targetPos, selected==obj?true:false, false);
            targetPos.y += target.transform.localScale.y;
        }

        components.levelManager.Transfer(recopy,x1,y1,x2,y2);
        selected = null;
        target = null;

        History.Add(move);
        components.uIManager.ActivateBackButton();
        CheckWinConditions();
    }

    public void CheckWinConditions(){
        var (count,line) = components.levelManager.NonEmptyBoxesCount();
        if(count ==1){
            if(line[line.Count-1].CompareTag("Bread") && line[0].CompareTag("Bread")){
                Win();
            }
        }
    }

    public void Win(){
        components.uIManager.Win();
        components.uIManager.back.interactable = false;
        ShakeCamera();
    }

    public void Reload(){
        components.uIManager.winText.gameObject.SetActive(false);
        components.uIManager.back.interactable = false;
        ResetCameraPosition();
        StartCoroutine("AllBack");
    }

    IEnumerator AllBack(){
        while(History.Count>0){
            if(moves==0){
                BackMove();
            }
            yield return new WaitForSeconds(.1f);
        }
    }

    public void BackMove(){
        Move move = History[History.Count-1];
        History.RemoveAt(History.Count-1);

        components.levelManager.Transfer(move.Objects,move.newX,move.newY,move.oldX,move.oldY);
        for(int i=0; i<move.Objects.Count; i++){
            moves++;
            move.Objects[i].GetComponent<ObjectController>().MoveTo(move.Positions[i], false, true);
        }
    }

    private void ShakeCamera(){
        StartCoroutine("ShakeCameraEvent");
        Invoke("ResetCameraPosition", ShakeDuration);
    }

    IEnumerator ShakeCameraEvent(){
        while(true){
            float XOffset = Random.value*Magnitude*3-Magnitude;
            float YOffset = Random.value*Magnitude*3-Magnitude;
            Camera.main.transform.position += new Vector3(XOffset,YOffset,0);
            yield return new WaitForSeconds(0.1f);
            Camera.main.transform.position = cameraDefaultPosition;
        }
    }

    void ResetCameraPosition(){
        StopCoroutine("ShakeCameraEvent");
        Camera.main.transform.position = cameraDefaultPosition;
    }


    [System.Serializable]
    public class Components
    {
        public LevelManager levelManager;
        public UIManager uIManager;
    }
    
    public struct Move
    {
        public int oldX{ get; }
        public int oldY{ get; }
        public int newX{ get; }
        public int newY{ get; }
        public List<GameObject> Objects;
        public List<Vector3> Positions;

        public Move(int oldx, int oldy, int newx, int newy){
            Objects = new List<GameObject>();
            Positions = new List<Vector3>();

            oldX = oldx;
            oldY = oldy;
            newX = newx;
            newY = newy;
        }

        public void Add(GameObject obj, Vector3 position){
            Objects.Add(obj);
            Positions.Add(position);
        }
    }
}
