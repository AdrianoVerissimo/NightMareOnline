using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

	public Transform target; //alvo que a câmera acompanhará
	public float smooth = 5f; //velocidade em que a câmera se moverá para acompanhar o alvo

	private Vector3 offset; //diferença inicial da posição da câmera com o alvo

	void Start()
	{
		//pega a diferença entre a posição do alvo com a da câmera
		offset = transform.position - target.position;
	}

	void FixedUpdate()
	{
		//define a posição da câmera como sendo a posição do alvo + a distância estabelecida pela diferença entre ambos
		Vector3 targetCamPos = target.position + offset;
		//move a câmera com a velocidade X para a posição do alvo
		transform.position = Vector3.Lerp (transform.position, targetCamPos, smooth * Time.deltaTime);
	}
}
