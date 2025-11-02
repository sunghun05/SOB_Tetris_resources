using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class BlockControl1week : MonoBehaviour
{
    // 블록이 한 칸 아래로 떨어지는 데 걸리는 시간 (초)
    public float delay = 0.5f;

    public GameObject dummyObjectPrefab;

    public bool isFocus = false;
    public UnityEvent focusEvent;

    //private List<GameObject> blocks = new List<GameObject>();
    //private GameObject[] dummyBlocks = new GameObject[4];
    //두 리스트를 하나의 맵으로 관리
    private Dictionary<GameObject, GameObject> blocks = new Dictionary<GameObject, GameObject>();

    private List<GameObject> coliderDownList = new List<GameObject>();
    private List<Transform> childList = new List<Transform>();
    void Start()
    {

        foreach (Transform child in transform)
        {
            childList.Add(child);
            if (child.CompareTag("Block")) // 태그 검사
            {

                blocks.Add(child.gameObject, null);
                foreach (Transform grandChild in child)
                {

                    grandChild.gameObject.layer = LayerMask.NameToLayer("Collider");
                    if (grandChild.gameObject.name == "collider_down")
                    {
                        coliderDownList.Add(grandChild.gameObject);
                    }
                }
            }
        }


        if (isFocus)
        {
            //spawnDummyBlock(minLocation());

            //게임 시작 시 코루틴 시작
            StartCoroutine("Fall");
        }
    }

    void Update()
    {
        if (!isFocus) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //hardDrop();
            //deleteDummyBlock();
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            //deleteDummyBlock();
            if (CheckLeftRight(-1) == 0)
            {
                transform.position += Vector3.left;
            }
            //spawnDummyBlock(minLocation());
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            //deleteDummyBlock();
            if (CheckLeftRight(1) == 0)
            {
                transform.position += Vector3.right;
            }
            //spawnDummyBlock(minLocation());
        }


        //spin
        //반시계
        if (Input.GetKeyDown(KeyCode.Z))
        {
            //deleteDummyBlock();
            if (SpinBlock(90.0f)) // 회전이 성공했다면
            {
                //spawnDummyBlock(minLocation());
            }
            else // 실패했다면 더미 블록을 원래 위치에 다시 생성
            {
                //spawnDummyBlock(minLocation());
            }
        }
        //시계
        if (Input.GetKeyDown(KeyCode.X))
        {
            //deleteDummyBlock();
            if (SpinBlock(-90.0f)) // 회전이 성공했다면
            {
                //spawnDummyBlock(minLocation());
            }
            else // 실패했다면 더미 블록을 원래 위치에 다시 생성
            {
                //spawnDummyBlock(minLocation());
            }
        }
    }
    // 지정된 시간(fallTime)마다 블록을 아래로 한 칸씩 이동시키는 코루틴
    IEnumerator Fall()
    {
        while (true)
        {
            // fallTime만큼 기다립니다.
            yield return new WaitForSeconds(delay);
            // 아래로 한 칸 이동
            if (!isFocus) break;
            transform.position += Vector3.down;

        }
    }
    


    private bool IsValidPosition()
    {
        // 블록을 구성하는 모든 작은 조각(Mino)에 대해 반복
        foreach (GameObject block in this.blocks.Keys)
        {
            // 경계선 체크 (게임 보드의 가로 세로 크기에 맞게 수정 필요)
            int x = Mathf.RoundToInt(block.transform.position.x);
            int y = Mathf.RoundToInt(block.transform.position.y);

            //// 예시: 가로 10, 세로 20 크기의 보드
            if (x < -5 || x >= 5 || y < -7)
            {
                return false;
            }

        }
        return true;
    }

    private int CheckLeftRight(int direction)
    {
        int status = 0;
        //direction -1: left, 1: right

        foreach (Transform child in transform)
        {
            // Layer가 Tetrominoes 또는 Z인 오브젝트와 닿았을 때의 위치 반환
            // 이를 위해서 isFocus가 true인 오브젝트의 collider_? 오브젝트는 Collider 레이어로 변경함
            RaycastHit2D hit = Physics2D.Raycast(child.transform.position, Vector2.right * direction, 0.5f, LayerMask.GetMask("Z", "Tetrominoes"));
            if (hit.collider != null)
            {
                // 하드드랍을 진행했을 때 내려갈 수 있는 칸 수 계산
                // 현재 레이어를 쏜 오브젝트의 y 좌표는 0.5를 차감
                // 닿은 오브젝트는 반올림 진행
                status = 1;

                UnityEngine.Debug.DrawLine(child.transform.position, hit.point, Color.red, 2f);

            }
        }

        if (status != 0)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }

}