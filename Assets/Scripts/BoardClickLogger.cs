using UnityEngine;

// 이 스크립트를 사용하려면 2D Collider가 필수
[RequireComponent(typeof(BoxCollider2D))]
public class BoardClickLogger : MonoBehaviour
{
    void OnMouseDown()
    {
        Debug.Log("바둑판을 클릭했습니다!");
    }
}