using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// MonoBehaviour : 
// Component 의 기본 단위, 생성자를 직접 호출해서 생성하는것이 아니고
// 해당 Script Instance 가 로드될 때 객체가 생성이 됨.
// -> 직접 우리가 생성자를 호출하면 안됨.
public class Test : MonoBehaviour
{
    // 씬이 로드되고나서 이 클래스를 컴포넌트로 가지는 게임오브젝트가 로드될때 이 클래스에 대한 스크립트 인스턴스도 로드됨.
    // (이 클래스를 컴포넌트로 가지는 게임오브잭트가 비활성화된 채로 씬이 로드되었다면, 스크립트인스턴스도 로드 ×
    // 활성화 되는 순간 스크립트 인스턴스도 로드함)
    // 스크립트 인스턴스가 로드될 때 한번 호출
    // 생성자에서 보통 구현하는 멤버 초기화 등에 대한 구현을 Awake()에다가 해주면 된다.
    private void Awake()
    {
        Debug.Log($"Awake");
    }

    private void OnEnable()
    {
        Debug.Log("Enabled");
    }

    private void Reset()
    {
        Debug.Log("Reset");
    }

    private void Start()
    {
        Debug.Log("Start");
    }

    private void FixedUpdate()
    {
        Debug.Log("Fixed Update");
    }

    private void OnTriggerEnter(Collider other)
    {

    }

    private void OnCollisionStay(Collision collision)
    {
        
    }

    private void OnMouseOver()
    {
        Debug.Log("On Mouse Over");
    }

    private void Update()
    {
        //Debug.Log("Update");
    }

    private void LateUpdate()
    {
        Debug.Log("Late Update");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(Vector3.zero, 2.0f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(Vector3.zero, 2.1f);
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Application Quit");
    }

    private void OnDisable()
    {
        Debug.Log("Disable");
    }

    private void OnDestroy()
    {
        Debug.Log("destroyed");
    }
}
