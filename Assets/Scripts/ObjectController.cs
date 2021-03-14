using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{

    public bool CanMove;
    private Vector3 CurrentPosition;
    private Quaternion CurrentRotation;


    public float JumpHeight = .6f;
    public float Duration = .7f;

    private enum Rotation{
        top,
        bottom,
        left,
        right
    }

    private void Start() {
        CanMove = true;
        CurrentPosition = transform.position;
        CurrentRotation = transform.rotation;
    }

    public void MoveTo(Vector3 target, bool playEfects, bool isBackMove){
        if(CanMove){
            if(target.z < transform.position.z){
                StartCoroutine(Move(Duration, target, Rotation.bottom, playEfects, isBackMove));
            }
            else if(target.z> transform.position.z){
                StartCoroutine(Move(Duration, target, Rotation.top, playEfects, isBackMove));
            }
            else{
                if(target.x< transform.position.x){
                    StartCoroutine(Move(Duration, target, Rotation.left, playEfects, isBackMove));
                }
                else if(target.x> transform.position.x){
                    StartCoroutine(Move(Duration, target, Rotation.right, playEfects, isBackMove));
                }
            }
        }
    }

    IEnumerator Move(float duration, Vector3 target, Rotation rotation, bool playEfects, bool isBackMove){
        CanMove = false;
        float progress = 0f;
        Quaternion endRotation;

        switch(rotation){
            case Rotation.top:
                endRotation = Quaternion.Euler(-180,0,0);
                break;
            case Rotation.bottom:
                endRotation = Quaternion.Euler(180,0,0);
                break;
            case Rotation.right:
                endRotation = Quaternion.Euler(0,0,180);
                break;
            case Rotation.left:
                endRotation = Quaternion.Euler(0,0,-180);
                break;
            default:
                endRotation = Quaternion.Euler(0,0,0);
                break;
        }


        var targetPosition = target+ (isBackMove?new Vector3(0,0,0):new Vector3(0,transform.localScale.y,0));//new Vector3(0,transform.position.y,0);

        while(progress <duration){
            progress += Time.deltaTime;
            var percent = Mathf.Clamp01(progress/duration);
            float height = (JumpHeight)*Mathf.Sin(Mathf.PI*percent);

            transform.position = Vector3.Lerp(CurrentPosition,targetPosition,percent)+new Vector3(0,height,0);
            transform.rotation = Quaternion.Lerp(CurrentRotation,endRotation,percent);
            yield return null;
        }

        if(playEfects){
            foreach(Transform child in transform){
                child.GetComponent<ParticleSystem>().Play();
            }
        }
        
        CurrentPosition = new Vector3(transform.position.x, transform.position.y+target.y,transform.position.z);

        /*
        target.transform.SetParent(transform);
        foreach(Transform child in target.transform){
            child.SetParent(transform);
        }
        */
        CanMove = true;
        GameManager.instance.moves--;
    }

}
