using UnityEngine;

public class CameraFollower : MonoBehaviour
{
	[SerializeField] public Transform m_ObjectToFollow;
	[SerializeField] public Vector3 m_Offset;

	private void Update()
	{
		var newPosition = this.m_ObjectToFollow.position + m_Offset; //Carrego a posicao do m_ObjectToFollow para uma variavel
		newPosition.z = this.transform.position.z; //Mudo a posicao "z" da variavel pra a da Camera, mantendo a posicao "z" da Camera.

		this.transform.position = newPosition; //Mudo a posicao atual da Camera para a posicao nova.
	}
}
